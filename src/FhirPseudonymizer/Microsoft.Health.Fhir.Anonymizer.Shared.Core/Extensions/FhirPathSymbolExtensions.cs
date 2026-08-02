using Hl7.Fhir.Model;
using Hl7.FhirPath.Expressions;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class FhirPathSymbolExtensions
    {
        private static readonly object _lock = new object();

        public static SymbolTable AddExtensionSymbols(this SymbolTable t)
        {
            // Add lock here to ensure thread safety when modifying a symbol table
            lock (_lock)
            {
                // Check whether extension method already exists
                if (t.Filter("nodesByType", 2).Count() == 0)
                {
                    // The registered delegate's focus/return type must be exactly
                    // IEnumerable<PocoNode> (FHIRPath's internal FocusCollection type in v6) -
                    // IEnumerable<ITypedElement> compiles fine here too, but at invocation time
                    // the engine's Typecasts.CastTo<FocusCollection> no longer recognizes it as
                    // "already a node sequence" and instead tries to re-infer a primitive type
                    // from each item, throwing.
                    t.Add(
                        "nodesByType",
                        (IEnumerable<PocoNode> f, string typeName) => NodesByType(f, typeName),
                        true
                    );
                }

                if (t.Filter("nodesByName", 2).Count() == 0)
                {
                    t.Add(
                        "nodesByName",
                        (IEnumerable<PocoNode> f, string name) => NodesByName(f, name),
                        true
                    );
                }
            }

            return t;
        }

        public static IEnumerable<PocoNode> NodesByType(
            IEnumerable<PocoNode> nodes,
            string typeName
        )
        {
            return nodes
                .SelfAndDescendantsWithoutSubResource()
                .Where(n => typeName.Equals(n.GetInstanceType()));
        }

        public static IEnumerable<PocoNode> NodesByName(IEnumerable<PocoNode> nodes, string name)
        {
            return nodes.SelfAndDescendantsWithoutSubResource().Where(n => name.Equals(n.Name));
        }
    }
}
