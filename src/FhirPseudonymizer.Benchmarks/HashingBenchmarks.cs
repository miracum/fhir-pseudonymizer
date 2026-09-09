using BenchmarkDotNet.Attributes;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     Compares CryptoHashUtility's two cryptoHash algorithms directly - the actual production
///     code path (including hex encoding), not the raw hashing primitives - at a size
///     representative of what this app actually hashes (identifiers/references, not large
///     payloads). Mirrors how CryptoHashProcessor actually calls these: both keys are prepared
///     once upfront and reused, not re-derived or re-encoded per value.
/// </summary>
[MemoryDiagnoser]
public class HashingBenchmarks
{
    private const string Key = "fhir-pseudonymizer-benchmark-key";
    private string input = string.Empty;
    private byte[] blake3Key = [];
    private byte[] hmacKey = [];

    [GlobalSetup]
    public void Setup()
    {
        input = Guid.NewGuid().ToString();
        blake3Key = CryptoHashUtility.DeriveBlake3Key(Key);
        hmacKey = CryptoHashUtility.GetHmacSha256KeyBytes(Key);
    }

    [Benchmark(Baseline = true)]
    public string HmacSha256() => CryptoHashUtility.ComputeHmacSHA256Hash(input, hmacKey);

    [Benchmark]
    public string Blake3() => CryptoHashUtility.ComputeKeyedBlake3Hash(input, blake3Key);
}
