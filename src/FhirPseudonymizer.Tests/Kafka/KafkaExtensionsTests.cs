using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using Microsoft.Extensions.Configuration;

namespace FhirPseudonymizer.Tests.Kafka;

public class KafkaExtensionsTests
{
    [Fact]
    public void CreateConsumerConfig_MergesClientAndConsumerSettings()
    {
        var kafkaConfig = new KafkaConfig
        {
            Client = new ClientConfig { BootstrapServers = "broker:9092" },
            Consumer = new ConsumerConfig { GroupId = "my-group", SessionTimeoutMs = 45000 },
        };

        var consumerConfig = KafkaExtensions.CreateConsumerConfig(kafkaConfig);

        consumerConfig.BootstrapServers.Should().Be("broker:9092");
        consumerConfig.GroupId.Should().Be("my-group");
        consumerConfig.SessionTimeoutMs.Should().Be(45000);
    }

    [Fact]
    public void CreateConsumerConfig_AppliesSaneDefaultsWhenUnset()
    {
        var consumerConfig = KafkaExtensions.CreateConsumerConfig(new KafkaConfig());

        consumerConfig.GroupId.Should().Be("fhir-pseudonymizer");
        consumerConfig.AutoOffsetReset.Should().Be(AutoOffsetReset.Earliest);
        consumerConfig
            .PartitionAssignmentStrategy.Should()
            .Be(PartitionAssignmentStrategy.CooperativeSticky);
    }

    [Fact]
    public void CreateConsumerConfig_AllowsOverridingDefaultsViaConsumerSection()
    {
        var kafkaConfig = new KafkaConfig
        {
            Consumer = new ConsumerConfig { AutoOffsetReset = AutoOffsetReset.Latest },
        };

        var consumerConfig = KafkaExtensions.CreateConsumerConfig(kafkaConfig);

        consumerConfig.AutoOffsetReset.Should().Be(AutoOffsetReset.Latest);
    }

    [Fact]
    public void CreateConsumerConfig_AlwaysDisablesAutoOffsetStoreRegardlessOfConfig()
    {
        var kafkaConfig = new KafkaConfig
        {
            Consumer = new ConsumerConfig { EnableAutoOffsetStore = true },
        };

        var consumerConfig = KafkaExtensions.CreateConsumerConfig(kafkaConfig);

        consumerConfig.EnableAutoOffsetStore.Should().BeFalse();
        consumerConfig.EnableAutoCommit.Should().BeTrue();
    }

    [Fact]
    public void CreateProducerConfig_MergesClientAndProducerSettings()
    {
        var kafkaConfig = new KafkaConfig
        {
            Client = new ClientConfig { BootstrapServers = "broker:9092" },
            Producer = new ProducerConfig { LingerMs = 5, CompressionType = CompressionType.Zstd },
        };

        var producerConfig = KafkaExtensions.CreateProducerConfig(kafkaConfig);

        producerConfig.BootstrapServers.Should().Be("broker:9092");
        producerConfig.LingerMs.Should().Be(5);
        producerConfig.CompressionType.Should().Be(CompressionType.Zstd);
    }

    [Fact]
    public void CreateConsumerConfig_DoesNotLeakConsumerOnlySettingsIntoSharedClientConfig()
    {
        var kafkaConfig = new KafkaConfig
        {
            Client = new ClientConfig { BootstrapServers = "broker:9092" },
        };

        // CreateConsumerConfig sets GroupId, AutoOffsetReset, PartitionAssignmentStrategy,
        // EnableAutoCommit and EnableAutoOffsetStore on the returned ConsumerConfig - none of
        // those should end up on kafkaConfig.Client, since ProducerConfig is also built from it.
        KafkaExtensions.CreateConsumerConfig(kafkaConfig);

        kafkaConfig
            .Client.Should()
            .BeEquivalentTo(new ClientConfig { BootstrapServers = "broker:9092" });

        var producerConfig = KafkaExtensions.CreateProducerConfig(kafkaConfig);
        var producerKeys = producerConfig.Select(kvp => kvp.Key);

        producerKeys
            .Should()
            .NotContain(
                new[]
                {
                    "group.id",
                    "auto.offset.reset",
                    "partition.assignment.strategy",
                    "enable.auto.commit",
                    "enable.auto.offset.store",
                }
            );
    }

    [Fact]
    public void KafkaConfig_BindsArbitraryClientConsumerAndProducerSettingsFromConfiguration()
    {
        var settings = new Dictionary<string, string>
        {
            ["Kafka:Topics:0"] = "topic-a",
            ["Kafka:Topics:1"] = "topic-b",
            ["Kafka:Client:BootstrapServers"] = "broker1:9092,broker2:9092",
            ["Kafka:Client:SecurityProtocol"] = "SaslSsl",
            ["Kafka:Client:SaslMechanism"] = "ScramSha512",
            ["Kafka:Consumer:GroupId"] = "my-group",
            ["Kafka:Consumer:SessionTimeoutMs"] = "45000",
            ["Kafka:Producer:LingerMs"] = "5",
            ["Kafka:Producer:CompressionType"] = "Zstd",
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

        var appConfig = new AppConfig();
        configuration.Bind(appConfig);

        appConfig.Kafka.Topics.Should().Equal("topic-a", "topic-b");
        appConfig.Kafka.Client.BootstrapServers.Should().Be("broker1:9092,broker2:9092");
        appConfig.Kafka.Client.SecurityProtocol.Should().Be(SecurityProtocol.SaslSsl);
        appConfig.Kafka.Client.SaslMechanism.Should().Be(SaslMechanism.ScramSha512);
        appConfig.Kafka.Consumer.GroupId.Should().Be("my-group");
        appConfig.Kafka.Consumer.SessionTimeoutMs.Should().Be(45000);
        appConfig.Kafka.Producer.LingerMs.Should().Be(5);
        appConfig.Kafka.Producer.CompressionType.Should().Be(CompressionType.Zstd);

        var consumerConfig = KafkaExtensions.CreateConsumerConfig(appConfig.Kafka);
        consumerConfig.BootstrapServers.Should().Be("broker1:9092,broker2:9092");
        consumerConfig.GroupId.Should().Be("my-group");
    }
}
