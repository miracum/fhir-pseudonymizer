using Confluent.Kafka;
using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Config;

public record AppConfig
{
    public string AnonymizationEngineConfigPath { get; init; }
    public string AnonymizationEngineConfigInline { get; set; }
    public bool UseSystemTextJsonFhirSerializer { get; init; }
    public string ApiKey { get; init; }
    public PseudonymizationServiceType PseudonymizationService { get; init; }
    public CacheConfig Cache { get; init; } = new();
    public GPasConfig GPas { get; init; } = new();
    public VfpsConfig Vfps { get; init; } = new();
    public EnticiConfig Entici { get; init; } = new();
    public MiiConfig Mii { get; init; } = new();
    public ushort MetricsPort { get; set; } = 8081;
    public bool EnableMetrics { get; set; } = true;
    public FeatureManagement Features { get; set; } = new();
    public AnonymizationConfig Anonymization { get; set; } = new();
    public KafkaConfig Kafka { get; init; } = new();
}

public record KafkaConfig
{
    public List<string> Topics { get; init; } = [];

    /// <summary>
    ///     A regular expression matched against the input topic name to derive the output topic
    ///     name it gets replaced with, e.g. matching "^fhir\." and replacing it with
    ///     "fhir.pseudonymized." turns the input topic "fhir.test" into the output topic
    ///     "fhir.pseudonymized.test". The default simply prepends "pseudonymized." to every topic.
    /// </summary>
    public string OutputTopicPattern { get; init; } = "^";
    public string OutputTopicReplacement { get; init; } = "pseudonymized.";
    public int WorkerCount { get; init; } = Environment.ProcessorCount;
    public int WorkerChannelCapacity { get; init; } = 100;

    /// <summary>
    ///     The topic that FHIR Provenance resources documenting the pseudonymization of a message
    ///     are produced to, one Provenance per pseudonymized resource, bundled into a single
    ///     "collection" Bundle per input message. Left unset (the default), no provenance records
    ///     are produced.
    /// </summary>
    public string ProvenanceTopic { get; init; }

    /// <summary>
    ///     Settings shared between the consumer and producer client (e.g. BootstrapServers,
    ///     SecurityProtocol, Sasl*). Bound from the "Kafka:Client" config section using the
    ///     property names of <see cref="ClientConfig" /> directly, so any
    ///     librdkafka client setting can be set without adding a dedicated property here.
    /// </summary>
    public ClientConfig Client { get; init; } = new();

    /// <summary>
    ///     Consumer-only overrides/additions on top of <see cref="Client" />. Bound from the
    ///     "Kafka:Consumer" section using <see cref="ConsumerConfig" /> property
    ///     names, e.g. "Kafka__Consumer__SessionTimeoutMs".
    /// </summary>
    public ConsumerConfig Consumer { get; init; } = new();

    /// <summary>
    ///     Producer-only overrides/additions on top of <see cref="Client" />. Bound from the
    ///     "Kafka:Producer" section using <see cref="ProducerConfig" /> property
    ///     names, e.g. "Kafka__Producer__LingerMs".
    /// </summary>
    public ProducerConfig Producer { get; init; } = new();
}

public record CacheConfig
{
    public uint SizeLimit { get; init; }
    public uint SlidingExpirationMinutes { get; init; }
    public uint AbsoluteExpirationMinutes { get; init; }
}

public record GPasConfig
{
    public Uri Url { get; init; }
    public int RequestRetryCount { get; init; }
    public string Version { get; init; }
    public PseudonymServiceAuthConfig Auth { get; init; } = new();
}

public record EnticiConfig
{
    public Uri Url { get; init; }
    public PseudonymServiceAuthConfig Auth { get; init; } = new();
    public int RequestRetryCount { get; init; }
}

public record VfpsConfig
{
    public Uri Address { get; init; }
    public PseudonymServiceAuthConfig Auth { get; init; } = new();
    public bool UnsafeUseInsecureChannelCallCredentials { get; init; }
    public bool UseTls { get; init; }
}

public record MiiConfig
{
    public Uri Url { get; init; }
    public PseudonymServiceAuthConfig Auth { get; init; } = new();
    public int RequestRetryCount { get; init; }
}

public record PseudonymServiceAuthConfig
{
    public PseudonymServiceBasicAuthConfig Basic { get; init; } = new();
    public PseudonymServiceOAuthConfig OAuth { get; init; } = new();
}

public record PseudonymServiceOAuthConfig
{
    public Uri TokenEndpoint { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string Scope { get; init; }

    public string Resource { get; init; }
}

public record PseudonymServiceBasicAuthConfig
{
    public string Username { get; init; }
    public string Password { get; init; }
}

public record FeatureManagement
{
    public bool ConditionalReferencePseudonymization { get; init; }
}

public record AnonymizationConfig
{
    public string CryptoHashKey { get; set; }
    public string EncryptKey { get; set; }
    public bool ShouldAddSecurityTag { get; set; } = true;
}
