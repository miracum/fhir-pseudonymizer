using System.Text;
using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer.Tests.Kafka;

public class KafkaProvenancePublisherTests
{
    private static KafkaProvenancePublisher CreatePublisher(
        IProducer<byte[], string> producer,
        string provenanceTopic = "provenance-topic"
    )
    {
        return new KafkaProvenancePublisher(
            producer,
            new KafkaConfig { ProvenanceTopic = provenanceTopic },
            A.Fake<ILogger<KafkaProvenancePublisher>>()
        );
    }

    [Fact]
    public void Publish_WithPseudonymizedResource_ProducesProvenanceBundleToConfiguredTopic()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var publisher = CreatePublisher(producer);

        Message<byte[], string> producedMessage = null;
        A.CallTo(() =>
                producer.Produce(
                    "provenance-topic",
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

        publisher.Publish(new Patient { Id = "456" }, new Patient { Id = "hashed-456" });

        producedMessage.Should().NotBeNull();
        producedMessage.Value.Should().Contain("\"Patient/hashed-456\"");
        producedMessage.Value.Should().Contain("\"Provenance\"");
    }

    [Fact]
    public void Publish_ForwardsGivenHeadersOntoTheProducedMessage()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var publisher = CreatePublisher(producer);

        var headers = new Headers { { "traceparent", "trace-123"u8.ToArray() } };

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

        publisher.Publish(new Patient { Id = "456" }, new Patient { Id = "hashed-456" }, headers);

        producedMessage.Headers.Should().BeSameAs(headers);
    }

    [Fact]
    public void Publish_UsesTheBundlesIdAsTheKafkaMessageKey()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var publisher = CreatePublisher(producer);

        var original = new Patient { Id = "456" };
        var pseudonymized = new Patient { Id = "hashed-456" };
        // the id is deterministic and doesn't depend on "recorded", so this independently computed
        // bundle's id matches whatever the publisher produces internally
        var expectedId = ProvenanceFactory
            .CreateBundle(original, pseudonymized, DateTimeOffset.UtcNow)
            .Id;

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

        publisher.Publish(original, pseudonymized);

        producedMessage.Key.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(expectedId));
    }

    [Fact]
    public void Publish_WithResourceWithoutId_DoesNotProduceAnything()
    {
        var producer = A.Fake<IProducer<byte[], string>>();
        var publisher = CreatePublisher(producer);

        publisher.Publish(null, new Patient());

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
    public void Publish_WhenProducerThrowsKafkaException_DoesNotThrow()
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

        var publisher = CreatePublisher(producer);

        Action act = () => publisher.Publish(null, new Patient { Id = "123" });

        act.Should().NotThrow();
    }
}
