using Confluent.Kafka;
using FhirPseudonymizer.Config;

namespace FhirPseudonymizer.Kafka;

public static class KafkaExtensions
{
    public const string DefaultGroupId = "fhir-pseudonymizer";

    /// <summary>
    ///     Topics are normally bound from an array (e.g. "Kafka:Topics:0", "Kafka:Topics:1" /
    ///     a JSON array), which is awkward to set via a single environment variable. As a
    ///     convenience, this also allows "Kafka:Topics" to be set to a single comma-separated
    ///     string (e.g. "Kafka__Topics=topic-a,topic-b") whenever no array was bound.
    /// </summary>
    public static List<string> NormalizeTopics(
        IConfiguration configuration,
        List<string> boundTopics
    )
    {
        if (boundTopics.Count > 0)
        {
            return boundTopics;
        }

        var raw = configuration["Kafka:Topics"];
        if (string.IsNullOrWhiteSpace(raw))
        {
            return boundTopics;
        }

        return
        [
            .. raw.Split(
                ',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
            ),
        ];
    }

    public static IServiceCollection AddKafkaConsumer(
        this IServiceCollection services,
        KafkaConfig kafkaConfig
    )
    {
        services.AddSingleton(_ =>
            new ConsumerBuilder<byte[], string>(CreateConsumerConfig(kafkaConfig)).Build()
        );

        services.AddHostedService<KafkaConsumerService>();

        return services;
    }

    /// <summary>
    ///     Registers the shared <see cref="IProducer{TKey,TValue}" /> used both by
    ///     <see cref="KafkaConsumerService" /> to publish anonymized messages, and by
    ///     <see cref="KafkaProvenancePublisher" /> to publish provenance bundles - the latter of
    ///     which can be needed even when reading from Kafka (<see cref="AddKafkaConsumer" />) is
    ///     not enabled, e.g. when only pseudonymizing over the REST API.
    /// </summary>
    public static IServiceCollection AddKafkaProducer(
        this IServiceCollection services,
        KafkaConfig kafkaConfig
    )
    {
        services.AddSingleton(_ =>
            new ProducerBuilder<byte[], string>(CreateProducerConfig(kafkaConfig)).Build()
        );

        return services;
    }

    /// <summary>
    ///     Merges the shared <see cref="KafkaConfig.Client" /> settings with the consumer-only
    ///     <see cref="KafkaConfig.Consumer" /> overrides, applying the sane defaults and the
    ///     non-negotiable offset-storing settings that <see cref="KafkaConsumerService" /> relies on.
    /// </summary>
    public static ConsumerConfig CreateConsumerConfig(KafkaConfig kafkaConfig)
    {
        // start from a copy of the settings shared with the producer (BootstrapServers,
        // SecurityProtocol, Sasl*, ...). ConsumerConfig(ClientConfig) does not clone the
        // underlying dictionary, so without copying it first, the consumer-only settings set
        // below would leak into kafkaConfig.Client and from there into the producer's config too.
        var consumerConfig = new ConsumerConfig(new Dictionary<string, string>(kafkaConfig.Client));

        // sane defaults, overridable via Kafka__Consumer__* settings
        consumerConfig.GroupId ??= DefaultGroupId;
        consumerConfig.AutoOffsetReset ??= AutoOffsetReset.Earliest;
        // makes rebalances incremental instead of stop-the-world, which matters once many
        // partitions (topics * partitions-per-topic) are assigned to a consumer instance.
        consumerConfig.PartitionAssignmentStrategy ??=
            PartitionAssignmentStrategy.CooperativeSticky;

        // layer the explicitly configured Kafka__Consumer__* settings on top of the above
        foreach (var (key, value) in kafkaConfig.Consumer)
        {
            consumerConfig.Set(key, value);
        }

        // offsets are stored manually (via IConsumer.StoreOffset) only after a message has been
        // anonymized and produced to its output topic, but are still committed to the broker
        // periodically in the background instead of blocking on every message. KafkaConsumerService's
        // correctness depends on this, so it is not overridable via Kafka__Consumer__*.
        consumerConfig.EnableAutoCommit = true;
        consumerConfig.EnableAutoOffsetStore = false;

        return consumerConfig;
    }

    /// <summary>
    ///     Merges the shared <see cref="KafkaConfig.Client" /> settings with the producer-only
    ///     <see cref="KafkaConfig.Producer" /> overrides.
    /// </summary>
    public static ProducerConfig CreateProducerConfig(KafkaConfig kafkaConfig)
    {
        // see CreateConsumerConfig for why this needs to be a copy of the dictionary
        var producerConfig = new ProducerConfig(new Dictionary<string, string>(kafkaConfig.Client));

        foreach (var (key, value) in kafkaConfig.Producer)
        {
            producerConfig.Set(key, value);
        }

        return producerConfig;
    }
}
