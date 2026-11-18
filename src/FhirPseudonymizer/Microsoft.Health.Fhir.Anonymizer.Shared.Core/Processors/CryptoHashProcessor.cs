using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly Func<string, string> _cryptoHashFunction;
        private readonly Func<string, string> _uuidHashFunction;

        public CryptoHashProcessor(
            string cryptoHashKey,
            CryptoHashAlgorithm algorithm = CryptoHashAlgorithm.HmacSha256
        )
        {
            if (algorithm == CryptoHashAlgorithm.Blake3)
            {
                // Derived once here and reused for every value this processor hashes, instead of
                // re-deriving the same 32-byte key from cryptoHashKey on every single call.
                var derivedKey = CryptoHashUtility.DeriveBlake3Key(cryptoHashKey);
                _cryptoHashFunction = input =>
                    CryptoHashUtility.ComputeKeyedBlake3Hash(input, derivedKey);
                _uuidHashFunction = input =>
                {
                    Span<byte> hash = stackalloc byte[Blake3.Hash.Size];
                    CryptoHashUtility.ComputeKeyedBlake3Hash(input, derivedKey, hash);
                    return CryptoHashUtility.ToUuid(hash);
                };
            }
            else
            {
                var keyBytes = CryptoHashUtility.GetHmacSha256KeyBytes(cryptoHashKey);
                _cryptoHashFunction = input =>
                    CryptoHashUtility.ComputeHmacSHA256Hash(input, keyBytes);
                _uuidHashFunction = input =>
                {
                    Span<byte> hash =
                        stackalloc byte[System.Security.Cryptography.HMACSHA256.HashSizeInBytes];
                    CryptoHashUtility.ComputeHmacSHA256Hash(input, keyBytes, hash);
                    return CryptoHashUtility.ToUuid(hash);
                };
            }
        }

        public Task<ProcessResult> ProcessAsync(
            PocoNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.GetValue()?.ToString()))
            {
                return Task.FromResult(processResult);
            }

            var cryptoHashFunction =
                CryptoHashSetting.GetOutputFormat(settings) == CryptoHashOutputFormat.Uuid
                    ? _uuidHashFunction
                    : _cryptoHashFunction;

            if (
                settings?.TryGetValue(
                    RuleKeys.TruncateToMaxLength,
                    out var truncateToMaxLengthObject
                ) == true
            )
            {
                var truncateToMaxLength = Convert.ToInt32(truncateToMaxLengthObject);
                var baseHashFunction = cryptoHashFunction;
                cryptoHashFunction = (input) =>
                {
                    var fullHash = baseHashFunction(input);
                    return fullHash[..Math.Min(truncateToMaxLength, fullHash.Length)];
                };
            }

            var transformWholeUri = settings.IsTransformWholeUriEnabled();

            var input = node.GetValue().ToString();
            // Hash the id part for "reference" and "uri" nodes and hash whole input for other node types.
            // With transformWholeUri, "uri" nodes that merely look like a reference (e.g. an identifier
            // system such as http://example.com/fhir/NamingSystem/test) are hashed as a whole, too.
            // Reference.reference nodes are real references, so they keep getting only their id hashed.
            if (
                node.IsReferenceStringNode()
                || (!transformWholeUri && node.IsReferenceUriNode(input))
            )
            {
                var newReference = ReferenceUtility.TransformReferenceId(input, cryptoHashFunction);
                node.SetPrimitiveValue(newReference);
            }
            else
            {
                node.SetPrimitiveValue(cryptoHashFunction(input));
            }

            processResult.AddProcessRecord(AnonymizationOperations.CryptoHash, node);
            return Task.FromResult(processResult);
        }
    }
}
