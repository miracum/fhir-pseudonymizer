using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public interface IAnonymizerProcessor
    {
        public Task<ProcessResult> ProcessAsync(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        );
    }
}
