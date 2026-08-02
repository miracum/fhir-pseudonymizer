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
                foreach (var child in node.Children().CastPocoNodes().ToList())
                {
                    await child.AcceptAsync(visitor);
                }
            }

            await visitor.EndVisitAsync(node);
        }
    }
}
