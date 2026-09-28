using System.Collections.Concurrent;
using System.Text;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests.Kafka;

public class KafkaConsumerServiceTests
{
    private const string InputTopic = "input-topic";

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
        KafkaConsumerFactory consumerFactory,
        RecordingProducer producer,
        IAnonymizerEngine anonymizer,
        KafkaConfig kafkaConfig,
        TimeSpan? retryDelay = null
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
            consumerFactory,
            processor,
            kafkaConfig,
            A.Fake<ILogger<KafkaConsumerService>>()
        )
        {
            RetryDelay = _ => retryDelay ?? TimeSpan.FromMinutes(1),
        };
    }

    private static TransientPseudonymizationException BackendUnavailable() =>
        new("backend unavailable", new InvalidOperationException());

    [Fact]
    public async Task OffsetsAreOnlyStoredOnceTheMessageAndAllItsPredecessorsWereAcknowledged()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer { DeliverImmediately = false };
        var service = CreateService(
            ScriptedConsumer.Factory(consumer),
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

            // e.g. because they went to different output partitions
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
    public async Task EachConsumerProcessesItsPartitionsIndependently_SoAStuckOneDoesNotHoldUpTheOthers()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var release = new TaskCompletionSource();
        var consumers = new[] { new ScriptedConsumer(), new ScriptedConsumer() };
        var producer = new RecordingProducer();
        var service = CreateService(
            ScriptedConsumer.Factory(consumers),
            producer,
            CreateAnonymizer(resource =>
                resource.Id == "p0-o0" ? release.Task : Task.CompletedTask
            ),
            new KafkaConfig { WorkerCount = 2 }
        );

        consumers[0].Assign(InputPartition(0));
        consumers[0].Deliver(Message(0, 0), Message(0, 1), Message(0, 2));
        consumers[1].Assign(InputPartition(1));
        consumers[1].Deliver(Message(1, 0), Message(1, 1), Message(1, 2));

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

            release.TrySetResult();
            await WaitUntilAsync(() => producer.Produced.Count == 6, cancellationToken);
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
            .Equal("p0-o0", "p0-o1", "p0-o2");
        consumers.Should().AllSatisfy(consumer => consumer.PausedPartitions.Should().BeEmpty());
    }

    [Fact]
    public async Task ATransientBackendFailure_PausesThePartitionUntilTheMessageIsRetried_WhileTheConsumersOtherPartitionsCarryOn()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var attempts = 0;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            ScriptedConsumer.Factory(consumer),
            producer,
            CreateAnonymizer(resource =>
                resource.Id == "p0-o0" && Interlocked.Increment(ref attempts) < 3
                    ? throw BackendUnavailable()
                    : Task.CompletedTask
            ),
            new KafkaConfig { WorkerCount = 1 },
            retryDelay: TimeSpan.FromMilliseconds(50)
        );

        consumer.Assign(InputPartition(0), InputPartition(1));
        // librdkafka stops handing out a paused partition's messages, this fake doesn't:
        // p0-o1 has to wait until p0-o0 went through
        consumer.Deliver(Message(0, 0), Message(0, 1), Message(1, 0));

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(() => producer.Produced.Count == 3, cancellationToken);
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 2, cancellationToken);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        attempts.Should().Be(3);
        producer.Produced.Select(p => p.Key).Should().Equal("p1-o0", "p0-o0", "p0-o1");
        consumer.PausedPartitions.Should().Equal(InputPartition(0));
        consumer.ResumedPartitions.Should().Equal(InputPartition(0));
        consumer
            .StoredOffsets.Should()
            .BeEquivalentTo([
                new TopicPartitionOffset(InputPartition(1), 1),
                new TopicPartitionOffset(InputPartition(0), 2),
            ]);
    }

    [Fact]
    public async Task AMessageThatCanNeitherBeProducedNorDeadLettered_StopsAllConsumersWithoutStoringItsOffset()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumers = new[] { new ScriptedConsumer(), new ScriptedConsumer() };
        var producer = new RecordingProducer { FailDeliveryOf = key => key == "p0-o0" };
        var service = CreateService(
            ScriptedConsumer.Factory(consumers),
            producer,
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = 2 }
        );

        consumers[0].Assign(InputPartition(0));
        consumers[0].Deliver(Message(0, 0), Message(0, 1));

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
        producer.Produced.Should().Contain(p => p.Key == "p0-o1");
        consumers[0].StoredOffsets.Should().BeEmpty();
        consumers
            .Should()
            .AllSatisfy(consumer =>
                A.CallTo(() => consumer.Consumer.Close()).MustHaveHappenedOnceExactly()
            );
    }

    [Fact]
    public async Task StoppingTheService_LetsTheMessageInProgressFinishAndStoresItsOffset_ButProcessesNoMore()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var processingStarted = new TaskCompletionSource();
        var release = new TaskCompletionSource();
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            ScriptedConsumer.Factory(consumer),
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

        consumer.Assign(InputPartition(0));
        consumer.Deliver([.. Enumerable.Range(0, 5).Select(offset => Message(0, offset))]);

        await service.StartAsync(cancellationToken);
        try
        {
            await processingStarted.Task.WaitAsync(cancellationToken);

            // cancels the service's stopping token right away, then waits for the consumer
            var stopping = service.StopAsync(cancellationToken);
            release.TrySetResult();
            await stopping;
        }
        finally
        {
            release.TrySetResult();
            await service.StopAsync(cancellationToken);
        }

        // the rest are reprocessed after a restart, from the stored offset on
        producer.Produced.Select(p => p.Key).Should().Equal("p0-o0");
        consumer.StoredOffsets.Should().Equal(new TopicPartitionOffset(InputPartition(0), 1));
        A.CallTo(() => consumer.Consumer.Close()).MustHaveHappenedOnceExactly();
        // e.g. for the Provenance messages produced along with the pseudonymized ones
        A.CallTo(() => producer.Producer.Flush(A<TimeSpan>._)).MustHaveHappened();
    }

    [Fact]
    public async Task RevokingAPartition_WaitsForItsProducedMessagesToBeAcknowledgedAndStoresTheirOffsetsBeforeHandingItOver()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer { DeliverImmediately = false };
        var service = CreateService(
            ScriptedConsumer.Factory(consumer),
            producer,
            CreateAnonymizer(),
            new KafkaConfig { WorkerCount = 1 }
        );

        consumer.Assign(InputPartition(0), InputPartition(1));
        consumer.Deliver(Message(0, 0), Message(0, 1));

        await service.StartAsync(cancellationToken);
        try
        {
            await WaitUntilAsync(() => producer.Produced.Count == 2, cancellationToken);

            consumer.Revoke(InputPartition(0));
            var storedOnceRevoked = consumer.RunOnPollThread(() => consumer.StoredOffsets);

            // the revocation waits for the produced messages to be acknowledged
            await Task.Delay(100, cancellationToken);
            storedOnceRevoked.IsCompleted.Should().BeFalse();
            foreach (var produced in producer.Produced)
            {
                produced.Acknowledge();
            }

            (await storedOnceRevoked.WaitAsync(cancellationToken))
                .Should()
                .Equal(new TopicPartitionOffset(InputPartition(0), 2));

            consumer.Deliver(Message(1, 0));
            await WaitUntilAsync(() => producer.Produced.Count == 3, cancellationToken);
            producer.Produced.Last().Acknowledge();
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 2, cancellationToken);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        consumer
            .StoredOffsets.Should()
            .Equal(
                new TopicPartitionOffset(InputPartition(0), 2),
                new TopicPartitionOffset(InputPartition(1), 1)
            );
    }

    [Fact]
    public async Task RevokingAPartition_DropsItsMessageWaitingToBeRetriedRightAway()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var firstAttempt = new TaskCompletionSource();
        var consumer = new ScriptedConsumer();
        var producer = new RecordingProducer();
        var service = CreateService(
            ScriptedConsumer.Factory(consumer),
            producer,
            CreateAnonymizer(resource =>
            {
                if (!resource.Id.StartsWith("p0-", StringComparison.Ordinal))
                {
                    return Task.CompletedTask;
                }

                firstAttempt.TrySetResult();
                throw BackendUnavailable();
            }),
            new KafkaConfig { WorkerCount = 1 }
        );

        consumer.Assign(InputPartition(0), InputPartition(1));
        consumer.Deliver(Message(0, 0));

        await service.StartAsync(cancellationToken);
        try
        {
            await firstAttempt.Task.WaitAsync(cancellationToken);

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            consumer.Revoke(InputPartition(0));
            await consumer.RunOnPollThread(() => true).WaitAsync(cancellationToken);
            stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));

            consumer.Deliver(Message(1, 0));
            await WaitUntilAsync(() => consumer.StoredOffsets.Count == 1, cancellationToken);
        }
        finally
        {
            await service.StopAsync(cancellationToken);
        }

        producer.Produced.Select(p => p.Key).Should().Equal("p1-o0");
        consumer.StoredOffsets.Should().Equal(new TopicPartitionOffset(InputPartition(1), 1));
        // not left paused, in case it is assigned to this consumer again
        consumer.ResumedPartitions.Should().Equal(InputPartition(0));
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
    ///     one step per call to Consume - so each of them runs on the consumer's thread, just like
    ///     librdkafka invokes rebalance callbacks from within Consume.
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

        /// <summary>Hands out the given consumers, one per index.</summary>
        public static KafkaConsumerFactory Factory(params ScriptedConsumer[] consumers) =>
            (index, onAssigned, onRevoked, _) =>
            {
                var consumer = consumers[index];
                consumer.onPartitionsAssigned = onAssigned;
                consumer.onPartitionsRevoked = onRevoked;
                return consumer.Consumer;
            };

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
