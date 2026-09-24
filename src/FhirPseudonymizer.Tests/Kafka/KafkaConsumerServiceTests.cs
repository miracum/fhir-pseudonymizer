using System.Collections.Concurrent;
using System.Text;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests.Kafka;

public class KafkaConsumerServiceTests
{
    private const string InputTopic = "input-topic";
    private const string OutputTopic = "pseudonymized.input-topic";

    private static TopicPartition InputPartition(int partition) =>
        new(InputTopic, new Partition(partition));

    /// <summary>
    ///     A message whose key and Patient id both identify it as "p{partition}-o{offset}".
    /// </summary>
    private static ConsumeResult<byte[], string> Message(int partition, long offset)
    {
        var id = $"p{partition}-o{offset}";
        return new ConsumeResult<byte[], string>
        {
            TopicPartitionOffset = new TopicPartitionOffset(
                InputPartition(partition),
                new Offset(offset)
            ),
            Message = new Message<byte[], string>
            {
                Key = Encoding.UTF8.GetBytes(id),
                Value = $$"""{"resourceType":"Patient","id":"{{id}}"}""",
            },
        };
    }

    /// <summary>
    ///     Passes resources through unchanged, first awaiting <paramref name="beforeReturning" />
    ///     for each, if given.
    /// </summary>
    private static IAnonymizerEngine CreateAnonymizer(Func<Resource, Task> beforeReturning = null)
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .ReturnsLazily(
                async (Resource resource, AnonymizerSettings _, CancellationToken _) =>
                {
                    if (beforeReturning is not null)
                    {
                        await beforeReturning(resource);
                    }

                    return resource;
                }
            );
        return anonymizer;
    }

    private static KafkaConsumerService CreateService(
        ScriptedConsumer consumer,
        RecordingProducer producer,
        IAnonymizerEngine anonymizer,
        KafkaConfig kafkaConfig
    )
    {
        var processor = new KafkaMessageProcessor(
            producer.Producer,
            anonymizer,
            A.Fake<AnonymizationConfig>(),
            kafkaConfig,
            A.Fake<IProvenancePublisher>(),
            A.Fake<ILogger<KafkaMessageProcessor>>()
        );

        return new KafkaConsumerService(
            consumer.Factory,
            processor,
            kafkaConfig,
            A.Fake<ILogger<KafkaConsumerService>>()
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(12)]
    [InlineData(16)]
    public async Task AssignedPartitionsAreSpreadEvenlyAcrossWorkers(int workerCount)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var service = CreateService(
            consumer,
            new RecordingProducer(),
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = workerCount }
        );

        consumer.Assign([.. Enumerable.Range(0, 12).Select(InputPartition)]);
        var assignments = consumer.RunOnPollThread(service.GetWorkerAssignments);

        await service.StartAsync(cancellationToken);
        try
        {
            var partitionsPerWorker = (await assignments.WaitAsync(cancellationToken))
                .Values.GroupBy(worker => worker)
                .Select(group => group.Count())
                .ToList();

            partitionsPerWorker.Should().HaveCount(Math.Min(12, workerCount));
            (partitionsPerWorker.Max() - partitionsPerWorker.Min()).Should().BeLessThanOrEqualTo(1);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task PartitionsAssignedInALaterRebalanceGoToTheLeastLoadedWorkers()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var service = CreateService(
            consumer,
            new RecordingProducer(),
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = 2 }
        );

        consumer.Assign(InputPartition(0), InputPartition(1), InputPartition(2), InputPartition(3));
        var initial = consumer.RunOnPollThread(service.GetWorkerAssignments);
        consumer.Revoke(InputPartition(0), InputPartition(2));
        consumer.Assign(InputPartition(4), InputPartition(5));
        var rebalanced = consumer.RunOnPollThread(service.GetWorkerAssignments);

        await service.StartAsync(cancellationToken);
        try
        {
            var before = await initial.WaitAsync(cancellationToken);
            var after = await rebalanced.WaitAsync(cancellationToken);

            // partitions 0 and 2 shared a worker, so both new ones must go to that one
            before[InputPartition(0)].Should().Be(before[InputPartition(2)]);
            after
                .Keys.Should()
                .BeEquivalentTo(
                    new[]
                    {
                        InputPartition(1),
                        InputPartition(3),
                        InputPartition(4),
                        InputPartition(5),
                    }
                );
            after[InputPartition(4)].Should().Be(before[InputPartition(0)]);
            after[InputPartition(5)].Should().Be(before[InputPartition(0)]);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task OffsetsAreOnlyStoredOnceTheMessageAndAllItsPredecessorsWereAcknowledged()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer { DeliverImmediately = false };
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = 1 }
        );

        consumer.Assign(InputPartition(0));
        consumer.Deliver(Message(0, 0), Message(0, 1), Message(0, 2));

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(() => producer.Produced.Count == 3, cancellationToken);
            var produced = producer.Produced.ToArray();

            produced[1].Acknowledge();
            await consumer.RunOnPollThread(() => true).WaitAsync(cancellationToken);
            await consumer.RunOnPollThread(() => true).WaitAsync(cancellationToken);
            consumer.StoredOffsets.Should().BeEmpty();

            produced[0].Acknowledge();
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 1, cancellationToken);
            consumer.StoredOffsets.Should().Equal(new TopicPartitionOffset(InputPartition(0), 2));

            produced[2].Acknowledge();
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 2, cancellationToken);
            consumer.StoredOffsets[1].Should().Be(new TopicPartitionOffset(InputPartition(0), 3));
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        A.CallTo(() => consumer.Consumer.Close()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task AWorkerThatIsBusyButKeepsUp_DoesNotGetItsPartitionPaused()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(_ => Task.Delay(20, cancellationToken)),
            new KafkaConfig
            {
                WorkerCount = 1,
                WorkerChannelCapacity = 1,
                WorkerBusyTimeoutMs = 10_000,
            }
        );

        consumer.Assign(InputPartition(0));
        consumer.Deliver([.. Enumerable.Range(0, 5).Select(offset => Message(0, offset))]);

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(() => producer.Produced.Count == 5, cancellationToken);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        consumer.PausedPartitions.Should().BeEmpty();
        producer
            .Produced.Select(p => p.Key)
            .Should()
            .Equal("p0-o0", "p0-o1", "p0-o2", "p0-o3", "p0-o4");
    }

    [Fact]
    public async Task AWorkerThatStopsAcceptingMessages_GetsItsPartitionPausedUntilItCatchesUp()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var release = new TaskCompletionSource();
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(resource =>
                resource.Id == "p0-o0" ? release.Task : Task.CompletedTask
            ),
            new KafkaConfig
            {
                WorkerCount = 1,
                WorkerChannelCapacity = 1,
                WorkerBusyTimeoutMs = 50,
            }
        );

        // the first message blocks the only worker, the second fills its queue, and the third
        // can't be handed over
        consumer.Assign(InputPartition(0));
        consumer.Deliver(Message(0, 0), Message(0, 1), Message(0, 2));

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(
                () => consumer.PausedPartitions.Contains(InputPartition(0)),
                cancellationToken
            );

            release.TrySetResult();

            await WaitUntilAsync(() => producer.Produced.Count == 3, cancellationToken);
            await WaitUntilAsync(
                () => consumer.ResumedPartitions.Contains(InputPartition(0)),
                cancellationToken
            );
            await WaitUntilAsync(
                () => consumer.StoredOffsets.LastOrDefault()?.Offset.Value == 3,
                cancellationToken
            );
        }
        finally
        {
            release.TrySetResult();
            await service.StopAsync(cancellationToken);
        }

        producer.Produced.Select(p => p.Key).Should().Equal("p0-o0", "p0-o1", "p0-o2");
    }

    [Fact]
    public async Task WhileOneWorkerIsStuck_PartitionsOfOtherWorkersKeepBeingProcessed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var release = new TaskCompletionSource();
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(resource =>
                resource.Id.StartsWith("p0-", StringComparison.Ordinal)
                    ? release.Task
                    : Task.CompletedTask
            ),
            new KafkaConfig
            {
                WorkerCount = 2,
                WorkerChannelCapacity = 1,
                WorkerBusyTimeoutMs = 50,
            }
        );

        consumer.Assign(InputPartition(0), InputPartition(1));
        consumer.Deliver(Message(0, 0), Message(0, 1), Message(0, 2), Message(0, 3));
        consumer.Deliver(Message(1, 0), Message(1, 1), Message(1, 2));

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(
                () =>
                    producer.Produced.Count(p => p.Key.StartsWith("p1-", StringComparison.Ordinal))
                    == 3,
                cancellationToken
            );

            producer
                .Produced.Should()
                .NotContain(p => p.Key.StartsWith("p0-", StringComparison.Ordinal));
            consumer.PausedPartitions.Should().Equal(InputPartition(0));

            release.TrySetResult();

            await WaitUntilAsync(() => producer.Produced.Count == 7, cancellationToken);
        }
        finally
        {
            release.TrySetResult();
            await service.StopAsync(cancellationToken);
        }

        producer
            .Produced.Where(p => p.Key.StartsWith("p0-", StringComparison.Ordinal))
            .Select(p => p.Key)
            .Should()
            .Equal("p0-o0", "p0-o1", "p0-o2", "p0-o3");
    }

    [Fact]
    public async Task AMessageThatCanNeitherBeProducedNorDeadLettered_StopsTheServiceWithoutStoringItsOffset()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer { FailDeliveryOf = key => key == "p0-o0" };
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = 1 }
        );

        consumer.Assign(InputPartition(0));
        consumer.Deliver(Message(0, 0), Message(0, 1));

        await service.StartAsync(cancellationToken);
        try
        {
            await service
                .Invoking(s => s.ExecuteTask.WaitAsync(TimeSpan.FromSeconds(10), cancellationToken))
                .Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*could neither be processed nor sent to its dead letter topic*");
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        // the later message was produced fine, but storing its offset would skip the failed one
        producer.Produced.Should().Contain(p => p.Key == "p0-o1" && p.Topic == OutputTopic);
        consumer.StoredOffsets.Should().BeEmpty();
        A.CallTo(() => consumer.Consumer.Close()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task MessagesOfARevokedPartitionThatAreStillQueued_AreSkipped()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var processingStarted = new TaskCompletionSource();
        var release = new TaskCompletionSource();
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            consumer,
            producer,
            CreateAnonymizer(resource =>
            {
                if (resource.Id != "p0-o0")
                {
                    return Task.CompletedTask;
                }

                processingStarted.TrySetResult();
                return release.Task;
            }),
            new KafkaConfig { WorkerCount = 1 }
        );

        consumer.Assign(InputPartition(0), InputPartition(1));
        consumer.Deliver(Message(0, 0), Message(0, 1), Message(0, 2));

        await service.StartAsync(cancellationToken);
        try
        {
            await processingStarted.Task.WaitAsync(cancellationToken);

            consumer.Revoke(InputPartition(0));
            await consumer.RunOnPollThread(() => true).WaitAsync(cancellationToken);
            release.TrySetResult();

            // queued behind the revoked partition's messages on the only worker
            consumer.Deliver(Message(1, 0));
            await WaitUntilAsync(
                () => producer.Produced.Any(p => p.Key == "p1-o0"),
                cancellationToken
            );
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 1, cancellationToken);
        }
        finally
        {
            release.TrySetResult();
            await service.StopAsync(cancellationToken);
        }

        // the message already being processed when its partition was revoked still gets
        // produced, but neither is its offset stored nor are the queued ones processed at all
        producer.Produced.Select(p => p.Key).Should().Equal("p0-o0", "p1-o0");
        consumer.StoredOffsets.Should().Equal(new TopicPartitionOffset(InputPartition(1), 1));
    }

    private static async Task WaitUntilAsync(
        Func<bool> condition,
        CancellationToken cancellationToken,
        TimeSpan? timeout = null
    )
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));

        while (!condition())
        {
            if (DateTime.UtcNow > deadline)
            {
                throw new TimeoutException("Condition was not met within the timeout.");
            }

            await Task.Delay(10, cancellationToken);
        }
    }

    /// <summary>
    ///     A fake consumer that plays back a script of rebalances, messages, and arbitrary actions,
    ///     one step per call to Consume - so each of them runs on the service's poll thread, just
    ///     like librdkafka invokes rebalance callbacks from within Consume.
    /// </summary>
    private sealed class ScriptedConsumer
    {
        private readonly ConcurrentQueue<Func<ConsumeResult<byte[], string>>> script = new();
        private Action<IReadOnlyList<TopicPartition>> onPartitionsAssigned;
        private Action<IReadOnlyList<TopicPartition>> onPartitionsRevoked;

        public ScriptedConsumer()
        {
            A.CallTo(() => Consumer.Consume(A<TimeSpan>._))
                .ReturnsLazily(() =>
                {
                    if (script.TryDequeue(out var step))
                    {
                        return step();
                    }

                    Thread.Sleep(1);
                    return null;
                });
        }

        public IConsumer<byte[], string> Consumer { get; } = A.Fake<IConsumer<byte[], string>>();

        public KafkaConsumerFactory Factory =>
            (onAssigned, onRevoked, _) =>
            {
                onPartitionsAssigned = onAssigned;
                onPartitionsRevoked = onRevoked;
                return Consumer;
            };

        public List<TopicPartitionOffset> StoredOffsets =>
            [
                .. Fake.GetCalls(Consumer)
                    .Where(call =>
                        call.Method.Name == nameof(IConsumer<byte[], string>.StoreOffset)
                    )
                    .Select(call =>
                        call.Arguments[0] switch
                        {
                            ConsumeResult<byte[], string> result => new TopicPartitionOffset(
                                result.TopicPartition,
                                result.Offset + 1
                            ),
                            TopicPartitionOffset offset => offset,
                            _ => null,
                        }
                    ),
            ];

        public List<TopicPartition> PausedPartitions =>
            CallsTo(nameof(IConsumer<byte[], string>.Pause));

        public List<TopicPartition> ResumedPartitions =>
            CallsTo(nameof(IConsumer<byte[], string>.Resume));

        public void Assign(params TopicPartition[] partitions) =>
            script.Enqueue(() =>
            {
                onPartitionsAssigned(partitions);
                return null;
            });

        public void Revoke(params TopicPartition[] partitions) =>
            script.Enqueue(() =>
            {
                onPartitionsRevoked(partitions);
                return null;
            });

        public void Deliver(params ConsumeResult<byte[], string>[] results)
        {
            foreach (var result in results)
            {
                script.Enqueue(() => result);
            }
        }

        public Task<T> RunOnPollThread<T>(Func<T> action)
        {
            var completion = new TaskCompletionSource<T>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            script.Enqueue(() =>
            {
                completion.SetResult(action());
                return null;
            });
            return completion.Task;
        }

        private List<TopicPartition> CallsTo(string methodName) =>
            [
                .. Fake.GetCalls(Consumer)
                    .Where(call => call.Method.Name == methodName)
                    .SelectMany(call => (IEnumerable<TopicPartition>)call.Arguments[0]),
            ];
    }

    /// <summary>
    ///     A fake producer recording every produced message, which by default acknowledges each
    ///     one right away.
    /// </summary>
    private sealed class RecordingProducer
    {
        public RecordingProducer()
        {
            A.CallTo(() =>
                    Producer.Produce(
                        A<string>._,
                        A<Message<byte[], string>>._,
                        A<Action<DeliveryReport<byte[], string>>>._
                    )
                )
                .Invokes(
                    (
                        string topic,
                        Message<byte[], string> message,
                        Action<DeliveryReport<byte[], string>> deliveryHandler
                    ) =>
                    {
                        var key = Encoding.UTF8.GetString(message.Key);
                        var produced = new ProducedMessage(
                            topic,
                            key,
                            () =>
                                deliveryHandler(
                                    new DeliveryReport<byte[], string>
                                    {
                                        Topic = topic,
                                        Message = message,
                                        Error = FailDeliveryOf(key)
                                            ? new Error(ErrorCode.Local_MsgTimedOut)
                                            : new Error(ErrorCode.NoError),
                                    }
                                )
                        );
                        Produced.Enqueue(produced);

                        if (DeliverImmediately)
                        {
                            produced.Acknowledge();
                        }
                    }
                );
        }

        public IProducer<byte[], string> Producer { get; } = A.Fake<IProducer<byte[], string>>();

        public ConcurrentQueue<ProducedMessage> Produced { get; } = new();

        public bool DeliverImmediately { get; init; } = true;

        public Func<string, bool> FailDeliveryOf { get; init; } = _ => false;
    }

    private sealed record ProducedMessage(string Topic, string Key, Action Acknowledge);
}
