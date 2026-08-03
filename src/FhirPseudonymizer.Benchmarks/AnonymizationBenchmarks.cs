using BenchmarkDotNet.Attributes;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Runs full de-identification (parse + anonymize + serialize) using a "complex" config that
///     combines redact, cryptoHash, dateshift, perturb, encrypt and (mocked) pseudonymize rules,
///     against a large, realistic FHIR bundle taken from the snapshot test fixtures.
/// </summary>
[MemoryDiagnoser]
public class AnonymizationBenchmarks
{
    private AnonymizerEngine engine = null!;
    private string largeBundleJson = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        var configYaml = File.ReadAllText(
            Path.Join(AppContext.BaseDirectory, "complex-anonymization.yaml")
        );
        largeBundleJson = File.ReadAllText(
            Path.Join(
                AppContext.BaseDirectory,
                "Ashleigh_Olson_9d9b8bed-7b79-7fa9-cea1-f133a6b4d551.json"
            )
        );

        var config = AnonymizerConfigurationManager.CreateFromYamlConfigString(configYaml);
        engine = new AnonymizerEngine(config);
        engine.AddProcessor(
            "pseudonymize",
            new PseudonymizationProcessor(new MockPseudonymServiceClient(), new FeatureManagement())
        );
    }

    [Benchmark]
    public Task<string> AnonymizeLargeBundleWithComplexConfig() =>
        engine.AnonymizeJsonAsync(largeBundleJson);
}
