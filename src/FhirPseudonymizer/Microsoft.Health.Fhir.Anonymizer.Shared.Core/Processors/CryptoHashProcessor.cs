using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly Func<string, string, string> _hashFunction;
        private readonly Func<string, string> _cryptoHashFunction;
        private readonly string _cryptoHashKey;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<CryptoHashProcessor>();

        public CryptoHashProcessor(
            string cryptoHashKey,
            CryptoHashAlgorithm algorithm = CryptoHashAlgorithm.HmacSha256
        )
        {
            _cryptoHashKey = cryptoHashKey;
            _hashFunction = algorithm switch
            {
                CryptoHashAlgorithm.Blake3 => CryptoHashUtility.ComputeKeyedBlake3Hash,
                _ => CryptoHashUtility.ComputeHmacSHA256Hash,
            };
            _cryptoHashFunction = input => _hashFunction(input, _cryptoHashKey);
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
                cryptoHashFunction = (input) =>
                {
                    var fullHash = _hashFunction(input, _cryptoHashKey);
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
