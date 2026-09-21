using BenchmarkDotNet.Attributes;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Ad-hoc benchmark (not wired into CI) isolating AnonymizationVisitor.MergeSettings, called
///     once per (resource, rule) pair - so tens of thousands of times for a realistic Bundle. The
///     "Old" benchmark reproduces the original ImmutableArray.Create/SelectMany/ToLookup/
///     ToDictionary implementation verbatim; "New" is the plain dictionary copy-and-overwrite it
///     was replaced with. Both take the same two three-key dictionaries, matching a typical rule's
///     settings plus a caller's dynamic overrides.
/// </summary>
[MemoryDiagnoser]
public class MergeSettingsBenchmarks
{
    private Dictionary<string, object> ruleSettings = null!;
    private Dictionary<string, object> dynamicRuleSettings = null!;

    [GlobalSetup]
    public void Setup()
    {
        ruleSettings = new Dictionary<string, object>
        {
            ["domain"] = "Patient",
            ["case"] = "upper",
            ["replaceWith"] = "REDACTED",
        };

        dynamicRuleSettings = new Dictionary<string, object>
        {
            ["domain"] = "Patient-Override",
            ["cryptoHashKey"] = "some-key",
        };
    }

    [Benchmark(Baseline = true)]
    public Dictionary<string, object> Old()
    {
        return System
            .Collections.Immutable.ImmutableArray.Create(ruleSettings, dynamicRuleSettings)
            .SelectMany(dict => dict)
            .ToLookup(pair => pair.Key, pair => pair.Value)
            .ToDictionary(group => group.Key, group => group.Last());
    }

    [Benchmark]
    public Dictionary<string, object> New()
    {
        var merged = new Dictionary<string, object>(ruleSettings);

        foreach (var (key, value) in dynamicRuleSettings)
        {
            merged[key] = value;
        }

        return merged;
    }
}
