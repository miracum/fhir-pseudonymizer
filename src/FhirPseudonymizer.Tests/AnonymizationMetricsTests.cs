using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Tests;

public class AnonymizationMetricsTests
{
    [Fact]
    public async Task MeasureAsync_WithBundle_ShouldRecordDurationPerEntryCountedBeforeAnonymizing()
    {
        using var durations = new MetricRecorder<double>(
            "fhirpseudonymizer.anonymization.duration"
        );
        using var resourceDurations = new MetricRecorder<double>(
            "fhirpseudonymizer.anonymization.resource.duration"
        );
        using var resourceCounts = new MetricRecorder<long>(
            "fhirpseudonymizer.anonymization.resources"
        );

        var bundle = new Bundle
        {
            Entry =
            [
                new() { Resource = new Patient() },
                new() { Resource = new Encounter() },
                new() { Resource = new Encounter() },
                new() { Resource = new Observation() },
            ],
        };

        var anonymized = await AnonymizationMetrics.MeasureAsync(
            AnonymizationMetrics.OperationDeIdentify,
            AnonymizationMetrics.SourceKafka,
            bundle,
            async () =>
            {
                await Task.Delay(
                    TimeSpan.FromMilliseconds(20),
                    TestContext.Current.CancellationToken
                );
                // like a remove rule matching the encounter entries
                bundle.Entry.RemoveAll(entry => entry.Resource is Encounter);
                return bundle;
            },
            TestContext.Current.CancellationToken
        );

        anonymized.Should().BeSameAs(bundle);

        var duration = durations.Measurements.Should().ContainSingle().Subject;
        // not the full 20 ms: timers have a resolution of a millisecond or so, and may fire early
        duration.Value.Should().BeGreaterThanOrEqualTo(0.015);
        duration
            .Tags.Should()
            .BeEquivalentTo(
                new Dictionary<string, object>
                {
                    ["operation"] = "DeIdentify",
                    ["source"] = "kafka",
                    ["outcome"] = "success",
                }
            );

        resourceCounts.Measurements.Should().ContainSingle().Which.Value.Should().Be(4);
        resourceDurations
            .Measurements.Should()
            .ContainSingle()
            .Which.Value.Should()
            .BeApproximately(duration.Value / 4, 0.000_001);
    }

    [Fact]
    public async Task MeasureAsync_WithSingleResource_ShouldCountOneResource()
    {
        using var resourceCounts = new MetricRecorder<long>(
            "fhirpseudonymizer.anonymization.resources"
        );
        var patient = new Patient();

        await AnonymizationMetrics.MeasureAsync(
            AnonymizationMetrics.OperationDePseudonymize,
            AnonymizationMetrics.SourceRest,
            patient,
            () => Task.FromResult<Resource>(patient),
            TestContext.Current.CancellationToken
        );

        var resourceCount = resourceCounts.Measurements.Should().ContainSingle().Subject;
        resourceCount.Value.Should().Be(1);
        resourceCount.Tags["operation"].Should().Be("DePseudonymize");
        resourceCount.Tags["source"].Should().Be("rest");
    }

    [Fact]
    public async Task MeasureAsync_WithEmptyBundle_ShouldCountTheBundleItself()
    {
        using var resourceCounts = new MetricRecorder<long>(
            "fhirpseudonymizer.anonymization.resources"
        );
        var bundle = new Bundle();

        await AnonymizationMetrics.MeasureAsync(
            AnonymizationMetrics.OperationDeIdentify,
            AnonymizationMetrics.SourceRest,
            bundle,
            () => Task.FromResult<Resource>(bundle),
            TestContext.Current.CancellationToken
        );

        resourceCounts.Measurements.Should().ContainSingle().Which.Value.Should().Be(1);
    }

    [Fact]
    public async Task MeasureAsync_WhenAnonymizationFails_ShouldRecordErrorAndRethrow()
    {
        using var durations = new MetricRecorder<double>(
            "fhirpseudonymizer.anonymization.duration"
        );

        var act = () =>
            AnonymizationMetrics.MeasureAsync(
                AnonymizationMetrics.OperationDeIdentify,
                AnonymizationMetrics.SourceRest,
                new Patient(),
                () => throw new InvalidOperationException("boom"),
                TestContext.Current.CancellationToken
            );

        await act.Should().ThrowAsync<InvalidOperationException>();
        durations.Measurements.Should().ContainSingle().Which.Tags["outcome"].Should().Be("error");
    }

    [Fact]
    public async Task MeasureAsync_WhenFailingAfterCancellation_ShouldRecordNothing()
    {
        using var durations = new MetricRecorder<double>(
            "fhirpseudonymizer.anonymization.duration"
        );
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        var act = () =>
            AnonymizationMetrics.MeasureAsync(
                AnonymizationMetrics.OperationDeIdentify,
                AnonymizationMetrics.SourceKafka,
                new Patient(),
                () => throw new OperationCanceledException(cancellationTokenSource.Token),
                cancellationTokenSource.Token
            );

        await act.Should().ThrowAsync<OperationCanceledException>();
        durations.Measurements.Should().BeEmpty();
    }
}
