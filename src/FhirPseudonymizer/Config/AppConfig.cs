using System;
using FhirPseudonymizer.Pseudonymization;

namespace FhirPseudonymizer.Config;

public record AppConfig
{
    public string AnonymizationEngineConfigPath { get; init; }
    public bool UseSystemTextJsonFhirSerializer { get; init; }
    public string ApiKey { get; init; }
    public PseudonymizationServiceType PseudonymizationService { get; init; }
    public CacheConfig Cache { get; init; } = new();
    public GPasConfig GPas { get; init; } = new();
    public VfpsConfig Vfps { get; init; } = new();
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
}

public record PseudonymServiceBasicAuthConfig
{
    public string Username { get; init; }
    public string Password { get; init; }
}
