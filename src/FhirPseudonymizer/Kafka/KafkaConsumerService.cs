using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Creates the <paramref name="index" />th of the consumers used by
///     <see cref="KafkaConsumerService" />, wired to call back into it whenever a consumer group
///     rebalance assigns, revokes, or loses partitions. The callbacks run on whichever thread is
///     calling <see cref="IConsumer{TKey,TValue}.Consume(TimeSpan)" />.
/// </summary>
public delegate IConsumer<byte[], string> KafkaConsumerFactory(
    int index,
    Action<IReadOnlyList<TopicPartition>> onPartitionsAssigned,
    Action<IReadOnlyList<TopicPartition>> onPartitionsRevoked,
    Action<IReadOnlyList<TopicPartition>> onPartitionsLost
);

/// <summary>
///     Consumes FHIR resources/bundles from one or more Kafka topics and pseudonymizes them via
///     <see cref="KafkaMessageProcessor" />, using <see cref="KafkaConfig.WorkerCount" />
///     independent consumers in the same consumer group - like Spring Kafka's listener
///     concurrency. Each one runs on its own thread and processes the messages of the
///     partitions Kafka assigned to it one after another, so they are processed in order, while
///     different consumers' partitions are processed in parallel. A consumer that is busy simply
///     doesn't ask for more messages, and librdkafka stops prefetching for it once its queue
///     (queued.max.messages.kbytes) is full: there is no buffering, and no backpressure to
///     manage, in between.
///
///     <list type="bullet">
///         <item>
///             Produced messages aren't waited for one by one. A message's offset is stored (and
///             later auto-committed) once the message it was turned into, and its provenance, have
///             been acknowledged by the broker, and only once all earlier messages of the same
///             partition have been, too - they may go to different output partitions, which are
///             acknowledged independently. If a message can neither be processed nor sent to its
///             dead letter topic, its partition can't move past it, so the service stops: after a
///             restart, it is reprocessed from the last committed offset.
///         </item>
///         <item>
///             A message that fails because the pseudonymization backend is unavailable
///             (<see cref="TransientPseudonymizationException" />) is retried indefinitely, with
///             exponential backoff of up to a minute. Meanwhile, its partition is paused while
///             the consumer keeps polling, so it keeps its other partitions going and isn't kicked
///             from its group for exceeding max.poll.interval.ms.
///         </item>
///         <item>
///             Rebalances only take effect between messages, from within Consume. When one revokes
///             a partition (e.g. because another replica joined the group), the messages of it
///             that were produced but not yet acknowledged are given up to
///             <see cref="RevocationTimeout" /> to be, and their offsets stored before the
///             partition is handed over. That way, its new owner doesn't process and produce them
///             a second time - out of order with the newer messages it produces.
///         </item>
///     </list>
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private static readonly UpDownCounter<long> PausedPartitionsCounter =
        Program.Meter.CreateUpDownCounter<long>(
            "fhirpseudonymizer.kafka.partitions_paused",
            description: "Number of partitions currently paused while one of their messages waits to be retried after a transient pseudonymization backend failure."
        );

    private static readonly Gauge<int> PartitionsAssignedGauge = Program.Meter.CreateGauge<int>(
        "fhirpseudonymizer.kafka.partitions_assigned",
        description: "Number of partitions currently assigned to one of this instance's consumers, across all subscribed topics."
    );

    private static readonly TimeSpan PollTimeout = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan ShutdownFlushTimeout = TimeSpan.FromSeconds(10);

    // Bounded well below the rebalance timeout (max.poll.interval.ms, 5 minutes by default),
    // since the revoked partitions aren't consumed by anyone until this consumer lets go of them.
    private static readonly TimeSpan RevocationTimeout = TimeSpan.FromSeconds(10);

    private readonly KafkaConsumerFactory consumerFactory;
    private readonly KafkaMessageProcessor processor;
    private readonly KafkaConfig kafkaConfig;
    private readonly ILogger<KafkaConsumerService> logger;

    public KafkaConsumerService(
        KafkaConsumerFactory consumerFactory,
        KafkaMessageProcessor processor,
        KafkaConfig kafkaConfig,
        ILogger<KafkaConsumerService> logger
    )
    {
        this.consumerFactory = consumerFactory;
        this.processor = processor;
        this.kafkaConfig = kafkaConfig;
        this.logger = logger;
    }

    /// <summary>
    ///     How long to wait before making the given attempt at processing a message, after the
    ///     previous one failed because the pseudonymization backend was unavailable.
    /// </summary>
    internal Func<int, TimeSpan> RetryDelay { get; init; } =
        attempt => TimeSpan.FromSeconds(Math.Min(60, Math.Pow(2, attempt - 2)));

    protected override async System.Threading.Tasks.Task ExecuteAsync(
        CancellationToken stoppingToken
    )
    {
        // Cancelled not only on shutdown but also once any consumer stops because of a message
        // that could neither be processed nor dead-lettered, so that the others stop, too.
        using var stopping = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        var consumers = Enumerable
            .Range(0, Math.Max(1, kafkaConfig.WorkerCount))
            .Select(index => RunConsumer(index, stopping))
            .ToArray();

        try
        {
            await System.Threading.Tasks.Task.WhenAll(consumers);
        }
        finally
        {
            // The consumers only wait for the messages produced for the ones they consumed, not
            // for the provenance published for REST API requests by the same producer, which
            // would be lost if still queued when the producer is disposed.
            processor.Flush(ShutdownFlushTimeout);
        }
    }

    private System.Threading.Tasks.Task RunConsumer(int index, CancellationTokenSource stopping)
    {
        // Consume blocks, and so does processing a message, so each consumer gets a dedicated
        // thread rather than tying up one of the thread pool's.
        return System.Threading.Tasks.Task.Factory.StartNew(
            () =>
            {
                try
                {
                    new PartitionConsumer(this, index).Run(stopping.Token);
                }
                catch
                {
                    stopping.Cancel();
                    throw;
                }
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );
    }

    /// <summary>
    ///     One of the consumers, along with the bookkeeping for the partitions assigned to it. All
    ///     of it is only ever touched by the consumer's own thread (see <see cref="Run" />),
    ///     including from within the rebalance callbacks, which librdkafka invokes from inside
    ///     Consume - except for the outcomes of produced messages, which are reported by the
    ///     producer's delivery report thread.
    /// </summary>
    private sealed class PartitionConsumer(KafkaConsumerService service, int index)
    {
        private readonly ILogger logger = service.logger;
        private readonly KafkaMessageProcessor processor = service.processor;

        // Written by whichever thread reports a message's outcome, read by the consumer's thread.
        private readonly Channel<WorkItem> completedItems = Channel.CreateUnbounded<WorkItem>(
            new UnboundedChannelOptions { SingleReader = true }
        );

        private readonly Dictionary<TopicPartition, PartitionState> partitions = [];
        private IConsumer<byte[], string> consumer;
        private WorkItem failedItem;

        public void Run(CancellationToken stoppingToken)
        {
            consumer = service.consumerFactory(
                index,
                OnPartitionsAssigned,
                OnPartitionsRevoked,
                OnPartitionsLost
            );
            consumer.Subscribe(service.kafkaConfig.Topics);
            logger.LogInformation(
                "Consumer {Consumer} subscribed to Kafka topics: {Topics}",
                index,
                string.Join(", ", service.kafkaConfig.Topics)
            );

            try
            {
                RunConsumeLoop(stoppingToken);
            }
            finally
            {
                // let the outcomes of messages that were already produced come in, so that their
                // offsets get stored and committed on close below instead of reprocessed on restart
                WaitForProducedMessages([.. partitions.Values], ShutdownFlushTimeout);
                StoreCompletedOffsets();

                try
                {
                    consumer.Close();
                }
                catch (KafkaException exc)
                {
                    logger.LogError(
                        exc,
                        "Failed to cleanly close Kafka consumer {Consumer}",
                        index
                    );
                }
                finally
                {
                    consumer.Dispose();
                }
            }

            if (failedItem is not null)
            {
                throw new InvalidOperationException(
                    $"The message at {failedItem.Result.TopicPartitionOffset} could neither be processed nor sent to its dead letter topic. Stopping, so that it is reprocessed after a restart instead of being skipped."
                );
            }
        }

        private void RunConsumeLoop(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested && failedItem is null)
            {
                ConsumeResult<byte[], string> result;

                try
                {
                    result = consumer.Consume(PollTimeout);
                }
                catch (ConsumeException exc) when (!exc.Error.IsFatal)
                {
                    logger.LogError(exc, "Failed to consume message from Kafka");
                    continue;
                }

                StoreCompletedOffsets();
                RetryDueMessages(stoppingToken);

                // Tombstones (and partition EOF events) have no value to pseudonymize. They are
                // skipped without being tracked: their offsets are covered once the offset of any
                // later message of the same partition is stored.
                if (result?.Message?.Value is not null)
                {
                    Handle(result, stoppingToken);
                }
            }
        }

        private void Handle(ConsumeResult<byte[], string> result, CancellationToken stoppingToken)
        {
            if (!partitions.TryGetValue(result.TopicPartition, out var partition))
            {
                // Only if librdkafka delivered a message without announcing the assignment
                // first, which it doesn't - kept so a message can never go untracked.
                partition = AddPartition(result.TopicPartition);
            }

            // Pausing a partition makes librdkafka discard what it prefetched for it, so this
            // shouldn't happen - but should it, the message has to wait its turn.
            if (partition.PendingRetry is not null)
            {
                partition.HeldBack.Enqueue(result);
                return;
            }

            Process(new WorkItem(result, partition), attempt: 1, stoppingToken);
        }

        /// <summary>
        ///     Processes a message and hands the result to the producer. If the pseudonymization
        ///     backend is unavailable, pauses the message's partition until it is time to retry.
        /// </summary>
        private void Process(WorkItem item, int attempt, CancellationToken stoppingToken)
        {
            var partition = item.Partition;
            if (attempt == 1)
            {
                partition.InFlight.Enqueue(item);
            }

            try
            {
                processor
                    .ProcessAsync(
                        item.Result,
                        attempt,
                        outcome => Complete(item, outcome),
                        stoppingToken
                    )
                    .GetAwaiter()
                    .GetResult();

                item.IsProduced = true;
            }
            catch (TransientPseudonymizationException exc)
            {
                var delay = service.RetryDelay(attempt + 1);
                logger.LogWarning(
                    exc,
                    "Pseudonymization backend unavailable while processing message from {TopicPartitionOffset} (attempt {Attempt}); retrying in {Delay}",
                    item.Result.TopicPartitionOffset,
                    attempt,
                    delay
                );

                partition.PendingRetry = new PendingRetry(
                    item,
                    attempt + 1,
                    Stopwatch.GetTimestamp() + (long)(delay.TotalSeconds * Stopwatch.Frequency)
                );
                Pause(partition);
            }
            catch (Exception exc)
            {
                // ProcessAsync handles all expected failures, and cancellation, itself; anything
                // else is a bug
                logger.LogError(
                    exc,
                    "Unexpected error processing message from {TopicPartitionOffset}",
                    item.Result.TopicPartitionOffset
                );
                item.IsProduced = true;
                Complete(item, KafkaMessageOutcome.Failed);
            }
        }

        /// <summary>
        ///     Retries the messages whose backoff has elapsed, followed by any of their partition's
        ///     messages that came in meanwhile, and resumes their partitions once that worked.
        /// </summary>
        private void RetryDueMessages(CancellationToken stoppingToken)
        {
            var now = Stopwatch.GetTimestamp();
            var due = partitions
                .Values.Where(partition => partition.PendingRetry?.DueTimestamp <= now)
                .ToList();

            foreach (var partition in due)
            {
                var retry = partition.PendingRetry;
                partition.PendingRetry = null;
                Process(retry.Item, retry.Attempt, stoppingToken);

                while (
                    partition.PendingRetry is null && partition.HeldBack.TryDequeue(out var held)
                )
                {
                    Process(new WorkItem(held, partition), attempt: 1, stoppingToken);
                }

                if (partition.PendingRetry is null)
                {
                    Resume(partition);
                }
            }
        }

        /// <summary>Called once per message, by whichever thread learns its outcome.</summary>
        private void Complete(WorkItem item, KafkaMessageOutcome outcome)
        {
            item.SetOutcome(outcome);
            completedItems.Writer.TryWrite(item);
        }

        /// <summary>
        ///     Stores the offset of every partition up to (and including) its last message that -
        ///     like all of its predecessors - was either produced or dead-lettered. Stops at the
        ///     first message that was abandoned (only happens when stopping anyway) or that failed
        ///     both, remembering the latter to stop the service.
        /// </summary>
        private void StoreCompletedOffsets()
        {
            while (completedItems.Reader.TryRead(out var completed))
            {
                if (!completed.Partition.IsRevoked)
                {
                    StoreCompletedOffsets(completed.Partition);
                }
            }
        }

        private void StoreCompletedOffsets(PartitionState partition)
        {
            WorkItem lastCompleted = null;
            while (partition.InFlight.TryPeek(out var item) && item.Outcome is { } outcome)
            {
                if (outcome == KafkaMessageOutcome.Abandoned)
                {
                    break;
                }

                if (outcome == KafkaMessageOutcome.Failed)
                {
                    failedItem ??= item;
                    break;
                }

                lastCompleted = partition.InFlight.Dequeue();
            }

            if (lastCompleted is null)
            {
                return;
            }

            try
            {
                consumer.StoreOffset(lastCompleted.Result);
            }
            catch (KafkaException exc)
            {
                // e.g. the partition was revoked in the meantime without us being told
                logger.LogWarning(
                    exc,
                    "Failed to store offset {TopicPartitionOffset}",
                    lastCompleted.Result.TopicPartitionOffset
                );
            }
        }

        /// <summary>
        ///     Waits for the messages of the given partitions that were produced but not yet
        ///     acknowledged to be, for up to <paramref name="timeout" />, storing their offsets.
        /// </summary>
        private void WaitForProducedMessages(List<PartitionState> waitFor, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();

            while (true)
            {
                // Goes by the messages' outcomes themselves rather than the notifications about
                // them, which are only posted after recording an outcome - so everything seen as
                // done here is sure to be stored below.
                var unacknowledged = waitFor
                    .SelectMany(partition => partition.InFlight)
                    .Where(item => item.IsProduced && item.Outcome is null)
                    .ToList();

                foreach (var partition in waitFor)
                {
                    StoreCompletedOffsets(partition);
                }

                if (unacknowledged.Count == 0)
                {
                    return;
                }

                if (stopwatch.Elapsed > timeout)
                {
                    logger.LogWarning(
                        "Messages {TopicPartitionOffsets} were still unacknowledged after {Timeout}; they will be processed again",
                        string.Join(
                            ", ",
                            unacknowledged.Select(item => item.Result.TopicPartitionOffset)
                        ),
                        timeout
                    );
                    return;
                }

                Thread.Sleep(10);
            }
        }

        private void OnPartitionsAssigned(IReadOnlyList<TopicPartition> assigned)
        {
            // the cooperative rebalance protocol also reports rebalances that didn't assign anything
            if (assigned.Count == 0)
            {
                return;
            }

            foreach (var topicPartition in assigned)
            {
                AddPartition(topicPartition);
            }

            RecordAssignedPartitions();
            logger.LogInformation(
                "Consumer {Consumer} was assigned partitions {Partitions}",
                index,
                string.Join(", ", assigned)
            );
        }

        private void OnPartitionsRevoked(IReadOnlyList<TopicPartition> revoked)
        {
            if (revoked.Count == 0)
            {
                return;
            }

            // librdkafka commits the stored offsets right after this callback returns, before
            // the partitions are handed over
            WaitForProducedMessages(
                [
                    .. revoked
                        .Select(topicPartition => partitions.GetValueOrDefault(topicPartition))
                        .Where(partition => partition is not null),
                ],
                RevocationTimeout
            );
            RemovePartitions(revoked);

            logger.LogInformation(
                "Consumer {Consumer} had partitions {Partitions} revoked",
                index,
                string.Join(", ", revoked)
            );
        }

        private void OnPartitionsLost(IReadOnlyList<TopicPartition> lost)
        {
            RemovePartitions(lost);

            logger.LogWarning(
                "Consumer {Consumer} lost partitions {Partitions}",
                index,
                string.Join(", ", lost)
            );
        }

        private PartitionState AddPartition(TopicPartition topicPartition)
        {
            var partition = new PartitionState(topicPartition);
            partitions[topicPartition] = partition;
            return partition;
        }

        /// <summary>
        ///     Forgets partitions this consumer no longer owns, along with any of their messages
        ///     waiting to be retried: their new owner processes them anyway.
        /// </summary>
        private void RemovePartitions(IReadOnlyList<TopicPartition> removed)
        {
            foreach (var topicPartition in removed)
            {
                if (!partitions.Remove(topicPartition, out var partition))
                {
                    continue;
                }

                partition.IsRevoked = true;
                partition.PendingRetry = null;
                partition.HeldBack.Clear();

                if (partition.IsPaused)
                {
                    partition.IsPaused = false;
                    PausedPartitionsCounter.Add(-1);

                    // don't leave it paused in case it is assigned to this consumer again later
                    try
                    {
                        consumer.Resume([topicPartition]);
                    }
                    catch (KafkaException exc)
                    {
                        logger.LogDebug(
                            exc,
                            "Failed to resume removed partition {Partition}",
                            topicPartition
                        );
                    }
                }
            }

            RecordAssignedPartitions();
        }

        private void Pause(PartitionState partition)
        {
            if (partition.IsPaused)
            {
                return;
            }

            consumer.Pause([partition.TopicPartition]);
            partition.IsPaused = true;
            PausedPartitionsCounter.Add(1);
        }

        private void Resume(PartitionState partition)
        {
            if (!partition.IsPaused)
            {
                return;
            }

            consumer.Resume([partition.TopicPartition]);
            partition.IsPaused = false;
            PausedPartitionsCounter.Add(-1);
        }

        private void RecordAssignedPartitions() =>
            PartitionsAssignedGauge.Record(partitions.Count, new TagList { { "consumer", index } });
    }

    private sealed class WorkItem(ConsumeResult<byte[], string> result, PartitionState partition)
    {
        private int outcome = -1;

        public ConsumeResult<byte[], string> Result { get; } = result;

        public PartitionState Partition { get; } = partition;

        /// <summary>
        ///     Whether the message was handed to the producer, so an outcome is bound to follow,
        ///     rather than waiting to be retried. Only used by the consumer's thread.
        /// </summary>
        public bool IsProduced { get; set; }

        /// <summary>How the message was handled, or <c>null</c> while it is still in progress.</summary>
        public KafkaMessageOutcome? Outcome
        {
            get
            {
                var value = Volatile.Read(ref outcome);
                return value < 0 ? null : (KafkaMessageOutcome)value;
            }
        }

        /// <summary>Called once, by whichever thread learns how the message was handled.</summary>
        public void SetOutcome(KafkaMessageOutcome value) =>
            Volatile.Write(ref outcome, (int)value);
    }

    private sealed record PendingRetry(WorkItem Item, int Attempt, long DueTimestamp);

    private sealed class PartitionState(TopicPartition topicPartition)
    {
        private volatile bool isRevoked;

        public TopicPartition TopicPartition { get; } = topicPartition;

        /// <summary>
        ///     Every message consumed but not yet stored as consumed, in offset order - including
        ///     one waiting to be retried, which is always the last.
        /// </summary>
        public Queue<WorkItem> InFlight { get; } = new();

        /// <summary>The message waiting to be retried, if any. The partition is paused meanwhile.</summary>
        public PendingRetry PendingRetry { get; set; }

        /// <summary>Messages that came in while one was waiting to be retried, in offset order.</summary>
        public Queue<ConsumeResult<byte[], string>> HeldBack { get; } = new();

        public bool IsPaused { get; set; }

        /// <summary>
        ///     Set once the partition is no longer owned, so that outcomes still being reported for
        ///     its messages are ignored.
        /// </summary>
        public bool IsRevoked
        {
            get => isRevoked;
            set => isRevoked = value;
        }
    }
}
