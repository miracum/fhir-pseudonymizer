using System.Diagnostics;
using System.Diagnostics.Metrics;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer;

/// <summary>
///     Records how long the anonymization engine took to de-identify (or de-pseudonymize) a single
///     resource or bundle - both in total and per resource it contains, so durations stay
///     comparable no matter how large the bundles are. Shared by the REST API and the Kafka
///     consumer, told apart by the <c>source</c> tag.
/// </summary>
public static class AnonymizationMetrics
{
    // The same values the REST API's fhirpseudonymizer.received.bundle_size uses.
    public const string OperationDeIdentify = nameof(Controllers.FhirController.DeIdentify);
    public const string OperationDePseudonymize = nameof(Controllers.FhirController.DePseudonymize);

    public const string SourceKafka = "kafka";
    public const string SourceRest = "rest";

    // Bucket boundaries of all three are configured as OpenTelemetry Views - see
    // MetricsConfigurationExtensions.
    private static readonly Histogram<double> Duration = Program.Meter.CreateHistogram<double>(
        "fhirpseudonymizer.anonymization.duration",
        unit: "s",
        description: "Time the anonymization engine took to process a single resource or bundle, including waiting for the pseudonymization service, but not parsing or serializing it."
    );

    private static readonly Histogram<double> ResourceDuration =
        Program.Meter.CreateHistogram<double>(
            "fhirpseudonymizer.anonymization.resource.duration",
            unit: "s",
            description: "Time the anonymization engine took to process a single resource or bundle, divided by the number of resources it contains (see fhirpseudonymizer.anonymization.resources)."
        );

    private static readonly Histogram<long> Resources = Program.Meter.CreateHistogram<long>(
        "fhirpseudonymizer.anonymization.resources",
        unit: "{resource}",
        description: "Number of resources in each resource or bundle processed by the anonymization engine: the number of entries for a bundle, 1 for any other resource."
    );

    /// <summary>
    ///     Runs <paramref name="anonymize" /> on <paramref name="resource" /> and records how long
    ///     it took, unless it failed after <paramref name="cancellationToken" /> was cancelled:
    ///     how long a cancelled run took says nothing about the resource.
    /// </summary>
    public static async Task<Resource> MeasureAsync(
        string operation,
        string source,
        Resource resource,
        Func<Task<Resource>> anonymize,
        CancellationToken cancellationToken = default
    )
    {
        // Counted up front, since the engine modifies the resource in place - e.g. a remove rule
        // matching bundle entries leaves fewer of them behind.
        var resourceCount = CountResources(resource);
        var startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            var anonymized = await anonymize();
            Record(operation, source, "success", resourceCount, startTimestamp);
            return anonymized;
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            Record(operation, source, "error", resourceCount, startTimestamp);
            throw;
        }
    }

    private static int CountResources(Resource resource)
    {
        // An empty bundle still is a resource to process.
        if (resource is Bundle bundle)
        {
            return Math.Max(1, bundle.Entry.Count);
        }

        return 1;
    }

    private static void Record(
        string operation,
        string source,
        string outcome,
        int resourceCount,
        long startTimestamp
    )
    {
        var elapsedSeconds = Stopwatch.GetElapsedTime(startTimestamp).TotalSeconds;
        var tags = new TagList
        {
            { "operation", operation },
            { "source", source },
            { "outcome", outcome },
        };

        Duration.Record(elapsedSeconds, tags);
        ResourceDuration.Record(elapsedSeconds / resourceCount, tags);
        Resources.Record(resourceCount, tags);
    }
}
