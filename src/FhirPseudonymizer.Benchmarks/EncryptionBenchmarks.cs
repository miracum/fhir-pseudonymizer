using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Benchmarks;

/// <summary>
///     The <c>encrypt</c> method's AES-CBC round trip as EncryptProcessor/DecryptProcessor call
///     it, for a value the size of a typical identifier.
/// </summary>
[MemoryDiagnoser]
public class EncryptionBenchmarks
{
    private byte[] key = [];
    private string plaintext = string.Empty;
    private string ciphertext = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        key = RandomNumberGenerator.GetBytes(32);
        plaintext = Guid.NewGuid().ToString();
        ciphertext = EncryptUtility.EncryptTextToHexWithAes(plaintext, key);
    }

    [Benchmark]
    public string Encrypt() => EncryptUtility.EncryptTextToHexWithAes(plaintext, key);

    [Benchmark]
    public string Decrypt() => EncryptUtility.DecryptTextFromHexStringWithAes(ciphertext, key);
}
