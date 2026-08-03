using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly Func<string, string> _cryptoHashFunction;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<CryptoHashProcessor>();

        public CryptoHashProcessor(
            string cryptoHashKey,
            CryptoHashAlgorithm algorithm = CryptoHashAlgorithm.HmacSha256
        )
        {
            _cryptoHashFunction =
                algorithm == CryptoHashAlgorithm.Blake3
                    ? CreateBlake3HashFunction(cryptoHashKey)
                    : input => CryptoHashUtility.ComputeHmacSHA256Hash(input, cryptoHashKey);
        }

        private static Func<string, string> CreateBlake3HashFunction(string cryptoHashKey)
        {
            // Derived once here and reused for every value this processor hashes, instead of
            // re-deriving the same 32-byte key from cryptoHashKey on every single call.
            var derivedKey = CryptoHashUtility.DeriveBlake3Key(cryptoHashKey);
            return input => CryptoHashUtility.ComputeKeyedBlake3Hash(input, derivedKey);
        }

        public Task<ProcessResult> ProcessAsync(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
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

            var input = node.Value.ToString();
            // Hash the id part for "reference" and "uri" nodes and hash whole input for other node types
            if (node.IsReferenceStringNode() || node.IsReferenceUriNode(input))
            {
                var newReference = ReferenceUtility.TransformReferenceId(input, cryptoHashFunction);
                node.Value = newReference;
            }
            else
            {
                node.Value = cryptoHashFunction(input);
            }

            _logger.LogDebug(
                "Fhir value '{Input}' at '{NodeLocation}' is hashed to '{NodeValue}'.",
                input,
                node.Location,
                node.Value
            );

            processResult.AddProcessRecord(AnonymizationOperations.CryptoHash, node);
            return Task.FromResult(processResult);
        }
    }
}
