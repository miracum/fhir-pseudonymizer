using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.ElementModel;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

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

            var input = node.Value.ToString();
            try
            {
                node.Value = EncryptUtility.DecryptTextFromHexStringWithAes(input, _key);
            }
            catch (Exception exc)
            {
                _logger.LogWarning(exc, "Decryption failed. Returning original value.");
            }

            _logger.LogDebug(
                $"Fhir value '{input}' at '{node.Location}' is decrypted to '{node.Value}'."
            );

            return processResult;
        }
    }
}
