using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    /// <summary>
    ///     Answers the nodesByType()/nodesByName() FHIRPath functions for a single resource from
    ///     one walk of it, instead of walking the whole resource again for every rule using them.
    ///     Each walk allocates a fresh node wrapper and child enumerators for every node visited, so
    ///     with e.g. the HIPAA sample config's 25 nodesByType rules, repeating it per rule was most
    ///     of the memory allocated per resource.
    ///     <para>
    ///         The index reflects the resource's structure at the time of the walk. Changing
    ///         primitive values in place (what most anonymization methods do) leaves it valid, but
    ///         <see cref="Invalidate" /> must be called after nodes were added or removed (the
    ///         substitute and remove methods), so that the next lookup walks the resource again.
    ///     </para>
    /// </summary>
    public sealed class ResourceNodeIndex
    {
        // The index the FHIRPath functions evaluated on the current thread should use, if any.
        // Scoped to a synchronous FHIRPath evaluation (see Use), so it never leaks across an await.
        [ThreadStatic]
        private static ResourceNodeIndex t_current;

        private readonly PocoNode _resource;
        private List<PocoNode> _selfAndDescendants;
        private Dictionary<string, List<PocoNode>> _byType;
        private Dictionary<string, List<PocoNode>> _byName;

        public ResourceNodeIndex(PocoNode resource)
        {
            _resource = resource;
        }

        /// <summary>
        ///     Makes nodesByType()/nodesByName() called on this index's resource use it, until the
        ///     returned scope is disposed. Must only span synchronous code, e.g. evaluating and
        ///     materializing a single FHIRPath expression.
        /// </summary>
        public Scope Use()
        {
            var previous = t_current;
            t_current = this;
            return new Scope(previous);
        }

        public void Invalidate()
        {
            _selfAndDescendants = null;
            _byType = null;
            _byName = null;
        }

        /// <summary>
        ///     The index in use on the current thread, if <paramref name="focus" /> is exactly its
        ///     resource - i.e. the function was called on the resource itself (e.g.
        ///     <c>nodesByType('HumanName')</c>), rather than on some part of it.
        /// </summary>
        public static bool TryGetFor(IEnumerable<PocoNode> focus, out ResourceNodeIndex index)
        {
            index = t_current;
            return index is not null
                && GetSingle(focus) is { } node
                && ReferenceEquals(node.Poco, index._resource.Poco);
        }

        public IReadOnlyList<PocoNode> NodesByType(string typeName)
        {
            _byType ??= GroupSelfAndDescendants(node => node.GetInstanceType());
            return _byType.GetValueOrDefault(typeName) ?? [];
        }

        public IReadOnlyList<PocoNode> NodesByName(string name)
        {
            _byName ??= GroupSelfAndDescendants(node => node.Name);
            return _byName.GetValueOrDefault(name) ?? [];
        }

        private Dictionary<string, List<PocoNode>> GroupSelfAndDescendants(
            Func<PocoNode, string> keySelector
        )
        {
            _selfAndDescendants ??= new[] { _resource }.SelfAndDescendantsWithoutSubResource();

            var groups = new Dictionary<string, List<PocoNode>>();
            foreach (var node in _selfAndDescendants)
            {
                var key = keySelector(node);
                if (key is null)
                {
                    continue;
                }

                if (!groups.TryGetValue(key, out var group))
                {
                    groups[key] = group = [];
                }

                group.Add(node);
            }

            return groups;
        }

        private static PocoNode GetSingle(IEnumerable<PocoNode> nodes)
        {
            if (nodes is PocoNode node)
            {
                return node;
            }

            using var enumerator = nodes.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }

            var single = enumerator.Current;
            return enumerator.MoveNext() ? null : single;
        }

        public readonly struct Scope(ResourceNodeIndex previous) : IDisposable
        {
            public void Dispose()
            {
                t_current = previous;
            }
        }
    }
}
