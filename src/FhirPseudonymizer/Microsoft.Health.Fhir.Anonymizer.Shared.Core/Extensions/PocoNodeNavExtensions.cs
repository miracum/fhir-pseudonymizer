using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class PocoNodeNavExtensions
    {
        public static List<PocoNode> GetEntryResourceChildren(this PocoNode node)
        {
            return node
                ?.ChildrenByName(Constants.EntryNodeName)
                .Select(entry =>
                    entry?.ChildrenByName(Constants.EntryResourceNodeName).FirstOrDefault()
                )
                .Where(resource => resource != null)
                .ToList();
        }

        public static List<PocoNode> GetContainedChildren(this PocoNode node)
        {
            return node?.ChildrenByName(Constants.ContainedNodeName).ToList();
        }

        public static IEnumerable<PocoNode> ResourceDescendantsWithoutSubResource(
            this PocoNode node
        )
        {
            foreach (var child in node.Children().CastPocoNodes())
            {
                // Skip sub resources in bundle entry and contained list
                if (child.IsFhirResource())
                {
                    continue;
                }

                yield return child;

                foreach (var n in child.ResourceDescendantsWithoutSubResource())
                {
                    yield return n;
                }
            }
        }

        public static IEnumerable<PocoNode> SelfAndDescendantsWithoutSubResource(
            this IEnumerable<PocoNode> nodes
        )
        {
            foreach (var node in nodes)
            {
                yield return node;

                foreach (var descendant in node.ResourceDescendantsWithoutSubResource())
                {
                    yield return descendant;
                }
            }
        }
    }
}
