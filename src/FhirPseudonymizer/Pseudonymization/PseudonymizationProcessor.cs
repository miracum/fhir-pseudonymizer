using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Pseudonymization
{
    public class PseudonymizationProcessor : IAnonymizerProcessor
    {
        public PseudonymizationProcessor(IPseudonymServiceClient psnClient)
        {
            PsnClient = psnClient;
        }

        protected IPseudonymServiceClient PsnClient { get; }
        private Regex ResourceTypeMatcher { get; } = new(@"^(?<domain>.*?)(\/|\?)");

        public ProcessResult Process(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
            {
                return processResult;
            }

            // prefix the domain, if set
            var domainPrefix =
                settings?.GetValueOrDefault("domain-prefix", null)
                ?? settings?.GetValueOrDefault("namespace-prefix", string.Empty);

            var domain =
                settings?.GetValueOrDefault("domain", null)
                ?? settings?.GetValueOrDefault("namespace", null)?.ToString();

            var input = node.Value.ToString();

            // Pseudonymize the id part for "Reference.reference" node and
            // pseudonymize whole input for other node types
            if (node.IsReferenceStringNode() || IsReferenceUriNode(node, input) || IsConditionalElement(node, input))
            {
                // if the domain setting is not set,
                // create a domain from the reference, ie "Patient/123" -> "Patient"
                domain ??= ResourceTypeMatcher
                    .Match(ReferenceUtility.GetReferencePrefix(input))
                    .Groups["domain"].Value;

                node.Value = ReferenceUtility.TransformReferenceId(
                    input,
                    x => GetOrCreatePseudonym(x, domainPrefix.ToString() + domain)
                );
            }
            else
            {
                node.Value = GetOrCreatePseudonym(input, domainPrefix.ToString() + domain);
            }

            processResult.AddProcessRecord(AnonymizationOperations.Pseudonymize, node);
            return processResult;
        }

        protected virtual string GetOrCreatePseudonym(string input, string domain)
        {
            return PsnClient.GetOrCreatePseudonymFor(input, domain).Result;
        }

        private static bool IsReferenceUriNode(ElementNode node, string value)
        {
            return node.InstanceType.Equals("uri", StringComparison.InvariantCultureIgnoreCase)
                && ReferenceUtility.IsResourceReference(value);
        }

        private static bool IsConditionalElement(ElementNode node, string value)
        {
            return node.Name.Equals("ifNoneExist", StringComparison.InvariantCultureIgnoreCase)
                   && ReferenceUtility.IsResourceReference(value);
        }
    }

    public class DePseudonymizationProcessor : PseudonymizationProcessor
    {
        public DePseudonymizationProcessor(IPseudonymServiceClient psnClient) : base(psnClient) { }

        protected override string GetOrCreatePseudonym(string input, string domain)
        {
            return PsnClient.GetOriginalValueFor(input, domain).Result;
        }
    }
}
