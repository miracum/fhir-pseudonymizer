using BenchmarkDotNet.Attributes;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Compares CryptoHashUtility's two cryptoHash algorithms directly - the actual production
///     code path (including hex encoding and, for Blake3, its key-derivation step), not the raw
///     hashing primitives - at a size representative of what this app actually hashes
///     (identifiers/references, not large payloads).
/// </summary>
[MemoryDiagnoser]
public class HashingBenchmarks
{
    private const string Key = "fhir-pseudonymizer-benchmark-key";
    private string input = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        input = Guid.NewGuid().ToString();
    }

    [Benchmark(Baseline = true)]
    public string HmacSha256() => CryptoHashUtility.ComputeHmacSHA256Hash(input, Key);

    [Benchmark]
    public string Blake3() => CryptoHashUtility.ComputeKeyedBlake3Hash(input, Key);
}
