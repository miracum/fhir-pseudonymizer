using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Visitors;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class PocoNodeVisitorExtensions
    {
        public static async Task AcceptAsync(this PocoNode node, AbstractPocoNodeVisitor visitor)
        {
            var shouldVisitChild = await visitor.VisitAsync(node);

            if (shouldVisitChild)
            {
                var children = new List<PocoNode>();
                node.AddChildren(children);

                foreach (var child in children)
                {
                    await child.AcceptAsync(visitor);
                }
            }

            await visitor.EndVisitAsync(node);
        }
    }
}
