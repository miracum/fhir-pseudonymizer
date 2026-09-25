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

        /// <summary>
        ///     All descendants of <paramref name="node" /> in document order, not descending into
        ///     sub-resources (Bundle entries, contained resources). Collected in a single recursive
        ///     pass into a list rather than yielded by a recursive iterator, which would allocate
        ///     another iterator per node on top of what enumerating each node's children already
        ///     costs.
        /// </summary>
        public static List<PocoNode> ResourceDescendantsWithoutSubResource(this PocoNode node)
        {
            var descendants = new List<PocoNode>();
            AddResourceDescendantsWithoutSubResource(node, descendants);
            return descendants;
        }

        public static List<PocoNode> SelfAndDescendantsWithoutSubResource(
            this IEnumerable<PocoNode> nodes
        )
        {
            var selfAndDescendants = new List<PocoNode>();
            foreach (var node in nodes)
            {
                selfAndDescendants.Add(node);
                AddResourceDescendantsWithoutSubResource(node, selfAndDescendants);
            }

            return selfAndDescendants;
        }

        /// <summary>
        ///     Adds the children of <paramref name="node" /> to <paramref name="children" />, in
        ///     document order. Equivalent to <c>node.Children().CastPocoNodes()</c>, minus the
        ///     single-element list and enumerator that enumerating a singular child as a
        ///     collection would allocate.
        /// </summary>
        public static void AddChildren(this PocoNode node, List<PocoNode> children)
        {
            foreach (var childOrList in node.Children())
            {
                if (childOrList is PocoNode child)
                {
                    children.Add(child);
                }
                else
                {
                    children.AddRange(childOrList);
                }
            }
        }

        private static void AddResourceDescendantsWithoutSubResource(
            PocoNode node,
            List<PocoNode> descendants
        )
        {
            foreach (var childOrList in node.Children())
            {
                if (childOrList is PocoNode child)
                {
                    AddUnlessSubResource(child, descendants);
                }
                else
                {
                    foreach (var item in childOrList)
                    {
                        AddUnlessSubResource(item, descendants);
                    }
                }
            }
        }

        private static void AddUnlessSubResource(PocoNode child, List<PocoNode> descendants)
        {
            // Skip sub resources in bundle entry and contained list
            if (child.IsFhirResource())
            {
                return;
            }

            descendants.Add(child);
            AddResourceDescendantsWithoutSubResource(child, descendants);
        }
    }
}
