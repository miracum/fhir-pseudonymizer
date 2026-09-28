using System.ComponentModel.DataAnnotations;
using System.Text;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Tests.Kafka;

public class KafkaMessageProcessorTests
{
    private const string OutputTopic = "pseudonymized.input-topic";
    private const string DeadLetterTopic = "error.input-topic.fhir-pseudonymizer";

    private static readonly string PatientJson = new FhirJsonSerializer().SerializeToString(
        new Patient { Id = "123" }
    );

    private static ConsumeResult<byte[], string> CreateConsumeResult(
        string json,
        byte[] key = null,
        Headers headers = null
    )
    {
        return new ConsumeResult<byte[], string>
        {
            TopicPartitionOffset = new TopicPartitionOffset(
                new TopicPartition("input-topic", new Partition(0)),
                new Offset(0)
            ),
            Message = new Message<byte[], string>
            {
                Key = key,
                Value = json,
                Headers = headers,
            },
        };
    }

    private static KafkaMessageProcessor CreateProcessor(
        IAnonymizerEngine anonymizer,
        IProducer<byte[], string> producer,
        KafkaConfig kafkaConfig = null,
        IProvenancePublisher provenancePublisher = null
    )
    {
        kafkaConfig ??= new KafkaConfig();

        return new KafkaMessageProcessor(
            producer,
            anonymizer,
            A.Fake<AnonymizationConfig>(),
            // message keys are crypto-hashed by default, which requires a key
            kafkaConfig.CryptoHashMessageKeys.Key is null
                ? kafkaConfig with
                {
                    CryptoHashMessageKeys = kafkaConfig.CryptoHashMessageKeys with
                    {
                        Key = TestKeys.CryptoHashKey,
                    },
                }
                : kafkaConfig,
            provenancePublisher ?? A.Fake<IProvenancePublisher>(),
            A.Fake<ILogger<KafkaMessageProcessor>>()
        );
    }

    private static Func<KafkaMessageProcessor> Constructing(KafkaConfig kafkaConfig) =>
        () =>
            new KafkaMessageProcessor(
                A.Fake<IProducer<byte[], string>>(),
                A.Fake<IAnonymizerEngine>(),
                A.Fake<AnonymizationConfig>(),
                kafkaConfig,
                A.Fake<IProvenancePublisher>(),
                A.Fake<ILogger<KafkaMessageProcessor>>()
            );

