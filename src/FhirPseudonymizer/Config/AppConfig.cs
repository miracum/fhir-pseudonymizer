using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Config;

public record AppConfig
{
    public string AnonymizationEngineConfigPath { get; init; }
    public AnonymizationConfig AnonymizationConfig { get; set; }
    public bool UseSystemTextJsonFhirSerializer { get; init; }
    public string ApiKey { get; init; }
    public PseudonymizationServiceType PseudonymizationService { get; init; }
    public CacheConfig Cache { get; init; } = new();
    public GPasConfig GPas { get; init; } = new();
    public VfpsConfig Vfps { get; init; } = new();
    public ushort MetricsPort { get; set; } = 8081;
    public bool EnableMetrics { get; set; } = true;
    public FeatureManagement Features { get; set; } = new();
}

public record AnonymizationConfig
{
    public string Path { get; init; }
    public string Inline { get; init; }
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
    public CacheConfig Cache { get; init; } = new();
}

public record VfpsConfig
{
    public Uri Address { get; init; }
    public PseudonymServiceAuthConfig Auth { get; init; } = new();
    public bool UnsafeUseInsecureChannelCallCredentials { get; init; }
    public bool UseTls { get; init; }
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

    public string Scope { get; init; } = "api";

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
