using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class KeepProcessor : IAnonymizerProcessor
    {
        public Task<ProcessResult> ProcessAsync(
            PocoNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            return Task.FromResult(new ProcessResult());
        }
    }
}
