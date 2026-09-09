using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Reproduces the reported "50k resources takes about a minute" case in the two shapes it can
///     take, because they stress different parts of the engine.
///     <list type="bullet">
///         <item>
///             "provenance" is one resource that holds <see cref="ResourceCount" /> Reference
///             nodes. The per-resource rule loop runs once, so this isolates node traversal and
///             hashing.
///         </item>
///         <item>
///             "bundle" is <see cref="ResourceCount" /> separate resources. The whole rule loop
///             in AnonymizationVisitor.ProcessResourceNodeAsync runs once per resource, so this
///             also exposes the per-resource fixed cost.
///         </item>
///     </list>
///     <para>
///         The <see cref="Method" /> parameter is what makes this useful: "keep" is a no-op
///         processor, so it pays the same parse, FHIRPath and ElementNode costs but does no
///         hashing. The gap between "keep" and the two cryptoHash variants is the true cost of
///         hashing, and the "keep" time itself is the floor no hash change can go below.
///     </para>
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 1, iterationCount: 3)]
public class LargeProvenanceBenchmarks
{
    private static readonly FhirJsonParser Parser = new();

    private AnonymizerEngine engine = null!;
    private string provenanceJson = string.Empty;

    [Params(50_000)]
    public int ResourceCount { get; set; }

    [Params("provenance", "bundle")]
    public string Shape { get; set; } = string.Empty;

    [Params("keep", "cryptoHash-hmacSha256", "cryptoHash-blake3")]
    public string Method { get; set; } = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();

        provenanceJson = Shape switch
        {
            "provenance" => BuildProvenanceJson(ResourceCount),
            "bundle" => BuildBundleJson(ResourceCount),
            _ => throw new ArgumentOutOfRangeException(nameof(Shape), Shape, null),
        };

        var (method, algorithm) = Method switch
        {
            "keep" => ("keep", "hmacSha256"),
            "cryptoHash-hmacSha256" => ("cryptoHash", "hmacSha256"),
            "cryptoHash-blake3" => ("cryptoHash", "blake3"),
            _ => throw new ArgumentOutOfRangeException(nameof(Method), Method, null),
        };

        var configYaml = $"""
            ---
            fhirVersion: R4
            fhirPathRules:
              - path: nodesByType('Reference').reference
                method: {method}
            parameters:
              cryptoHashKey: "fhir-pseudonymizer"
              cryptoHashAlgorithm: {algorithm}
            """;

        engine = new AnonymizerEngine(
            AnonymizerConfigurationManager.CreateFromYamlConfigString(configYaml)
        );
    }

    /// <summary>
    ///     Parse plus serialize with no anonymization at all. This is the cost the engine can
    ///     never avoid, so subtract it from the other results to isolate the anonymization work.
    /// </summary>
    [Benchmark(Baseline = true)]
    public string ParseAndSerializeOnly()
    {
        return Parser.Parse<Hl7.Fhir.Model.Resource>(provenanceJson).ToJson();
    }

    [Benchmark]
    public Task<string> DeIdentify()
    {
        return engine.AnonymizeJsonAsync(provenanceJson);
    }

    private static string BuildProvenanceJson(int targetCount)
    {
        var builder = new StringBuilder(targetCount * 64);
        builder.Append(
            """{"resourceType":"Provenance","id":"large","recorded":"2020-01-01T00:00:00Z","agent":[{"who":{"reference":"Practitioner/agent-1"}}],"target":["""
        );

        for (var i = 0; i < targetCount; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            builder.Append("{\"reference\":\"Observation/obs-").Append(i).Append("\"}");
        }

        builder.Append("]}");
        return builder.ToString();
    }

    private static string BuildBundleJson(int resourceCount)
    {
        var builder = new StringBuilder(resourceCount * 128);
        builder.Append("""{"resourceType":"Bundle","id":"large","type":"collection","entry":[""");

        for (var i = 0; i < resourceCount; i++)
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            builder
                .Append("{\"resource\":{\"resourceType\":\"Observation\",\"id\":\"obs-")
                .Append(i)
                .Append(
                    "\",\"status\":\"final\",\"code\":{\"coding\":[{\"system\":\"http://loinc.org\",\"code\":\"1234-5\"}]},\"subject\":{\"reference\":\"Patient/pat-"
                )
                .Append(i)
                .Append("\"}}}");
        }

        builder.Append("]}");
        return builder.ToString();
    }
}
