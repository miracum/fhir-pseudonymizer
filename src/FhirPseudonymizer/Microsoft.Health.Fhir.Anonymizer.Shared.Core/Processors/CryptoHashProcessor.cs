using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly Func<string, string> _cryptoHashFunction;
        private readonly string _cryptoHashKey;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<CryptoHashProcessor>();

        public CryptoHashProcessor(string cryptoHashKey)
        {
            _cryptoHashKey = cryptoHashKey;
            _cryptoHashFunction = input =>
                CryptoHashUtility.ComputeHmacSHA256Hash(input, _cryptoHashKey);
        }

        public ProcessResult Process(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
            {
                return processResult;
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
                    var fullHash = CryptoHashUtility.ComputeHmacSHA256Hash(input, _cryptoHashKey);
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
            return processResult;
        }
    }
}
