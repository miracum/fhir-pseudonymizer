using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using Confluent.Kafka;
using FhirPseudonymizer.Config;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Creates the consumer used by <see cref="KafkaConsumerService" />, wired to call back into it
///     whenever a consumer group rebalance assigns, revokes, or loses partitions. The callbacks
///     run on whichever thread is calling <see cref="IConsumer{TKey,TValue}.Consume(TimeSpan)" />.
/// </summary>
public delegate IConsumer<byte[], string> KafkaConsumerFactory(
    Action<IReadOnlyList<TopicPartition>> onPartitionsAssigned,
    Action<IReadOnlyList<TopicPartition>> onPartitionsRevoked,
    Action<IReadOnlyList<TopicPartition>> onPartitionsLost
);

/// <summary>
///     Consumes FHIR resources/bundles from one or more Kafka topics and hands them to a fixed
///     pool of workers that pseudonymize them via <see cref="KafkaMessageProcessor" />.
///
///     A single poll thread owns the <see cref="IConsumer{TKey,TValue}" /> (Consume/StoreOffset/
///     Pause/Resume are not guaranteed to be thread-safe) and all partition bookkeeping:
///     <list type="bullet">
///         <item>
///             Every assigned partition is pinned to one worker, so its messages are processed
///             in order, while different partitions are processed in parallel. Newly assigned
///             partitions go to whichever worker currently has the fewest, keeping the load even
///             across rebalances.
///         </item>
///         <item>
///             Each worker has a bounded queue (by message count and approximate size). When a
///             worker's queue is full, the poll thread waits for it to make room - that is the
///             normal backpressure while working through a backlog. Only if the worker doesn't
///             accept another message within <see cref="KafkaConfig.WorkerBusyTimeoutMs" /> (e.g.
///             because it is retrying a pseudonymization backend that is down) are its partitions
///             paused, so that the others keep being consumed and this consumer isn't kicked from
///             its group for exceeding max.poll.interval.ms. They are resumed once the worker
///             has worked off half of its queue. Pausing is deliberately kept out of the normal
///             path: librdkafka discards everything it has already prefetched for a partition
///             when pausing it, so pausing on every full queue means refetching most messages.
///         </item>
///         <item>
///             A message's offset is only stored (and later auto-committed) once the message it
///             was turned into has been acknowledged by the broker, and only once all earlier
///             messages of the same partition have been, too. If a message can neither be
///             processed nor sent to its dead letter topic, its partition can't move past it, so
///             the service stops: after a restart, it is reprocessed from the last committed offset.
///         </item>
///         <item>
///             When a rebalance revokes a partition (e.g. because another replica joined the
///             group), its queued messages are dropped, but the one already being processed is
///             given up to <see cref="RevocationTimeout" /> to be acknowledged, and its offset
///             stored before the partition is handed over. That way, its new owner doesn't process
///             and produce it a second time - out of order with the newer messages it produces.
///         </item>
///     </list>
/// </summary>
public class KafkaConsumerService : BackgroundService
{
    private static readonly UpDownCounter<long> PausedPartitionsCounter =
        Program.Meter.CreateUpDownCounter<long>(
            "fhirpseudonymizer.kafka.partitions_paused",
            description: "Number of partitions currently paused because the worker processing them stopped accepting new messages - most likely because it is retrying a transient pseudonymization backend failure."
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
    private readonly Worker[] workers;
    private readonly TimeSpan workerBusyTimeout;

    // Written by whichever thread reports a message's outcome, read by the poll thread.
    private readonly Channel<WorkItem> completedItems = Channel.CreateUnbounded<WorkItem>(
        new UnboundedChannelOptions { SingleReader = true }
    );

    // Everything below is only ever touched by the poll thread (see Run), including from within
    // the rebalance callbacks, which librdkafka invokes from inside Consume().
    private readonly Dictionary<TopicPartition, PartitionState> partitions = [];
    private IConsumer<byte[], string> consumer;
    private WorkItem failedItem;

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

        workerBusyTimeout = TimeSpan.FromMilliseconds(Math.Max(0, kafkaConfig.WorkerBusyTimeoutMs));
        workers =
        [
            .. Enumerable
                .Range(0, Math.Max(1, kafkaConfig.WorkerCount))
                .Select(index => new Worker(
                    index,
                    Math.Max(1, kafkaConfig.WorkerChannelCapacity),
                    Math.Max(1, kafkaConfig.WorkerChannelCapacityBytes)
                )),
        ];
    }

    protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // The poll loop blocks (in Consume, and while waiting for a busy worker), so it gets a
        // dedicated thread rather than tying up one of the thread pool's.
        return System.Threading.Tasks.Task.Factory.StartNew(
            () => Run(stoppingToken),
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default
        );
    }

    private void Run(CancellationToken stoppingToken)
    {
        consumer = consumerFactory(OnPartitionsAssigned, OnPartitionsRevoked, OnPartitionsLost);
        consumer.Subscribe(kafkaConfig.Topics);
        logger.LogInformation(
            "Subscribed to Kafka topics: {Topics} using {WorkerCount} workers",
            string.Join(", ", kafkaConfig.Topics),
            workers.Length
        );

        // Cancelled not only on shutdown but also when stopping because of a failed message, so
        // that workers stop picking up new ones in either case: anything not yet processed simply
        // doesn't get its offset stored and is reprocessed after a restart.
        using var workersCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            stoppingToken
        );
        var workerTasks = workers
            .Select(worker =>
                System.Threading.Tasks.Task.Run(() =>
                    RunWorkerAsync(worker, workersCancellation.Token)
                )
            )
            .ToArray();

        try
        {
            RunConsumeLoop(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // shutting down while waiting for a busy worker
        }
        finally
        {
            workersCancellation.Cancel();
            foreach (var partition in partitions.Values)
            {
                partition.Cancellation.Cancel();
            }

            foreach (var worker in workers)
            {
                worker.Complete();
            }

            System.Threading.Tasks.Task.WaitAll(workerTasks);

            // let the outcomes of messages that were already produced come in, so that their
            // offsets get stored and committed on close below instead of reprocessed on restart
            processor.Flush(ShutdownFlushTimeout);
            StoreCompletedOffsets();

            try
            {
                consumer.Close();
            }
            catch (KafkaException exc)
            {
                logger.LogError(exc, "Failed to cleanly close the Kafka consumer");
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

            // Tombstones (and partition EOF events) have no value to pseudonymize. They are
            // skipped without being tracked: their offsets are covered once the offset of any
            // later message of the same partition is stored.
            if (result?.Message?.Value is not null)
            {
                Dispatch(result, stoppingToken);
            }

            ResumeRecoveredWorkers();
            StoreCompletedOffsets();
        }
    }

    private void Dispatch(ConsumeResult<byte[], string> result, CancellationToken stoppingToken)
    {
        if (!partitions.TryGetValue(result.TopicPartition, out var partition))
        {
            // Only if librdkafka delivered a message without announcing the assignment first,
            // which it doesn't - kept so a partition can never be left without a worker.
            partition = AddPartition(result.TopicPartition);
        }

        var item = new WorkItem(result, partition);
        partition.InFlight.Enqueue(item);

        var worker = workers[partition.WorkerIndex];

        if (partition.HeldBack.Count > 0 || worker.IsStalled)
        {
            HoldBack(partition, item);
            return;
        }

        if (worker.TryEnqueue(item) || WaitToEnqueue(worker, item, stoppingToken))
        {
            return;
        }

        logger.LogWarning(
            "Worker {Worker} did not accept a new message within {Timeout}, pausing its partitions until it catches up",
            worker.Index,
            workerBusyTimeout
        );

        worker.IsStalled = true;
        HoldBack(partition, item);
    }

    /// <summary>
    ///     Waits for a worker with a full queue to make room for <paramref name="item" />, for up to
    ///     <see cref="KafkaConfig.WorkerBusyTimeoutMs" />. Keeps storing the offsets of completed
    ///     messages meanwhile, and stops early if one of them turned out to have failed.
    /// </summary>
    private bool WaitToEnqueue(Worker worker, WorkItem item, CancellationToken stoppingToken)
    {
        var stopwatch = Stopwatch.StartNew();

        while (true)
        {
            worker.SpaceAvailable.Reset();

            if (worker.TryEnqueue(item))
            {
                return true;
            }

            var remaining = workerBusyTimeout - stopwatch.Elapsed;
            if (remaining <= TimeSpan.Zero || failedItem is not null)
            {
                return false;
            }

            worker.SpaceAvailable.Wait(
                remaining < PollTimeout ? remaining : PollTimeout,
                stoppingToken
            );

            StoreCompletedOffsets();
        }
    }

    /// <summary>
    ///     Keeps a message whose worker isn't accepting new ones in memory instead, and pauses its
    ///     partition so librdkafka stops handing out more of them.
    /// </summary>
    private void HoldBack(PartitionState partition, WorkItem item)
    {
        partition.HeldBack.Enqueue(item);

        if (!partition.IsPaused)
        {
            consumer.Pause([partition.TopicPartition]);
            partition.IsPaused = true;
            PausedPartitionsCounter.Add(1);
        }
    }

    /// <summary>
    ///     Hands the held back messages of stalled workers that have since worked off half of their
    ///     queue over to them, oldest first, and resumes their partitions once all of them are.
    /// </summary>
    private void ResumeRecoveredWorkers()
    {
        foreach (var worker in workers)
        {
            if (!worker.IsStalled || !worker.IsAtMostHalfFull)
            {
                continue;
            }

            var allHandedOver = true;
            foreach (var partition in worker.Partitions)
            {
                while (partition.HeldBack.TryPeek(out var item) && worker.TryEnqueue(item))
                {
                    partition.HeldBack.Dequeue();
                }

                if (partition.HeldBack.Count > 0)
                {
                    allHandedOver = false;
                    break;
                }
            }

            if (!allHandedOver)
            {
                continue;
            }

            worker.IsStalled = false;
            foreach (var partition in worker.Partitions.Where(partition => partition.IsPaused))
            {
                Resume(partition);
            }

            logger.LogInformation(
                "Worker {Worker} caught up, resumed consuming its partitions",
                worker.Index
            );
        }
    }

    private void Resume(PartitionState partition)
    {
        consumer.Resume([partition.TopicPartition]);
        partition.IsPaused = false;
        PausedPartitionsCounter.Add(-1);
    }

    /// <summary>
    ///     Stores the offset of every partition up to (and including) its last message that - like
    ///     all of its predecessors - was either produced or dead-lettered. Stops at the first message
    ///     that was abandoned (only happens when stopping to process a partition anyway) or that
    ///     failed both, remembering the latter to stop the service.
    /// </summary>
    private void StoreCompletedOffsets()
    {
        while (completedItems.Reader.TryRead(out var completed))
        {
            var partition = completed.Partition;
            if (partition.IsRevoked)
            {
                continue;
            }

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
                continue;
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
    }

    private void OnPartitionsAssigned(IReadOnlyList<TopicPartition> assigned)
    {
        // the cooperative rebalance protocol also reports rebalances that didn't assign anything
        if (assigned.Count == 0)
        {
            return;
        }

        foreach (
            var topicPartition in assigned
                .OrderBy(p => p.Topic, StringComparer.Ordinal)
                .ThenBy(p => p.Partition.Value)
        )
        {
            AddPartition(topicPartition);
        }

        logger.LogInformation(
            "Assigned partitions {Partitions}, now processing {PartitionsPerWorker} partitions per worker",
            string.Join(", ", assigned),
            string.Join(", ", workers.Select(worker => worker.Partitions.Count))
        );
    }

    private void OnPartitionsRevoked(IReadOnlyList<TopicPartition> revoked)
    {
        if (revoked.Count == 0)
        {
            return;
        }

        var revokedPartitions = revoked
            .Select(topicPartition => partitions.GetValueOrDefault(topicPartition))
            .Where(partition => partition is not null)
            .ToList();

        foreach (var partition in revokedPartitions)
        {
            StopProcessing(partition);
        }

        WaitForStartedMessages(revokedPartitions);
        RemovePartitions(revoked);

        logger.LogInformation("Revoked partitions {Partitions}", string.Join(", ", revoked));
    }

    /// <summary>
    ///     Waits for those messages of revoked partitions that were already being processed to be
    ///     acknowledged (or abandoned), for up to <see cref="RevocationTimeout" />, storing their
    ///     offsets. librdkafka commits them right after the rebalance callback returns, before the
    ///     partitions are handed over.
    /// </summary>
    private void WaitForStartedMessages(List<PartitionState> revokedPartitions)
    {
        var stopwatch = Stopwatch.StartNew();

        while (true)
        {
            StoreCompletedOffsets();

            var inProgress = revokedPartitions
                .SelectMany(partition => partition.InFlight)
                .Where(item => item.IsStarted && item.Outcome is null)
                .ToList();

            if (inProgress.Count == 0)
            {
                return;
            }

            if (stopwatch.Elapsed > RevocationTimeout)
            {
                logger.LogWarning(
                    "Messages {TopicPartitionOffsets} were still being processed {Timeout} after their partitions were revoked; their partitions' new owners will process them again",
                    string.Join(", ", inProgress.Select(item => item.Result.TopicPartitionOffset)),
                    RevocationTimeout
                );
                return;
            }

            Thread.Sleep(10);
        }
    }

    /// <summary>
    ///     Makes sure none of a partition's messages that haven't been started yet will be, and
    ///     cancels retrying the one being processed, if it is.
    /// </summary>
    private static void StopProcessing(PartitionState partition)
    {
        foreach (var item in partition.InFlight)
        {
            item.TrySkip();
        }

        partition.HeldBack.Clear();
        partition.Cancellation.Cancel();
    }

    private void OnPartitionsLost(IReadOnlyList<TopicPartition> lost)
    {
        RemovePartitions(lost);

        logger.LogWarning("Lost partitions {Partitions}", string.Join(", ", lost));
    }

    private PartitionState AddPartition(TopicPartition topicPartition)
    {
        var worker = workers.MinBy(worker => worker.Partitions.Count);
        var partition = new PartitionState(topicPartition, worker.Index);

        partitions[topicPartition] = partition;
        worker.Partitions.Add(partition);

        return partition;
    }

    /// <summary>
    ///     Forgets partitions this consumer no longer owns. Their messages still queued for a
    ///     worker are skipped, since their new owner processes them anyway.
    /// </summary>
    private void RemovePartitions(IReadOnlyList<TopicPartition> removed)
    {
        foreach (var topicPartition in removed)
        {
            if (!partitions.Remove(topicPartition, out var partition))
            {
                continue;
            }

            StopProcessing(partition);
            partition.IsRevoked = true;
            workers[partition.WorkerIndex].Partitions.Remove(partition);

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
    }

    private async System.Threading.Tasks.Task RunWorkerAsync(
        Worker worker,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await foreach (var item in worker.ReadAllAsync(cancellationToken))
            {
                // skipped because its partition was revoked in the meantime
                if (!item.TryStart())
                {
                    continue;
                }

                try
                {
                    await processor.ProcessAsync(
                        item.Result,
                        outcome => Complete(item, outcome),
                        item.Partition.Cancellation.Token
                    );
                }
                catch (Exception exc)
                {
                    // ProcessAsync handles all expected failures, and cancellation, itself;
                    // anything else is a bug
                    logger.LogError(
                        exc,
                        "Unexpected error processing message from {TopicPartitionOffset}",
                        item.Result.TopicPartitionOffset
                    );
                    Complete(item, KafkaMessageOutcome.Failed);
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // stopping: messages still queued are left unprocessed
        }
    }

    private void Complete(WorkItem item, KafkaMessageOutcome outcome)
    {
        item.SetOutcome(outcome);
        completedItems.Writer.TryWrite(item);
    }

    /// <summary>
    ///     The worker a partition is pinned to, for the poll thread's bookkeeping and tests.
    /// </summary>
    internal IReadOnlyDictionary<TopicPartition, int> GetWorkerAssignments() =>
        partitions.ToDictionary(entry => entry.Key, entry => entry.Value.WorkerIndex);

    private sealed class WorkItem(ConsumeResult<byte[], string> result, PartitionState partition)
    {
        private const int Queued = 0;
        private const int Started = 1;
        private const int Skipped = 2;

        private int state = Queued;
        private int outcome = -1;

        public ConsumeResult<byte[], string> Result { get; } = result;

        public PartitionState Partition { get; } = partition;

        /// <summary>
        ///     Approximate memory held by the message while it waits for a worker: its key plus its
        ///     value, which is a UTF-16 string.
        /// </summary>
        public long Size { get; } =
            (result.Message.Key?.Length ?? 0) + (2L * result.Message.Value.Length);

        /// <summary>How the message was handled, or <c>null</c> while it is still in progress.</summary>
        public KafkaMessageOutcome? Outcome
        {
            get
            {
                var value = Volatile.Read(ref outcome);
                return value < 0 ? null : (KafkaMessageOutcome)value;
            }
        }

        public bool IsStarted => Volatile.Read(ref state) == Started;

        /// <summary>
        ///     Called by the worker before processing the message. Fails if the message was skipped
        ///     before, the two being atomic, so a message is either processed or skipped, never both.
        /// </summary>
        public bool TryStart() => Interlocked.CompareExchange(ref state, Started, Queued) == Queued;

        /// <summary>
        ///     Called by the poll thread to make sure the message won't be processed, unless it
        ///     already is.
        /// </summary>
        public bool TrySkip() => Interlocked.CompareExchange(ref state, Skipped, Queued) == Queued;

        /// <summary>Called once, by whichever thread learns how the message was handled.</summary>
        public void SetOutcome(KafkaMessageOutcome value) =>
            Volatile.Write(ref outcome, (int)value);
    }

    private sealed class PartitionState(TopicPartition topicPartition, int workerIndex)
    {
        private volatile bool isRevoked;

        public TopicPartition TopicPartition { get; } = topicPartition;

        public int WorkerIndex { get; } = workerIndex;

        /// <summary>
        ///     Every message consumed but not yet stored as consumed, in offset order.
        /// </summary>
        public Queue<WorkItem> InFlight { get; } = new();

        /// <summary>
        ///     Messages not yet handed to the worker because it stopped accepting them, in offset
        ///     order. The partition stays paused while there are any.
        /// </summary>
        public Queue<WorkItem> HeldBack { get; } = new();

        public bool IsPaused { get; set; }

        /// <summary>
        ///     Cancelled when this consumer stops processing the partition, to stop retrying a
        ///     message that is stuck, e.g. on an unavailable pseudonymization backend.
        /// </summary>
        public CancellationTokenSource Cancellation { get; } = new();

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

    /// <summary>
    ///     A worker's queue, bounded by both the number and the approximate size of the messages
    ///     in it. A message is always accepted into an empty queue, however large it is.
    /// </summary>
    private sealed class Worker(int index, int maxCount, long maxBytes)
    {
        private readonly Channel<WorkItem> queue = Channel.CreateUnbounded<WorkItem>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = true }
        );

        private int queuedCount;
        private long queuedBytes;

        public int Index { get; } = index;

        /// <summary>Only used by the poll thread.</summary>
        public List<PartitionState> Partitions { get; } = [];

        /// <summary>
        ///     Whether this worker didn't accept a message within the busy timeout and hasn't
        ///     caught up since. Only used by the poll thread.
        /// </summary>
        public bool IsStalled { get; set; }

        /// <summary>Set whenever the worker takes a message off its queue.</summary>
        public ManualResetEventSlim SpaceAvailable { get; } = new(false);

        public bool IsAtMostHalfFull =>
            Volatile.Read(ref queuedCount) <= maxCount / 2
            && Interlocked.Read(ref queuedBytes) <= maxBytes / 2;

        /// <summary>
        ///     Only ever called from the poll thread, so the queue can only have become emptier,
        ///     never fuller, between checking for room and adding to it.
        /// </summary>
        public bool TryEnqueue(WorkItem item)
        {
            var count = Volatile.Read(ref queuedCount);
            if (
                count > 0
                && (count >= maxCount || Interlocked.Read(ref queuedBytes) + item.Size > maxBytes)
            )
            {
                return false;
            }

            Interlocked.Increment(ref queuedCount);
            Interlocked.Add(ref queuedBytes, item.Size);
            queue.Writer.TryWrite(item);
            return true;
        }

        public async IAsyncEnumerable<WorkItem> ReadAllAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation]
                CancellationToken cancellationToken
        )
        {
            await foreach (var item in queue.Reader.ReadAllAsync(cancellationToken))
            {
                Interlocked.Decrement(ref queuedCount);
                Interlocked.Add(ref queuedBytes, -item.Size);
                SpaceAvailable.Set();

                yield return item;
            }
        }

        public void Complete() => queue.Writer.TryComplete();
    }
}
