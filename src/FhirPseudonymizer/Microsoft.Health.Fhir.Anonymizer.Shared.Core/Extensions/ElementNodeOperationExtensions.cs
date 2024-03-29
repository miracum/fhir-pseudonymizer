using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Visitors;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions
{
    public static class ElementNodeOperationExtensions
    {
        private static readonly PocoStructureDefinitionSummaryProvider s_provider =
            new PocoStructureDefinitionSummaryProvider();

        public static ElementNode Anonymize(
            this ElementNode node,
            AnonymizationFhirPathRule[] rules,
            Dictionary<string, IAnonymizerProcessor> processors,
            AnonymizerSettings settings = null
        )
        {
            var visitor = new AnonymizationVisitor(rules, processors, settings);
            node.Accept(visitor);
            node.RemoveNullChildren();

            return node;
        }

        // Remove null children of current node, and return true => current node is null
        public static bool RemoveNullChildren(this ElementNode node)
        {
            if (node == null)
            {
                return true;
            }

            var children = node.Children().CastElementNodes().ToList();
            foreach (var child in children)
            {
                // Remove child if it is null => return true
                if (RemoveNullChildren(child))
                {
                    node.Remove(child);
                }
            }

            var currentNodeIsEmpty = !node.Children().Any() && node.Value == null;
            var currentNodeIsFhirResource = node.IsFhirResource();
            if (currentNodeIsEmpty && !currentNodeIsFhirResource)
            {
                return true;
            }

            return false;
        }

        public static void AddSecurityTag(this ElementNode node, ProcessResult result)
        {
            if (node == null)
            {
                return;
            }

            if (result.ProcessRecords.Count == 0)
            {
                return;
            }

            var metaNode = node.GetMeta();
            var meta = metaNode?.ToPoco<Meta>() ?? new Meta();

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

            var newMetaNode = ElementNode.FromElement(meta.ToTypedElement());
            if (metaNode == null)
            {
                node.Add(s_provider, newMetaNode);
            }
            else
            {
                node.Replace(s_provider, metaNode, newMetaNode);
            }
        }
    }
}
