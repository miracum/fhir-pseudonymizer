using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class EncryptProcessor : IAnonymizerProcessor
    {
        private readonly byte[] _key;

        public EncryptProcessor(string encryptKey)
        {
            _key = Encoding.UTF8.GetBytes(encryptKey);
        }

        public EncryptProcessor(byte[] encryptKey)
        {
            _key = encryptKey;
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
            node.SetPrimitiveValue(EncryptUtility.EncryptTextToHexWithAes(input, _key));

            processResult.AddProcessRecord(AnonymizationOperations.Encrypt, node);
            return Task.FromResult(processResult);
        }
    }
}
