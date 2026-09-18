using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Covers the cancellation path through the anonymization engine: a caller that goes away
///     mid-bundle should stop the walk instead of pseudonymizing every remaining entry.
/// </summary>
public class CancellationTests
{
    private const int BundleEntryCount = 50;

    // A rule scoped to the resource type: each Patient in the bundle is matched separately, so
    // the engine re-enters the per-rule loop once per entry.
    private const string PerResourceRulePath = "Patient.identifier.value";

    // A rule evaluated against the Bundle itself, the shape the shipped anonymization.yaml uses
    // via nodesByType(...): one rule iteration matches every entry at once, so the per-rule check
    // fires only once for the whole bundle.
    private const string WholeBundleRulePath = "Bundle.entry.resource.identifier.value";

    // A rule that matches nothing in the test bundle: the FHIRPath evaluation still runs once per
    // resource, but no node is ever handed to a processor.
    private const string NonMatchingRulePath = "Patient.telecom.value";

    private static string ConfigFor(string rulePath) =>
        $"""
            fhirVersion: R4
            fhirPathRules:
              - path: {rulePath}
                method: pseudonymize
                domain: test
            parameters:
              dateShiftKey: ""
              dateShiftScope: resource
              cryptoHashKey: "secret"
              encryptKey: ""
              enablePartialAgesForRedact: true
              enablePartialDatesForRedact: true
              enablePartialZipCodesForRedact: true
              restrictedZipCodeTabulationAreas: []
            """;

    private static AnonymizerEngine CreateEngine(
        IPseudonymServiceClient psnClient,
        string rulePath = PerResourceRulePath
    )
    {
        var configManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
            ConfigFor(rulePath)
        );
        var engine = new AnonymizerEngine(configManager);
        engine.AddProcessor(
            "pseudonymize",
            new PseudonymizationProcessor(psnClient, new FeatureManagement())
        );
        return engine;
    }

    private static Bundle CreateBundle()
    {
        var bundle = new Bundle();
        for (var i = 0; i < BundleEntryCount; i++)
        {
            bundle.Entry.Add(
                new Bundle.EntryComponent
                {
                    Resource = new Patient
                    {
                        Id = i.ToString(),
                        Identifier = { new Identifier("http://example.com/mrn", $"patient-{i}") },
                    },
                }
            );
        }

        return bundle;
    }

    [Fact]
    public async Task AnonymizeResourceAsync_WithoutCancellation_ProcessesEveryEntry()
    {
        var psnClient = new RecordingPseudonymServiceClient();

        await CreateEngine(psnClient)
            .AnonymizeResourceAsync(
                CreateBundle(),
                new AnonymizerSettings(),
                TestContext.Current.CancellationToken
            );

        psnClient.Calls.Should().Be(BundleEntryCount);
    }

    [Fact]
    public async Task AnonymizeResourceAsync_WhenCancelledMidBundle_StopsWithoutProcessingTheRest()
    {
        using var cts = new CancellationTokenSource();
        const int cancelAfter = 5;
        var psnClient = new RecordingPseudonymServiceClient(cts, cancelAfter);

        var act = async () =>
            await CreateEngine(psnClient)
                .AnonymizeResourceAsync(CreateBundle(), new AnonymizerSettings(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();

        // The point of the exercise: the remaining entries are never sent to the
        // pseudonymization service. The exact count may exceed cancelAfter by the work already
        // in flight, but must stay far below the whole bundle.
        psnClient.Calls.Should().BeGreaterThanOrEqualTo(cancelAfter).And.BeLessThan(10);
    }

    // Guards the second cancellation check, the one inside the node recursion. With a rule that
    // matches the whole bundle in a single pass the per-rule check runs exactly once, so without
    // that inner check the run would pseudonymize all 50 entries before noticing.
    [Fact]
    public async Task AnonymizeResourceAsync_WhenOneRuleMatchesTheWholeBundle_StillStopsOnCancellation()
    {
        using var cts = new CancellationTokenSource();
        const int cancelAfter = 5;
        var psnClient = new RecordingPseudonymServiceClient(cts, cancelAfter);

        var act = async () =>
            await CreateEngine(psnClient, WholeBundleRulePath)
                .AnonymizeResourceAsync(CreateBundle(), new AnonymizerSettings(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();

        psnClient.Calls.Should().BeGreaterThanOrEqualTo(cancelAfter).And.BeLessThan(10);
    }

    // Guards the per-rule check. Rules that match nothing still cost a FHIRPath evaluation per
    // resource, which on a large bundle is most of the work - and with no matches the node
    // recursion never runs, so that check is the only thing left that can notice a cancellation.
    [Fact]
    public async Task AnonymizeResourceAsync_WithRulesThatMatchNothing_IsStillCancellable()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var psnClient = new RecordingPseudonymServiceClient();

        var act = async () =>
            await CreateEngine(psnClient, NonMatchingRulePath)
                .AnonymizeResourceAsync(CreateBundle(), new AnonymizerSettings(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        psnClient.Calls.Should().Be(0);
    }

    [Fact]
    public async Task AnonymizeResourceAsync_ForwardsTheCallersTokenToThePseudonymService()
    {
        using var cts = new CancellationTokenSource();
        var psnClient = new RecordingPseudonymServiceClient();

        await CreateEngine(psnClient)
            .AnonymizeResourceAsync(CreateBundle(), new AnonymizerSettings(), cts.Token);

        psnClient.ReceivedTokens.Should().OnlyContain(token => token == cts.Token);
    }

    [Fact]
    public async Task AnonymizeResourceAsync_WithAnAlreadyCancelledToken_DoesNoWorkAtAll()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var psnClient = new RecordingPseudonymServiceClient();

        var act = async () =>
            await CreateEngine(psnClient)
                .AnonymizeResourceAsync(CreateBundle(), new AnonymizerSettings(), cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
        psnClient.Calls.Should().Be(0);
    }

    /// <summary>
    ///     Counts calls, records the token each one was given, and optionally cancels the given
    ///     source once it has been called <paramref name="cancelAfter" /> times - standing in for
    ///     a caller that disconnects part-way through a bundle.
    /// </summary>
    private sealed class RecordingPseudonymServiceClient(
        CancellationTokenSource cancelAfterCalls = null,
        int cancelAfter = int.MaxValue
    ) : IPseudonymServiceClient
    {
        public int Calls { get; private set; }

        public List<CancellationToken> ReceivedTokens { get; } = [];

        public Task<string> GetOrCreatePseudonymFor(
            string value,
            string domain,
            IReadOnlyDictionary<string, object> settings = null,
            CancellationToken cancellationToken = default
        )
        {
            Calls++;
            ReceivedTokens.Add(cancellationToken);

            if (Calls >= cancelAfter)
            {
                cancelAfterCalls?.Cancel();
            }

            return Task.FromResult($"pseudonym-for-{value}");
        }

        public Task<string> GetOriginalValueFor(
            string pseudonym,
            string domain,
            IReadOnlyDictionary<string, object> settings = null,
            CancellationToken cancellationToken = default
        ) => throw new NotSupportedException();
    }
}
