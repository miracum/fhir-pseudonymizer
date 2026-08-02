using System.Collections;
using System.Text.RegularExpressions;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class PocoNodeExtension
    {
        private static readonly string s_locationToFhirPathRegex = @"\[.*?\]";

        /// <summary>
        ///     Builds a root PocoNode for the given POCO. PocoNodeOrList.Root() alone doesn't
        ///     attach a ModelInspector annotation, so ITypedElement.Definition - and therefore
        ///     IsFhirResource() - would silently return null/false for the root and every
        ///     descendant (FindInspector() walks up looking for one and finds nothing), which
        ///     means the anonymization visitor would never recognize any resource in the tree and
        ///     rules would silently match nothing.
        /// </summary>
        public static PocoNode CreateRootNode(Base poco)
        {
            var root = (PocoNode)PocoNodeOrList.Root(poco);
            ((IAnnotatable)root).AddAnnotation(ModelInfo.ModelInspector);
            return root;
        }

        // InstanceType/Value/Location/Definition/Children(name) are explicit ITypedElement
        // interface implementations on PocoNode, so they're only reachable through that
        // interface - not directly off a PocoNode-typed reference.
        public static string GetInstanceType(this PocoNode node) =>
            ((ITypedElement)node)?.InstanceType;

        public static bool IsDateNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.DateTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsDateTimeNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.DateTimeTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsAgeDecimalNode(this PocoNode node)
        {
            return node != null
                && node.Parent.IsAgeNode()
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.DecimalTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsInstantNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.InstantTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsAgeNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.AgeTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsBundleNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.BundleTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsReferenceNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    Constants.ReferenceTypeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsPostalCodeNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.Name,
                    Constants.PostalCodeNodeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsReferenceStringNode(this PocoNode node)
        {
            return node != null
                && node.Parent.IsReferenceNode()
                && string.Equals(
                    node.Name,
                    Constants.ReferenceStringNodeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsReferenceUriNode(this PocoNode node, string value)
        {
            return node != null
                && string.Equals(
                    node.GetInstanceType(),
                    "uri",
                    StringComparison.InvariantCultureIgnoreCase
                )
                && ReferenceUtility.IsResourceReference(value);
        }

        public static bool IsConditionalReferenceNode(this PocoNode node, string value)
        {
            return node != null
                && string.Equals(
                    node.Name,
                    "ifNoneExist",
                    StringComparison.InvariantCultureIgnoreCase
                )
                && ReferenceUtility.IsResourceReference(value);
        }

        public static bool IsEntryNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.Name,
                    Constants.EntryNodeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool IsContainedNode(this PocoNode node)
        {
            return node != null
                && string.Equals(
                    node.Name,
                    Constants.ContainedNodeName,
                    StringComparison.InvariantCultureIgnoreCase
                );
        }

        public static bool HasContainedNode(this PocoNode node)
        {
            return node != null && node.ChildrenByName(Constants.ContainedNodeName).Any();
        }

        public static bool IsFhirResource(this PocoNode node)
        {
            return node != null && (((ITypedElement)node).Definition?.IsResource ?? false);
        }

        public static string GetFhirPath(this PocoNode node)
        {
            return node == null
                ? string.Empty
                : Regex.Replace(
                    ((ITypedElement)node).Location,
                    s_locationToFhirPathRegex,
                    string.Empty
                );
        }

        public static string GetNodeId(this PocoNode node)
        {
            var id = node.ChildrenByName("id").FirstOrDefault();
            return id?.GetValue()?.ToString() ?? string.Empty;
        }

        public static PocoNode GetMeta(this PocoNode node)
        {
            return node?.ChildrenByName("meta").FirstOrDefault();
        }

        /// <summary>
        ///     Sets the value of a primitive leaf node in place on the live POCO it wraps.
        ///     Uses the (obsolete-but-functional) ObjectValue setter rather than its JsonValue
        ///     replacement: JsonValue changed representation for base64Binary/instant/integer64
        ///     to plain strings, while ObjectValue keeps the original CLR types (byte[]/
        ///     DateTimeOffset/long) that the date-shift/redact utilities below are written
        ///     against.
        /// </summary>
        public static void SetPrimitiveValue(this PocoNode node, object value)
        {
#pragma warning disable CS0618 // ObjectValue is obsolete in favor of JsonValue
            ((PrimitiveType)node.Poco).ObjectValue = value;
#pragma warning restore CS0618
        }

        /// <summary>
        ///     Detaches <paramref name="child" /> from this node's live POCO graph - removing it
        ///     from the repeating list it lives in, or nulling the singular property, whichever
        ///     applies.
        /// </summary>
        public static void RemoveChild(this PocoNode parent, PocoNode child)
        {
            if (parent?.Poco == null || child == null)
            {
                return;
            }

            if (parent.Poco.TryGetValue(child.Name, out var current) && current is IList list)
            {
                list.Remove(child.Poco);
            }
            else
            {
                parent.Poco.SetValue(child.Name, null);
            }
        }

        /// <summary>
        ///     Attaches <paramref name="child" /> (typically borrowed from an otherwise-disconnected
        ///     replacement template built during substitution) onto this node's live POCO graph
        ///     under the given element name, appending to the existing repeating list if there is
        ///     one.
        /// </summary>
        public static void AddChild(this PocoNode node, string name, Base child)
        {
            if (node.Poco.TryGetValue(name, out var current) && current is IList existing)
            {
                existing.Add(child);
                return;
            }

            // The property has no value yet, so its cardinality can't be inferred from an
            // existing value the way the branch above does - ask the model directly instead.
            // Wrapping a value for a singular property (e.g. CodeableConcept.text) in a list
            // here would make SetValue silently misroute it into the overflow dictionary rather
            // than the real property, since the shape wouldn't match what's declared for it.
            var isCollection =
                ModelInfo
                    .ModelInspector.FindClassMapping(node.Poco)
                    ?.FindMappedElementByName(name)
                    ?.IsCollection
                ?? false;
            node.Poco.SetValue(name, isCollection ? new List<Base> { child } : child);
        }

        public static IEnumerable<PocoNode> ChildrenByName(this PocoNode node, string name)
        {
            return ((ITypedElement)node).Children(name).CastPocoNodes();
        }

        public static IEnumerable<PocoNode> CastPocoNodes(this IEnumerable<ITypedElement> input)
        {
            return input.Cast<PocoNode>();
        }

        public static IEnumerable<PocoNode> CastPocoNodes(this IEnumerable<PocoNodeOrList> input)
        {
            return input.SelectMany(x => x);
        }
    }
}
