using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Visitors;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class ElementNodeVisitorExtensions
    {
        public static async Task AcceptAsync(
            this ElementNode node,
            AbstractElementNodeVisitor visitor
        )
        {
            var shouldVisitChild = await visitor.VisitAsync(node);

            if (shouldVisitChild)
            {
                foreach (var child in node.Children().CastElementNodes())
                {
                    await child.AcceptAsync(visitor);
                }
            }

            await visitor.EndVisitAsync(node);
        }
    }
}
