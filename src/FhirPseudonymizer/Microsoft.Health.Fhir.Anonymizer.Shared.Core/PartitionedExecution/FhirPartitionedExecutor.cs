using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.Anonymizer.Core.PartitionedExecution;

namespace Microsoft.Health.Fhir.Anonymizer.Core
{
    public class FhirPartitionedExecutor<TSource, TResult>
    {
        public FhirPartitionedExecutor(IFhirDataReader<TSource> rawDataReader,
            IFhirDataConsumer<TResult> anonymizedDataConsumer)
        {
            RawDataReader = rawDataReader;
            AnonymizedDataConsumer = anonymizedDataConsumer;
            AnonymizerFunctionAsync = async content =>
            {
                return await Task.FromResult<TResult>(default).ConfigureAwait(false);
            };
        }

        public FhirPartitionedExecutor(IFhirDataReader<TSource> rawDataReader,
            IFhirDataConsumer<TResult> anonymizedDataConsumer, Func<TSource, TResult> anonymizerFunction)
        {
            RawDataReader = rawDataReader;
            AnonymizedDataConsumer = anonymizedDataConsumer;
            AnonymizerFunctionAsync = async content =>
            {
                var result = anonymizerFunction(content);
                return await Task.FromResult(result).ConfigureAwait(false);
            };
        }

        public FhirPartitionedExecutor(IFhirDataReader<TSource> rawDataReader,
            IFhirDataConsumer<TResult> anonymizedDataConsumer, Func<TSource, Task<TResult>> anonymizerFunctionAsync)
        {
            RawDataReader = rawDataReader;
            AnonymizedDataConsumer = anonymizedDataConsumer;
            AnonymizerFunctionAsync = anonymizerFunctionAsync;
        }

        public IFhirDataReader<TSource> RawDataReader { set; get; }

        public IFhirDataConsumer<TResult> AnonymizedDataConsumer { set; get; }

        public Func<TSource, Task<TResult>> AnonymizerFunctionAsync { set; get; }

        public int PartitionCount { set; get; } = Constants.DefaultPartitionedExecutionCount;

        public int BatchSize { set; get; } = Constants.DefaultPartitionedExecutionBatchSize;

        public bool KeepOrder { set; get; } = true;

        public async Task ExecuteAsync(CancellationToken cancellationToken, bool breakOnAnonymizationException = false,
            IProgress<BatchAnonymizeProgressDetail> progress = null)
        {
            var executionTasks = new List<Task<IEnumerable<TResult>>>();
            var batchData = new List<TSource>();

            TSource content;
            while ((content = await RawDataReader.NextAsync().ConfigureAwait(false)) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                batchData.Add(content);
                if (batchData.Count < BatchSize)
                {
                    continue;
                }

                executionTasks.Add(
                    AnonymizeAsync(batchData, breakOnAnonymizationException, progress, cancellationToken));
                batchData = new List<TSource>();
                if (executionTasks.Count < PartitionCount)
                {
                    continue;
                }

                await ConsumeExecutionResultTask(executionTasks, progress).ConfigureAwait(false);
            }

            if (batchData.Count > 0)
            {
                executionTasks.Add(
                    AnonymizeAsync(batchData, breakOnAnonymizationException, progress, cancellationToken));
            }

            while (executionTasks.Count > 0)
            {
                await ConsumeExecutionResultTask(executionTasks, progress).ConfigureAwait(false);
            }

            if (AnonymizedDataConsumer != null)
            {
                await AnonymizedDataConsumer.CompleteAsync().ConfigureAwait(false);
            }
        }

        private async Task<IEnumerable<TResult>> AnonymizeAsync(List<TSource> batchData,
            bool breakOnAnonymizationException, IProgress<BatchAnonymizeProgressDetail> progress,
            CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                var result = new List<TResult>();

                var batchAnonymizeProgressDetail = new BatchAnonymizeProgressDetail
                {
                    CurrentThreadId = Thread.CurrentThread.ManagedThreadId
                };

                foreach (var content in batchData)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException();
                    }

                    try
                    {
                        var anonymizedResult = await AnonymizerFunctionAsync(content);
                        result.Add(anonymizedResult);
                        batchAnonymizeProgressDetail.ProcessCompleted++;
                    }
                    catch (Exception)
                    {
                        if (breakOnAnonymizationException)
                        {
                            throw;
                        }

                        batchAnonymizeProgressDetail.ProcessFailed++;
                    }
                }

                progress?.Report(batchAnonymizeProgressDetail);
                return result;
            }).ConfigureAwait(false);
        }

        private async Task ConsumeExecutionResultTask(List<Task<IEnumerable<TResult>>> executionTasks,
            IProgress<BatchAnonymizeProgressDetail> progress)
        {
            if (KeepOrder)
            {
                var resultContents = await executionTasks.First().ConfigureAwait(false);
                executionTasks.RemoveAt(0);

                if (AnonymizedDataConsumer != null)
                {
                    var consumeCount = await AnonymizedDataConsumer.ConsumeAsync(resultContents).ConfigureAwait(false);
                    progress?.Report(new BatchAnonymizeProgressDetail
                    {
                        ConsumeCompleted = consumeCount,
                        CurrentThreadId = Thread.CurrentThread.ManagedThreadId
                    });
                }
            }
            else
            {
                await Task.WhenAny(executionTasks).ConfigureAwait(false);

                for (var index = 0; index < executionTasks.Count; ++index)
                {
                    if (executionTasks[index].IsCompleted)
                    {
                        var resultContents = await executionTasks[index].ConfigureAwait(false);
                        executionTasks.RemoveAt(index);

                        if (AnonymizedDataConsumer != null)
                        {
                            var consumeCount = await AnonymizedDataConsumer.ConsumeAsync(resultContents)
                                .ConfigureAwait(false);
                            progress?.Report(new BatchAnonymizeProgressDetail
                            {
                                ConsumeCompleted = consumeCount,
                                CurrentThreadId = Thread.CurrentThread.ManagedThreadId
                            });
                        }

                        break; // Only consume 1 result from completed task
                    }
                }
            }
        }
    }
}
