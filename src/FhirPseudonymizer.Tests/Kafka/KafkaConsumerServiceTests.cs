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
    private static ConsumeResult<byte[], string> CreateConsumeResult(
        string topic,
        int partition,
        long offset,
        string json,
        byte[] key = null
    )
    {
        return new ConsumeResult<byte[], string>
        {
            TopicPartitionOffset = new TopicPartitionOffset(
                new TopicPartition(topic, new Partition(partition)),
                new Offset(offset)
            ),
            Message = new Message<byte[], string> { Key = key, Value = json },
        };
    }

    private static KafkaConsumerService CreateService(
        IAnonymizerEngine anonymizer,
        IProducer<byte[], string> producer,
        KafkaConfig kafkaConfig = null
    )
    {
        return new KafkaConsumerService(
            A.Fake<IConsumer<byte[], string>>(),
            producer,
            anonymizer,
            A.Fake<AnonymizationConfig>(),
            kafkaConfig ?? new KafkaConfig(),
            A.Fake<ILogger<KafkaConsumerService>>()
        );
    }

    [Fact]
    public void AnonymizeMessage_ReturnsAnonymizedResource()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .ReturnsLazily((Resource resource, AnonymizerSettings _) => resource);

        var service = CreateService(anonymizer, A.Fake<IProducer<byte[], string>>());

        var patient = new Patient { Id = "123" };
        var json = new Hl7.Fhir.Serialization.FhirJsonSerializer().SerializeToString(patient);

        var output = service.AnonymizeMessage("input-topic", json);

        output.Should().Contain("\"id\":\"123\"");
    }

    [Fact]
    public async System.Threading.Tasks.Task ProcessResultAsync_AnonymizesResourceAndProducesToPrefixedTopic()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .ReturnsLazily((Resource resource, AnonymizerSettings _) => resource);

        var producer = A.Fake<IProducer<byte[], string>>();
        var service = CreateService(anonymizer, producer);

        var json = new Hl7.Fhir.Serialization.FhirJsonSerializer().SerializeToString(
            new Patient { Id = "123" }
        );
        var result = CreateConsumeResult("input-topic", 0, 0, json);

        await service.ProcessResultAsync(result);

        A.CallTo(() =>
                producer.Produce(
                    "pseudonymized.input-topic",
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async System.Threading.Tasks.Task ProcessResultAsync_PreservesOriginalMessageKey()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .ReturnsLazily((Resource resource, AnonymizerSettings _) => resource);

        var producer = A.Fake<IProducer<byte[], string>>();
        var service = CreateService(anonymizer, producer);

        var json = new Hl7.Fhir.Serialization.FhirJsonSerializer().SerializeToString(
            new Patient { Id = "123" }
        );
        var key = "patient-123"u8.ToArray();
        var result = CreateConsumeResult("input-topic", 0, 0, json, key);

        Message<byte[], string> producedMessage = null;
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
                    Message<byte[], string> message,
                    Action<DeliveryReport<byte[], string>> _
                ) => producedMessage = message
            );

        await service.ProcessResultAsync(result);

        producedMessage.Key.Should().BeEquivalentTo(key);
    }

    [Fact]
    public async System.Threading.Tasks.Task ProcessResultAsync_WithInvalidJson_DoesNotProduceToOutputTopic()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var service = CreateService(A.Fake<IAnonymizerEngine>(), producer);

        var result = CreateConsumeResult("input-topic", 0, 0, "not valid fhir json");

        await service.ProcessResultAsync(result);

        A.CallTo(() =>
                producer.Produce(
                    "pseudonymized.input-topic",
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .MustNotHaveHappened();
    }

    [Fact]
    public async System.Threading.Tasks.Task ProcessResultAsync_WithInvalidJson_SendsOriginalMessageToDeadLetterTopic()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var service = CreateService(A.Fake<IAnonymizerEngine>(), producer);

        var key = "patient-123"u8.ToArray();
        var result = CreateConsumeResult("input-topic", 0, 0, "not valid fhir json", key);

        Message<byte[], string> deadLetterMessage = null;
        A.CallTo(() =>
                producer.Produce(
                    "error.input-topic.fhir-pseudonymizer",
                    A<Message<byte[], string>>._,
                    A<Action<DeliveryReport<byte[], string>>>._
                )
            )
            .Invokes(
                (
                    string _,
                    Message<byte[], string> message,
                    Action<DeliveryReport<byte[], string>> _
                ) => deadLetterMessage = message
            );

        await service.ProcessResultAsync(result);

        deadLetterMessage.Should().NotBeNull();
        deadLetterMessage.Key.Should().BeEquivalentTo(key);
        deadLetterMessage.Value.Should().Be("not valid fhir json");
        deadLetterMessage
            .Headers.Should()
            .Contain(h => h.Key == "x-source-topic")
            .Which.GetValueBytes()
            .Should()
            .BeEquivalentTo("input-topic"u8.ToArray());
    }

    [Fact]
    public async System.Threading.Tasks.Task ProcessResultAsync_WhenDeadLetterProduceAlsoFails_DoesNotThrow()
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

        var service = CreateService(A.Fake<IAnonymizerEngine>(), producer);
        var result = CreateConsumeResult("input-topic", 0, 0, "not valid fhir json");

        await service.Invoking(s => s.ProcessResultAsync(result)).Should().NotThrowAsync();
    }

    [Fact]
    public void GetOutputTopic_WithDefaultConfig_PrependsPseudonymizedPrefix()
    {
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>()
        );

        service.GetOutputTopic("input-topic").Should().Be("pseudonymized.input-topic");
    }

    [Fact]
    public void GetOutputTopic_WithCustomPattern_ReplacesMatchedSegment()
    {
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig
            {
                OutputTopicPattern = "^fhir\\.",
                OutputTopicReplacement = "fhir.pseudonymized.",
            }
        );

        service.GetOutputTopic("fhir.test").Should().Be("fhir.pseudonymized.test");
    }

    [Fact]
    public void GetDeadLetterTopic_WithDefaultGroupId_UsesDefaultGroupIdInTopicName()
    {
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>()
        );

        service
            .GetDeadLetterTopic("input-topic")
            .Should()
            .Be("error.input-topic.fhir-pseudonymizer");
    }

    [Fact]
    public void GetDeadLetterTopic_WithCustomGroupId_UsesConfiguredGroupIdInTopicName()
    {
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig { Consumer = new ConsumerConfig { GroupId = "my-group" } }
        );

        service.GetDeadLetterTopic("input-topic").Should().Be("error.input-topic.my-group");
    }

    [Fact]
    public void GetWorkerIndex_IsStableForTheSamePartition()
    {
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig { WorkerCount = 8 }
        );

        var partition = new TopicPartition("input-topic", new Partition(3));

        var firstIndex = service.GetWorkerIndex(partition);
        var secondIndex = service.GetWorkerIndex(partition);

        firstIndex.Should().Be(secondIndex);
        firstIndex.Should().BeInRange(0, 7);
    }

    [Fact]
    public void GetWorkerIndex_DistributesPartitionsAcrossAllWorkers()
    {
        const int workerCount = 4;
        var service = CreateService(
            A.Fake<IAnonymizerEngine>(),
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig { WorkerCount = workerCount }
        );

        var usedIndices = Enumerable
            .Range(0, 12)
            .Select(partitionNumber =>
                service.GetWorkerIndex(
                    new TopicPartition("input-topic", new Partition(partitionNumber))
                )
            )
            .Distinct()
            .ToList();

        usedIndices.Should().HaveCountGreaterThan(1);
        usedIndices.Should().OnlyContain(index => index >= 0 && index < workerCount);
    }
}
