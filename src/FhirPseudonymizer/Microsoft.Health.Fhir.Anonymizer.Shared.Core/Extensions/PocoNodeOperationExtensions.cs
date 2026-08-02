using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Visitors;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class PocoNodeOperationExtensions
    {
        public static async Task<PocoNode> AnonymizeAsync(
            this PocoNode node,
            AnonymizationFhirPathRule[] rules,
            Dictionary<string, IAnonymizerProcessor> processors,
            AnonymizerSettings settings = null
        )
        {
            var visitor = new AnonymizationVisitor(rules, processors, settings);
            await node.AcceptAsync(visitor);
            node.RemoveNullChildren();

            return node;
        }

        // Remove null children of current node, and return true => current node is null
        public static bool RemoveNullChildren(this PocoNode node)
        {
            if (node == null)
            {
                return true;
            }

            var children = node.Children().CastPocoNodes().ToList();
            foreach (var child in children)
            {
                // Remove child if it is null => return true
                if (RemoveNullChildren(child))
                {
                    node.RemoveChild(child);
                }
            }

            var currentNodeIsEmpty =
                !node.Children().CastPocoNodes().Any() && node.GetValue() == null;
            var currentNodeIsFhirResource = node.IsFhirResource();
            if (currentNodeIsEmpty && !currentNodeIsFhirResource)
            {
                return true;
            }

            return false;
        }

        public static void AddSecurityTag(this PocoNode node, ProcessResult result)
        {
            if (node == null)
            {
                return;
            }

            if (result.ProcessRecords.Count == 0)
            {
                return;
            }

            // node is always a resource here (guarded by the AnonymizationVisitor caller), so its
            // live Poco can be edited directly via the strongly-typed Meta property instead of
            // splicing a synthetic node into the tree.
            var resource = (Resource)node.Poco;
            var meta = resource.Meta ?? new Meta();

            if (
                result.IsRedacted
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.REDACT.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.REDACT);
            }

            if (
                result.IsAbstracted
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.ABSTRED.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.ABSTRED);
            }

            if (
                result.IsCryptoHashed
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.CRYTOHASH.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.CRYTOHASH);
            }

            if (
                result.IsEncrypted
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.ENCRYPT.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.ENCRYPT);
            }

            if (
                result.IsPerturbed
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.PERTURBED.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.PERTURBED);
            }

            if (
                result.IsSubstituted
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.SUBSTITUTED.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.SUBSTITUTED);
            }

            if (
                result.IsGeneralized
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.GENERALIZED.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.GENERALIZED);
            }

            if (
                result.IsPseudonymized
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.PSEUDED.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.PSEUDED);
            }

            if (
                result.IsRemoved
                && !meta.Security.Any(x =>
                    string.Equals(
                        x.Code,
                        SecurityLabels.REDACT.Code,
                        StringComparison.InvariantCultureIgnoreCase
                    )
                )
            )
            {
                meta.Security.Add(SecurityLabels.REDACT);
            }

            resource.Meta = meta;
        }
    }
}
