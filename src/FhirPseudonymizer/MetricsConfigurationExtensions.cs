using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace FhirPseudonymizer;

public static class MetricsConfigurationExtensions
{
    public static IServiceCollection AddMetrics(
        this IServiceCollection services,
        ushort metricsPort
    )
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(r =>
                r.AddService(
                    serviceName: Program.ServiceName,
                    serviceVersion: Program.ServiceVersion,
                    serviceInstanceId: Environment.MachineName
                )
            )
            .WithMetrics(metricsBuilder =>
                metricsBuilder
                    .AddMeter(Program.Meter.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    // Matches the 20 linear buckets of width 5 (1, 6, 11, ..., 96) that this
                    // histogram used under prometheus-net - the default OpenTelemetry boundaries
                    // are tuned for durations in seconds, not a count of bundle entries.
                    .AddView(
                        "fhirpseudonymizer.received.bundle_size",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries =
                            [
                                .. Enumerable.Range(0, 20).Select(i => 1d + (5d * i)),
                            ],
                        }
                    )
                    .AddPrometheusExporter()
            );

        // A dedicated metrics port (separate from the app's public HTTP listener) keeps /metrics
        // off the internet-facing endpoint - only an in-cluster scraper needs to reach it. Kept
        // as a second, code-configured Kestrel listener alongside the appsettings.json-configured
        // one, rather than folding it into that. The separation is enforced in both directions by
        // the MetricsPortGuard middleware in Startup - /metrics answers only on this port, and
        // this port answers nothing but /metrics.
        services.Configure<KestrelServerOptions>(options => options.ListenAnyIP(metricsPort));

        return services;
    }
}