    private static IAnonymizerEngine CreatePassThroughAnonymizer()
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
                (Resource resource, AnonymizerSettings _, CancellationToken _) =>
                    Task.FromResult(resource)
            );
        return anonymizer;
    }

    /// <summary>
    ///     A producer that records every produced message and acknowledges it right away, failing
    ///     the delivery of those produced to one of <paramref name="failingTopics" />.
    /// </summary>
    private static IProducer<byte[], string> CreateProducer(
        out List<(string Topic, Message<byte[], string> Message)> produced,
        params string[] failingTopics
    )
    {
        var messages = new List<(string, Message<byte[], string>)>();
        var producer = A.Fake<IProducer<byte[], string>>();
        A.CallTo(() =>
                producer.Produce(
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
                    messages.Add((topic, message));
                    deliveryHandler?.Invoke(
                        new DeliveryReport<byte[], string>
                        {
                            Topic = topic,
                            Message = message,
                            Error = failingTopics.Contains(topic)
                                ? new Error(ErrorCode.Local_MsgTimedOut)
                                : new Error(ErrorCode.NoError),
                        }
                    );
                }
            );

        produced = messages;
        return producer;
    }

    [Fact]
    public async Task ProcessAsync_ProducesTheSerializedAnonymizedResourceToThePrefixedTopic()
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        produced.Should().ContainSingle();
        produced[0].Topic.Should().Be(OutputTopic);
        produced[0].Message.Value.Should().Contain("\"id\":\"123\"");
    }

    [Fact]
    public async Task ProcessAsync_CryptoHashesTheMessageKeyByDefault()
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson, "patient-123"u8.ToArray()),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        // the lowercase hex HMAC-SHA256, just like the anonymization config's cryptoHash method
        // (by default) turns "patient-123" into, given the same key
        Encoding
            .UTF8.GetString(produced.Single().Message.Key)
            .Should()
            .Be(CryptoHashUtility.ComputeHmacSHA256Hash("patient-123", TestKeys.CryptoHashKey));
    }

    [Fact]
    public async Task ProcessAsync_WithCryptoHashingMessageKeysDisabled_PreservesOriginalMessageKey()
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(
            CreatePassThroughAnonymizer(),
            producer,
            new KafkaConfig { CryptoHashMessageKeys = new() { Enabled = false } }
        );
        var key = "patient-123"u8.ToArray();

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson, key),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        produced.Single().Message.Key.Should().BeEquivalentTo(key);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new byte[0])]
    public async Task ProcessAsync_WithoutAMessageKey_LeavesTheKeyAsItIs(byte[] key)
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson, key),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        produced.Single().Message.Key.Should().BeEquivalentTo(key);
    }

    [Fact]
    public void Constructor_WithoutACryptoHashMessageKeysKey_Throws()
    {
        Constructing(new KafkaConfig())
            .Should()
            .Throw<ValidationException>()
            .WithMessage("*Kafka__CryptoHashMessageKeys__Key*");
    }

    [Fact]
    public void Constructor_WithACryptoHashMessageKeysKeyShorterThan32Bytes_ThrowsWithoutRevealingIt()
    {
        const string key = "0123456789012345678901234567890";

        Constructing(new KafkaConfig { CryptoHashMessageKeys = new() { Key = key } })
            .Should()
            .Throw<ValidationException>()
            .Where(exc => exc.Message.Contains("31 bytes") && !exc.Message.Contains(key));
    }

    [Fact]
    public void Constructor_WithCryptoHashingMessageKeysDisabled_DoesNotRequireAKey()
    {
        Constructing(new KafkaConfig { CryptoHashMessageKeys = new() { Enabled = false } })
            .Should()
            .NotThrow();
    }

    [Fact]
    public async Task ProcessAsync_ForwardsOriginalMessageHeaders()
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);
        var headers = new Headers { { "traceparent", "trace-123"u8.ToArray() } };

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson, headers: headers),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        produced
            .Single()
            .Message.Headers.Should()
            .Contain(h => h.Key == "traceparent")
            .Which.GetValueBytes()
            .Should()
            .BeEquivalentTo("trace-123"u8.ToArray());
    }

    [Fact]
    public async Task ProcessAsync_ReportsProducedOnlyOnceTheBrokerAcknowledgedTheMessage()
    {
        Action<DeliveryReport<byte[], string>> pendingDelivery = null;
        var producer = A.Fake<IProducer<byte[], string>>();
        A.CallTo(() =>
                producer.Produce(
                    A<string>._,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .Invokes(
                (
                    string _,
                    Message<byte[], string> _,
                    Action<DeliveryReport<byte[], string>> deliveryHandler
                ) => pendingDelivery = deliveryHandler
            );
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        outcomes.Should().BeEmpty();

        pendingDelivery(
            new DeliveryReport<byte[], string>
            {
                Topic = OutputTopic,
                Error = new Error(ErrorCode.NoError),
            }
        );

        outcomes.Should().Equal(KafkaMessageOutcome.Produced);
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidJson_SendsOriginalMessageToDeadLetterTopicInstead()
    {
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(A.Fake<IAnonymizerEngine>(), producer);
        var key = "patient-123"u8.ToArray();
        var headers = new Headers { { "traceparent", "trace-123"u8.ToArray() } };

        await processor.ProcessAsync(
            CreateConsumeResult("not valid fhir json", key, headers),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        produced.Should().ContainSingle();
        var (topic, deadLetterMessage) = produced[0];
        topic.Should().Be(DeadLetterTopic);
        // not crypto-hashed: the dead letter topic gets the original message as it is
        deadLetterMessage.Key.Should().BeEquivalentTo(key);
        deadLetterMessage.Value.Should().Be("not valid fhir json");
        deadLetterMessage.Headers.Should().Contain(h => h.Key == "traceparent");
        deadLetterMessage.Headers.Should().Contain(h => h.Key == "x-error-type");
        deadLetterMessage
            .Headers.Should()
            .Contain(h => h.Key == "x-source-topic")
            .Which.GetValueBytes()
            .Should()
            .BeEquivalentTo("input-topic"u8.ToArray());
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidJson_ReportsDeadLetteredOnceTheDeadLetterMessageWasAcknowledged()
    {
        var processor = CreateProcessor(A.Fake<IAnonymizerEngine>(), CreateProducer(out _));

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult("not valid fhir json"),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        outcomes.Should().Equal(KafkaMessageOutcome.DeadLettered);
    }

    [Fact]
    public async Task ProcessAsync_WithAResourceThatIsNotValidFhir_SendsItToTheDeadLetterTopic()
    {
        // ids may only contain [A-Za-z0-9\-\.] as per the FHIR spec, which is validated while
        // parsing
        var json = """{"resourceType":"Patient","id":"pid_1"}""";
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(json),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        produced.Single().Topic.Should().Be(DeadLetterTopic);
        produced.Single().Message.Value.Should().Be(json);
        outcomes.Should().Equal(KafkaMessageOutcome.DeadLettered);
    }

    [Fact]
    public async Task ProcessAsync_WhenPseudonymizationFails_SendsOriginalMessageToDeadLetterTopic()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .Throws(new InvalidTimeZoneException("simulated non-Kafka, non-format exception"));
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(anonymizer, producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        produced.Should().ContainSingle();
        produced[0].Topic.Should().Be(DeadLetterTopic);
        produced[0].Message.Value.Should().Be(PatientJson);
        outcomes.Should().Equal(KafkaMessageOutcome.DeadLettered);
    }

    [Fact]
    public async Task ProcessAsync_WhenTheOutputMessageCannotBeDelivered_SendsOriginalMessageToDeadLetterTopic()
    {
        var producer = CreateProducer(out var produced, OutputTopic);
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        produced.Select(p => p.Topic).Should().Equal(OutputTopic, DeadLetterTopic);
        produced[1].Message.Value.Should().Be(PatientJson);
        outcomes.Should().Equal(KafkaMessageOutcome.DeadLettered);
    }

    [Fact]
    public async Task ProcessAsync_WhenTheDeadLetterMessageCannotBeDeliveredEither_ReportsFailed()
    {
        var processor = CreateProcessor(
            CreatePassThroughAnonymizer(),
            CreateProducer(out _, OutputTopic, DeadLetterTopic)
        );

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        outcomes.Should().Equal(KafkaMessageOutcome.Failed);
    }

    [Fact]
    public async Task ProcessAsync_WhenProducingToTheDeadLetterTopicThrows_ReportsFailedWithoutThrowing()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        A.CallTo(() =>
                producer.Produce(
                    A<string>._,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .Throws(new KafkaException(ErrorCode.Local_Transport));
        var processor = CreateProcessor(A.Fake<IAnonymizerEngine>(), producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor
            .Invoking(p =>
                p.ProcessAsync(
                    CreateConsumeResult("not valid fhir json"),
                    1,
                    outcomes.Add,
                    TestContext.Current.CancellationToken
                )
            )
            .Should()
            .NotThrowAsync();

        outcomes.Should().Equal(KafkaMessageOutcome.Failed);
    }

    [Fact]
    public async Task ProcessAsync_WhenTheProducerQueueIsFull_RetriesInsteadOfDeadLettering()
    {
        var attempts = 0;
        var producer = A.Fake<IProducer<byte[], string>>();
        A.CallTo(() =>
                producer.Produce(
                    A<string>._,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .Invokes(
                (
                    string topic,
                    Message<byte[], string> _,
                    Action<DeliveryReport<byte[], string>> deliveryHandler
                ) =>
                {
                    if (++attempts < 3)
                    {
                        throw new ProduceException<byte[], string>(
                            new Error(ErrorCode.Local_QueueFull),
                            null
                        );
                    }

                    deliveryHandler(
                        new DeliveryReport<byte[], string>
                        {
                            Topic = topic,
                            Error = new Error(ErrorCode.NoError),
                        }
                    );
                }
            );
        var processor = CreateProcessor(CreatePassThroughAnonymizer(), producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson),
            1,
            outcomes.Add,
            TestContext.Current.CancellationToken
        );

        attempts.Should().Be(3);
        A.CallTo(() =>
                producer.Produce(
                    DeadLetterTopic,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .MustNotHaveHappened();
        outcomes.Should().Equal(KafkaMessageOutcome.Produced);
    }

    [Fact]
    public async Task ProcessAsync_PublishesProvenanceForTheOriginalAndAnonymizedResourceWithForwardedHeaders()
    {
        var anonymized = new Patient { Id = "hashed-456" };
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .Returns(Task.FromResult<Resource>(anonymized));
        var provenancePublisher = A.Fake<IProvenancePublisher>();
        // Mirror KafkaProvenancePublisher: snapshot the resource before the anonymizer gets it.
        A.CallTo(() => provenancePublisher.CapturePreImage(A<Resource>._))
            .ReturnsLazily((Resource r) => (Resource)r.DeepCopy());
        var processor = CreateProcessor(
            anonymizer,
            A.Fake<IProducer<byte[], string>>(),
            provenancePublisher: provenancePublisher
        );
        var headers = new Headers { { "traceparent", "trace-123"u8.ToArray() } };

        Resource publishedOriginal = null;
        Resource publishedPseudonymized = null;
        Headers publishedHeaders = null;
        A.CallTo(() => provenancePublisher.Publish(A<Resource>._, A<Resource>._, A<Headers>._))
            .Invokes(
                (Resource o, Resource p, Headers h) =>
                {
                    publishedOriginal = o;
                    publishedPseudonymized = p;
                    publishedHeaders = h;
                }
            );

        await processor.ProcessAsync(
            CreateConsumeResult(PatientJson, headers: headers),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        publishedOriginal.Id.Should().Be("123");
        publishedPseudonymized.Should().BeSameAs(anonymized);
        publishedHeaders.Should().Contain(h => h.Key == "traceparent");
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidJson_DoesNotPublishProvenance()
    {
        var provenancePublisher = A.Fake<IProvenancePublisher>();
        var processor = CreateProcessor(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            provenancePublisher: provenancePublisher
        );

        await processor.ProcessAsync(
            CreateConsumeResult("not valid fhir json"),
            1,
            _ => { },
            TestContext.Current.CancellationToken
        );

        A.CallTo(() => provenancePublisher.Publish(A<Resource>._, A<Resource>._, A<Headers>._))
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task ProcessAsync_WhenPseudonymizationBackendIsTransientlyUnavailable_ThrowsForTheCallerToRetryInsteadOfDeadLettering()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .Throws(
                new TransientPseudonymizationException(
                    "backend unavailable",
                    new InvalidOperationException()
                )
            );
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(anonymizer, producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor
            .Invoking(p =>
                p.ProcessAsync(
                    CreateConsumeResult(PatientJson),
                    1,
                    outcomes.Add,
                    TestContext.Current.CancellationToken
                )
            )
            .Should()
            .ThrowAsync<TransientPseudonymizationException>();

        produced.Should().BeEmpty();
        outcomes.Should().BeEmpty();
    }

    [Fact]
    public async Task ProcessAsync_WhenRetryingATransientFailure_AnonymizesAFreshlyParsedResource()
    {
        // like the real anonymizer, this one modifies the resource it is given in place - and
        // then fails the first attempt, as if a pseudonymization backend call went wrong midway
        var attempt = 0;
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .ReturnsLazily(
                (Resource resource, AnonymizerSettings _, CancellationToken _) =>
                {
                    resource.Id += "-anonymized";
                    if (Interlocked.Increment(ref attempt) < 2)
                    {
                        throw new TransientPseudonymizationException(
                            "backend unavailable",
                            new InvalidOperationException()
                        );
                    }

                    return Task.FromResult(resource);
                }
            );
        var producer = CreateProducer(out var produced);
        var processor = CreateProcessor(anonymizer, producer);
        var result = CreateConsumeResult(PatientJson);

        await processor
            .Invoking(p =>
                p.ProcessAsync(result, 1, _ => { }, TestContext.Current.CancellationToken)
            )
            .Should()
            .ThrowAsync<TransientPseudonymizationException>();
        await processor.ProcessAsync(result, 2, _ => { }, TestContext.Current.CancellationToken);

        attempt.Should().Be(2);
        produced.Single().Message.Value.Should().Contain("\"id\":\"123-anonymized\"");
    }

    [Fact]
    public async Task ProcessAsync_WhenThePseudonymizationBackendIsUnavailableOnceCancelled_ReportsAbandonedWithoutProducing()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .Throws(
                new TransientPseudonymizationException(
                    "backend unavailable",
                    new InvalidOperationException()
                )
            );
        var producer = A.Fake<IProducer<byte[], string>>();
        var processor = CreateProcessor(anonymizer, producer);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var outcomes = new List<KafkaMessageOutcome>();
        await processor
            .Invoking(p =>
                p.ProcessAsync(CreateConsumeResult(PatientJson), 1, outcomes.Add, cts.Token)
            )
            .Should()
            .NotThrowAsync();

        outcomes.Should().Equal(KafkaMessageOutcome.Abandoned);
        A.CallTo(() =>
                producer.Produce(
                    A<string>._,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task ProcessAsync_WhenProcessingFailsAfterItWasCancelled_ReportsAbandonedInsteadOfDeadLettering()
    {
        using var cts = new CancellationTokenSource();
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() =>
                anonymizer.AnonymizeResourceAsync(
                    A<Resource>._,
                    A<AnonymizerSettings>._,
                    A<CancellationToken>._
                )
            )
            .ReturnsLazily(
                async Task<Resource> (Resource _, AnonymizerSettings _, CancellationToken _) =>
                {
                    // e.g. the host shutting down disposes the pseudonym cache under the message
                    await cts.CancelAsync();
                    throw new ObjectDisposedException("MemoryCache");
                }
            );
        var producer = A.Fake<IProducer<byte[], string>>();
        var processor = CreateProcessor(anonymizer, producer);

        var outcomes = new List<KafkaMessageOutcome>();
        await processor
            .Invoking(p =>
                p.ProcessAsync(CreateConsumeResult(PatientJson), 1, outcomes.Add, cts.Token)
            )
            .Should()
            .NotThrowAsync();

        outcomes.Should().Equal(KafkaMessageOutcome.Abandoned);
        A.CallTo(() =>
                producer.Produce(
                    A<string>._,
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .MustNotHaveHappened();
    }

    [Fact]
    public void GetOutputTopic_WithDefaultConfig_PrependsPseudonymizedPrefix()
    {
        var processor = CreateProcessor(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>()
        );

        processor.GetOutputTopic("input-topic").Should().Be("pseudonymized.input-topic");
    }

    [Fact]
    public void GetOutputTopic_WithCustomPattern_ReplacesMatchedSegment()
    {
        var processor = CreateProcessor(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig
            {
                OutputTopicPattern = "^fhir\\.",
                OutputTopicReplacement = "fhir.pseudonymized.",
            }
        );

        processor.GetOutputTopic("fhir.test").Should().Be("fhir.pseudonymized.test");
    }

    [Fact]
    public void GetDeadLetterTopic_WithDefaultGroupId_UsesDefaultGroupIdInTopicName()
    {
        var processor = CreateProcessor(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>()
        );

        processor
            .GetDeadLetterTopic("input-topic")
            .Should()
            .Be("error.input-topic.fhir-pseudonymizer");
    }

    [Fact]
    public void GetDeadLetterTopic_WithCustomGroupId_UsesConfiguredGroupIdInTopicName()
    {
        var processor = CreateProcessor(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig { Consumer = new ConsumerConfig { GroupId = "my-group" } }
        );

        processor.GetDeadLetterTopic("input-topic").Should().Be("error.input-topic.my-group");
    }
}
