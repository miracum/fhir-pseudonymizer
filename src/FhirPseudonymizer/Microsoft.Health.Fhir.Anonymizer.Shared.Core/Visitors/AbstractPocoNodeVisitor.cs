using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Visitors
{
    public abstract class AbstractPocoNodeVisitor
    {
        public virtual Task<bool> VisitAsync(PocoNode node)
        {
            return Task.FromResult(true);
        }

        public virtual Task EndVisitAsync(PocoNode node)
        {
            return Task.CompletedTask;
        }
    }
}
