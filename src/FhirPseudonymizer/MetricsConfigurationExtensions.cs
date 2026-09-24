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
                            Boundaries = [.. Enumerable.Range(0, 20).Select(i => 1d + (5d * i))],
                        }
                    )
                    .AddView(
                        "fhirpseudonymizer.kafka.message.duration",
                        new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries =
                            [
                                0.05,
                                0.1,
                                0.25,
                                0.5,
                                1,
                                2.5,
                                5,
                                10,
                                20,
                                30,
                                60,
                                120,
                                300,
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
                        // Host/Port build a System.Uri internally, so neither HttpListener's own
                        // "+"/"*" wildcard syntax nor a literal "0.0.0.0" work here - Uri rejects
                        // the former outright, and .NET's cross-platform HttpListener refuses to
                        // bind the latter on Linux ("the request is not supported"). Left at the
                        // "localhost" default (which Uri accepts, giving the constructor a prefix
                        // it can register without throwing) and replaced below with the actual
                        // all-interfaces prefix, needed since the scraper is never the same host
                        // as the container.
                        options.Port = metricsPort;
                        options.ConfigureHttpListener = (_, listener) =>
                        {
                            listener.Prefixes.Clear();
                            listener.Prefixes.Add($"http://+:{metricsPort}/metrics/");
                        };
                    })
            );

        return services;
    }
}
