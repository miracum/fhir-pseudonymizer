using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    /// <summary>
    ///     PocoNode is a record, so its compiler-generated Equals/GetHashCode include every
    ///     field declared on it - including the private, lazily-initialized annotations list used
    ///     for IAnnotatable. Touching that list (e.g. via IsFhirResource(), which reads
    ///     ITypedElement.Definition and so calls FindInspector()/Annotation&lt;T&gt;() under the
    ///     hood) mutates a node's hash code in place. That makes plain record equality unsafe for
    ///     tracking "have I already visited this position" in a HashSet&lt;PocoNode&gt; - a node
    ///     can silently become unfindable after being inserted. Comparing by the live Poco
    ///     reference instead (which is what actually identifies a position in the live resource
    ///     graph) sidesteps the mutable-annotations trap entirely.
    /// </summary>
    public sealed class PocoNodeIdentityComparer : IEqualityComparer<PocoNode>
    {
        public static readonly PocoNodeIdentityComparer Instance = new();

        public bool Equals(PocoNode x, PocoNode y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return ReferenceEquals(x.Poco, y.Poco);
        }

        public int GetHashCode(PocoNode obj) =>
            obj?.Poco is null ? 0 : RuntimeHelpers.GetHashCode(obj.Poco);
    }
}
