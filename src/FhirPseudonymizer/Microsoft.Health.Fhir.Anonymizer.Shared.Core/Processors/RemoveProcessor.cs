using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    // Unlike Redact - which only ever clears scalar leaf values and relies on a later cleanup pass
    // to prune composites that end up fully empty, and which never touches a nested FHIR resource -
    // Remove deletes the matched node outright from its parent, taking anything nested within it
    // (including a resource, e.g. Bundle.entry.resource) with it. This lets a rule such as
    // `path: Bundle.entry.where(resource is Patient)` discard whole bundle entries.
    public class RemoveProcessor : IAnonymizerProcessor
    {
        public Task<ProcessResult> ProcessAsync(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var result = new ProcessResult();
            if (node?.Parent == null)
            {
                return Task.FromResult(result);
            }

            node.Parent.Remove(node);
            result.AddProcessRecord(AnonymizationOperations.Remove, node);
            return Task.FromResult(result);
        }
    }
}
