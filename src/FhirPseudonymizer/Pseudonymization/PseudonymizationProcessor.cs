using System.Text.RegularExpressions;
using FhirPseudonymizer.Config;
using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace FhirPseudonymizer.Pseudonymization;

public partial class PseudonymizationProcessor : IAnonymizerProcessor
{
    public PseudonymizationProcessor(IPseudonymServiceClient psnClient, FeatureManagement features)
    {
        PsnClient = psnClient;
        IsConditionalReferencePseudonymizationEnabled =
            features.ConditionalReferencePseudonymization;
    }

    [GeneratedRegex("^(?<domain>.*?)(\\/|\\?)")]
    private static partial Regex ResourceTypeRegex();

    protected IPseudonymServiceClient PsnClient { get; }
    private Regex ResourceTypeMatcher { get; } = ResourceTypeRegex();
    private bool IsConditionalReferencePseudonymizationEnabled { get; }

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
        if (node.IsReferenceStringNode() || node.IsReferenceUriNode(input))
        {
            // if the domain setting is not set,
            // create a domain from the reference, ie "Patient/123" -> "Patient"
            domain ??= ReferenceUtility.GetReferencePrefix(input).TrimEnd('/');

            node.Value = ReferenceUtility.TransformReferenceId(
                input,
                x => GetOrCreatePseudonym(x, domainPrefix.ToString() + domain, settings)
            );
        }
        else if (
            IsConditionalReferencePseudonymizationEnabled && node.IsConditionalReferenceNode(input)
        )
        {
            domain ??= ResourceTypeMatcher
                .Match(ReferenceUtility.GetReferencePrefix(input))
                .Groups["domain"]
                .Value;

            node.Value = ReferenceUtility.TransformReferenceId(
                input,
                x => GetOrCreatePseudonym(x, domainPrefix.ToString() + domain, settings)
            );
        }
        else
        {
            node.Value = GetOrCreatePseudonym(input, domainPrefix.ToString() + domain, settings);
        }

        processResult.AddProcessRecord(AnonymizationOperations.Pseudonymize, node);
        return processResult;
    }

    protected virtual string GetOrCreatePseudonym(
        string input,
        string domain,
        IReadOnlyDictionary<string, object> settings
    )
    {
        return PsnClient.GetOrCreatePseudonymFor(input, domain, settings).Result;
    }
}

public class DePseudonymizationProcessor : PseudonymizationProcessor
{
    public DePseudonymizationProcessor(
        IPseudonymServiceClient psnClient,
        FeatureManagement features
    )
        : base(psnClient, features) { }

    protected override string GetOrCreatePseudonym(
        string input,
        string domain,
        IReadOnlyDictionary<string, object> settings
    )
    {
        return PsnClient.GetOriginalValueFor(input, domain, settings).Result;
    }
}
