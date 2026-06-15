using Hl7.Fhir.ElementModel;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Visitors
{
    public abstract class AbstractElementNodeVisitor
    {
        public virtual Task<bool> VisitAsync(ElementNode node)
        {
            return Task.FromResult(true);
        }

        public virtual Task EndVisitAsync(ElementNode node)
        {
            return Task.CompletedTask;
        }
    }
}
