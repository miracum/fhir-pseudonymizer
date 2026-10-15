using System.Text.RegularExpressions;
using FhirPseudonymizer.Config;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

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

    public async Task<ProcessResult> ProcessAsync(
        PocoNode node,
        ProcessContext context = null,
        Dictionary<string, object> settings = null
    )
    {
        var processResult = new ProcessResult();
        if (string.IsNullOrEmpty(node?.GetValue()?.ToString()))
        {
            return processResult;
        }

        // This is the one processor that makes a remote call per node, so it is the one that
        // needs the caller's token. The visitor always supplies a context; a null one only
        // happens when a test drives the processor directly, and then there is nothing to cancel.
        var cancellationToken = context?.CancellationToken ?? CancellationToken.None;

        // prefix the domain, if set
        var domainPrefix =
            settings?.GetValueOrDefault("domain-prefix", null)
            ?? settings?.GetValueOrDefault("namespace-prefix", null)
            ?? settings?.GetValueOrDefault("context-prefix", string.Empty);

        var domain =
            settings?.GetValueOrDefault("domain", null)
            ?? settings?.GetValueOrDefault("namespace", null)
            ?? settings?.GetValueOrDefault("context", null)?.ToString();

        var input = node.GetValue().ToString();

        // Pseudonymize the id part for "Reference.reference" node and
        // pseudonymize whole input for other node types
        if (node.IsReferenceStringNode() || node.IsReferenceUriNode(input))
        {
            // if the domain setting is not set,
            // create a domain from the reference, ie "Patient/123" -> "Patient"
            domain ??= ReferenceUtility.GetReferencePrefix(input).TrimEnd('/');

            node.SetPrimitiveValue(
                await ReferenceUtility.TransformReferenceIdAsync(
                    input,
                    x =>
                        GetOrCreatePseudonymAsync(
                            x,
                            domainPrefix.ToString() + domain,
                            settings,
                            cancellationToken
                        )
                )
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

            node.SetPrimitiveValue(
                await ReferenceUtility.TransformReferenceIdAsync(
                    input,
                    x =>
                        GetOrCreatePseudonymAsync(
                            x,
                            domainPrefix.ToString() + domain,
                            settings,
                            cancellationToken
                        )
                )
            );
        }
        else
        {
            node.SetPrimitiveValue(
                await GetOrCreatePseudonymAsync(
                    input,
                    domainPrefix.ToString() + domain,
                    settings,
                    cancellationToken
                )
            );
        }

        processResult.AddProcessRecord(AnonymizationOperations.Pseudonymize, node);
        return processResult;
    }

    protected virtual Task<string> GetOrCreatePseudonymAsync(
        string input,
        string domain,
        IReadOnlyDictionary<string, object> settings,
        CancellationToken cancellationToken
    )
    {
        return PsnClient.GetOrCreatePseudonymFor(input, domain, settings, cancellationToken);
    }
}

public class DePseudonymizationProcessor : PseudonymizationProcessor
{
    public DePseudonymizationProcessor(
        IPseudonymServiceClient psnClient,
        FeatureManagement features
    )
        : base(psnClient, features) { }

    protected override Task<string> GetOrCreatePseudonymAsync(
        string input,
        string domain,
        IReadOnlyDictionary<string, object> settings,
        CancellationToken cancellationToken
    )
    {
        return PsnClient.GetOriginalValueFor(input, domain, settings, cancellationToken);
    }
}
