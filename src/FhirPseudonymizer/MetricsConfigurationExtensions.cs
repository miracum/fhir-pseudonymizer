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
                    // A standalone HttpListener on its own port, rather than
                    // AddPrometheusExporter()/MapPrometheusScrapingEndpoint(): that alternative
                    // maps /metrics onto the app's own Kestrel pipeline, which means adding a
                    // second Kestrel listener for it - and Kestrel drops ASPNETCORE_URLS/
                    // ASPNETCORE_HTTP_PORTS support entirely as soon as any endpoint is configured
                    // in code or via the Kestrel:Endpoints config section. This exporter instead
                    // runs its own independent listener that never touches the app's Kestrel
                    // configuration, so the app's own port keeps responding to the standard env
                    // vars exactly as it did under prometheus-net. It also means this port
                    // inherently serves nothing but /metrics - there's no shared routing table for
                    // anything else to be reachable through.
                    .AddPrometheusHttpListener(options =>
                    {
                        // Default is "localhost", which HttpListener binds loopback-only - useless
                        // for a container, where the scraper is never the same host. This builds a
                        // System.Uri internally, which rejects HttpListener's own "*"/"+" wildcard
                        // host syntax, so the all-interfaces address has to be spelled out instead.
                        options.Host = "0.0.0.0";
                        options.Port = metricsPort;
                    })
            );

        return services;
    }
}
