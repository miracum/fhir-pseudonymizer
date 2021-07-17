using System.Linq;
using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Visitors;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class ElementNodeVisitorExtensions
    {
        public static void Accept(this ElementNode node, AbstractElementNodeVisitor visitor)
        {
            var shouldVisitChild = visitor.Visit(node);

            if (shouldVisitChild)
            {
                foreach (var child in node.Children().CastElementNodes())
                {
                    child.Accept(visitor);
                }
            }

            visitor.EndVisit(node);
        }
    }
}
