using BenchmarkDotNet.Attributes;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Ad-hoc benchmark (not wired into CI) written to measure the AnonymizationVisitor hot-path
///     changes: memoizing GetRulesByType per resource type, precomputing
///     AnonymizationFhirPathRule.MethodUpper/IsResourceTypeRule instead of recomputing them per
///     (resource, rule), and replacing MergeSettings' ImmutableArray/ToLookup/ToDictionary chain
///     with a plain dictionary copy-and-overwrite.
///
///     Uses the same large, realistic bundle (286 entries, heavily repeated resource types -
///     118 Observations etc.) and complex rule config (100+ rules) as
///     AnonymizationBenchmarks, but additionally sets AnonymizerSettings.DynamicRuleSettings so
///     MergeSettings actually runs its merge path on every single rule application instead of
///     short-circuiting.
/// </summary>
[MemoryDiagnoser]
public class VisitorHotPathBenchmarks
{
    private AnonymizerEngine engine = null!;
    private Resource largeBundle = null!;
    private AnonymizerSettings settingsWithDynamicOverrides = null!;

    [GlobalSetup]
    public void Setup()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        var configYaml = File.ReadAllText(
            Path.Join(AppContext.BaseDirectory, "complex-anonymization.yaml")
        );
        var largeBundleJson = File.ReadAllText(
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

        largeBundle = new FhirJsonParser().Parse<Resource>(largeBundleJson);

        // Forces MergeSettings to actually merge (not short-circuit) on every rule application.
        settingsWithDynamicOverrides = new AnonymizerSettings
        {
            DynamicRuleSettings = new Dictionary<string, object>
            {
                ["some-dynamic-key"] = "override",
            },
        };
    }

    [Benchmark]
    public Task<Resource> AnonymizeLargeBundleWithDynamicSettings() =>
        engine.AnonymizeResourceAsync(largeBundle, settingsWithDynamicOverrides);
}
