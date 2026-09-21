using BenchmarkDotNet.Attributes;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Ad-hoc benchmark (not wired into CI) isolating AnonymizationVisitor.GetRulesByType, called
///     once per FHIR resource node visited (every Bundle entry, every contained resource). Builds
///     a ~150-rule set spread over ~15 resource types (roughly matching
///     FhirPseudonymizer.Benchmarks/complex-anonymization.yaml and the resource-type distribution
///     of the Ashleigh_Olson fixture bundle it's benchmarked against), then repeatedly asks for
///     the rules matching each of those types - as the visitor does once per resource in a Bundle.
///     "Old" re-runs the LINQ Where() filter over the full rule array every call; "New" reproduces
///     the per-type memoization it was replaced with.
/// </summary>
[MemoryDiagnoser]
public class RuleFilteringBenchmarks
{
    private const int CallsPerType = 20; // ~286 resource nodes / ~15 distinct types in the fixture bundle

    private static readonly string[] ResourceTypes =
    [
        "Observation",
        "DiagnosticReport",
        "Procedure",
        "Claim",
        "ExplanationOfBenefit",
        "Immunization",
        "Encounter",
        "DocumentReference",
        "Condition",
        "MedicationRequest",
        "CareTeam",
        "CarePlan",
        "Patient",
        "ImagingStudy",
        "Provenance",
    ];

    private AnonymizationFhirPathRule[] rules = null!;
    private readonly Dictionary<string, AnonymizationFhirPathRule[]> rulesByTypeCache = new();

    [GlobalSetup]
    public void Setup()
    {
        var built = new List<AnonymizationFhirPathRule>();

        // A handful of general rules (no/blank resource type, or "Resource"/"DomainResource"),
        // matching every resource, plus ~10 resource-specific rules per type - proportioned like
        // complex-anonymization.yaml.
        for (var i = 0; i < 10; i++)
        {
            built.Add(
                AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule(
                    new Dictionary<string, object>
                    {
                        ["path"] = $"Resource.meta.tag[{i}]",
                        ["method"] = "keep",
                    }
                )
            );
        }

        foreach (var resourceType in ResourceTypes)
        {
            for (var i = 0; i < 10; i++)
            {
                built.Add(
                    AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule(
                        new Dictionary<string, object>
                        {
                            ["path"] = $"{resourceType}.field{i}",
                            ["method"] = "redact",
                        }
                    )
                );
            }
        }

        rules = [.. built];
    }

    [Benchmark(Baseline = true)]
    public int Old()
    {
        var matched = 0;

        foreach (var resourceType in ResourceTypes)
        {
            for (var call = 0; call < CallsPerType; call++)
            {
                var rulesForType = rules.Where(r =>
                    r.ResourceType.Equals(resourceType)
                    || string.IsNullOrEmpty(r.ResourceType)
                    || string.Equals("Resource", r.ResourceType)
                    || string.Equals("DomainResource", r.ResourceType)
                );

                foreach (var _ in rulesForType)
                {
                    matched++;
                }
            }
        }

        return matched;
    }

    [Benchmark]
    public int New()
    {
        rulesByTypeCache.Clear();
        var matched = 0;

        foreach (var resourceType in ResourceTypes)
        {
            for (var call = 0; call < CallsPerType; call++)
            {
                if (!rulesByTypeCache.TryGetValue(resourceType, out var rulesForType))
                {
                    rulesForType =
                    [
                        .. rules.Where(r =>
                            r.ResourceType.Equals(resourceType)
                            || string.IsNullOrEmpty(r.ResourceType)
                            || string.Equals("Resource", r.ResourceType)
                            || string.Equals("DomainResource", r.ResourceType)
                        ),
                    ];
                    rulesByTypeCache[resourceType] = rulesForType;
                }

                matched += rulesForType.Length;
            }
        }

        return matched;
    }
}
