using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace FhirPseudonymizer
{
    public class DecryptProcessor : IAnonymizerProcessor
    {
        private readonly byte[] _key;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<EncryptProcessor>();

        public DecryptProcessor(string decryptKey)
        {
            _key = Encoding.UTF8.GetBytes(decryptKey);
        }

        public DecryptProcessor(byte[] decryptKey)
        {
            _key = decryptKey;
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

            var input = node.GetValue().ToString();
            try
            {
                node.SetPrimitiveValue(EncryptUtility.DecryptTextFromHexStringWithAes(input, _key));
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Decryption failed. Returning original value.");
            }

            _logger.LogDebug(
                $"Fhir value '{input}' at '{node.GetLocation()}' is decrypted to '{node.GetValue()}'."
            );

            return Task.FromResult(processResult);
        }
    }
}
