using System;
using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer
{
    public class GPasPseudonymizationProcessor : IAnonymizerProcessor
    {
        public GPasPseudonymizationProcessor(IGPasFhirClient psnClient)
        {
            GPasClient = psnClient;
        }

        protected IGPasFhirClient GPasClient { get; }

        public ProcessResult Process(ElementNode node, ProcessContext context = null,
            Dictionary<string, object> settings = null)
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
            {
                return processResult;
            }

            // prefix the domain, if set
            var domainPrefix = settings?.GetValueOrDefault("domain-prefix", string.Empty);

            var domain = settings?
                .GetValueOrDefault("domain", null)?
                .ToString();

            var input = node.Value.ToString();

            // Pseudonymize the id part for "Reference.reference" node and
            // pseudonymize whole input for other node types
            if (node.IsReferenceStringNode() || IsReferenceUriNode(node, input))
            {
                // if the domain setting is not set,
                // create a domain from the reference, ie "Patient/123" -> "Patient"
                domain ??= ReferenceUtility
                    .GetReferencePrefix(input)
                    .TrimEnd('/');

                node.Value = ReferenceUtility.TransformReferenceId(
                    input,
                    x => GetOrCreatePseudonym(x, domainPrefix + domain));
            }
            else
            {
                node.Value = GetOrCreatePseudonym(input, domainPrefix + domain);
            }

            processResult.AddProcessRecord(AnonymizationOperations.Pseudonymize, node);
            return processResult;
        }

        protected virtual string GetOrCreatePseudonym(string input, string domain)
        {
            return GPasClient.GetOrCreatePseudonymFor(input, domain).Result;
        }

        private static bool IsReferenceUriNode(ElementNode node, string value)
        {
            return node.InstanceType.Equals("uri", StringComparison.InvariantCultureIgnoreCase)
                   && ReferenceUtility.IsResourceReference(value);
        }
    }

    public class GPasDePseudonymizationProcessor : GPasPseudonymizationProcessor
    {
        public GPasDePseudonymizationProcessor(IGPasFhirClient psnClient) : base(psnClient)
        {
        }

        protected override string GetOrCreatePseudonym(string input, string domain)
        {
            return GPasClient.GetOriginalValueFor(input, domain).Result;
        }
    }
}
