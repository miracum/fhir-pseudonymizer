using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Models
{
    public class ProcessContext
    {
        public HashSet<PocoNode> VisitedNodes { get; set; }
    }
}
