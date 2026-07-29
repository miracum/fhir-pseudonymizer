namespace FhirPseudonymizer.Tests;

public class StartupTests
{
    /// <summary>
    ///     Every request is served with the server's own config unless it names a Project, so a
    ///     deployment holding no config has no rules to fall back on. Refusing at startup keeps
    ///     that from surfacing as a failure on the first resource, long after the deploy.
    /// </summary>
    [Fact]
    public void Startup_WithoutAnyAnonymizationConfig_ShouldFailFast()
    {
        using var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["AnonymizationEngineConfigPath"] = "",
                ["AnonymizationEngineConfigInline"] = "",
                // a per-test factory would otherwise race the shared one for the metrics port
                ["EnableMetrics"] = "false",
            },
        };

        var start = () => factory.CreateClient();

        start
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "*Anonymization config not set*",
                "an operator has to be told which setting to fix, not handed a dependency "
                    + "resolution error"
            );
    }
}
