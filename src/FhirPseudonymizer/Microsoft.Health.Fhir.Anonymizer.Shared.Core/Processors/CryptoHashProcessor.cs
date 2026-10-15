using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly Func<string, string> _cryptoHashFunction;

        public CryptoHashProcessor(
            string cryptoHashKey,
            CryptoHashAlgorithm algorithm = CryptoHashAlgorithm.HmacSha256
        )
        {
            _cryptoHashFunction =
                algorithm == CryptoHashAlgorithm.Blake3
                    ? CreateBlake3HashFunction(cryptoHashKey)
                    : CreateHmacSha256HashFunction(cryptoHashKey);
        }

        private static Func<string, string> CreateHmacSha256HashFunction(string cryptoHashKey)
        {
            var keyBytes = CryptoHashUtility.GetHmacSha256KeyBytes(cryptoHashKey);
            return input => CryptoHashUtility.ComputeHmacSHA256Hash(input, keyBytes);
        }

        private static Func<string, string> CreateBlake3HashFunction(string cryptoHashKey)
        {
            // Derived once here and reused for every value this processor hashes, instead of
            // re-deriving the same 32-byte key from cryptoHashKey on every single call.
            var derivedKey = CryptoHashUtility.DeriveBlake3Key(cryptoHashKey);
            return input => CryptoHashUtility.ComputeKeyedBlake3Hash(input, derivedKey);
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

            var cryptoHashFunction = _cryptoHashFunction;

            if (
                settings?.TryGetValue("truncateToMaxLength", out var truncateToMaxLengthObject)
                == true
            )
            {
                var truncateToMaxLength = Convert.ToInt32(truncateToMaxLengthObject);
                var baseHashFunction = _cryptoHashFunction;
                cryptoHashFunction = (input) =>
                {
                    var fullHash = baseHashFunction(input);
                    return fullHash[..Math.Min(truncateToMaxLength, fullHash.Length)];
                };
            }

            var input = node.GetValue().ToString();
            // Hash the id part for "reference" and "uri" nodes and hash whole input for other node types
            if (node.IsReferenceStringNode() || node.IsReferenceUriNode(input))
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
