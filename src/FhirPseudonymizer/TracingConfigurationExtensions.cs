using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FhirPseudonymizer;

public static class TracingConfigurationExtensions
{
    public static IServiceCollection AddTracing(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Build a resource configuration action to set service information.
        void configureResource(ResourceBuilder r) =>
            r.AddService(
                serviceName: Program.ServiceName,
                serviceVersion: Program.ServiceVersion,
                serviceInstanceId: Environment.MachineName
            );

        var rootSamplerType = configuration.GetValue("Tracing:RootSampler", "AlwaysOnSampler");
        var samplingRatio = configuration.GetValue("Tracing:SamplingProbability", 0.1d);

        Sampler rootSampler = rootSamplerType switch
        {
            nameof(AlwaysOnSampler) => new AlwaysOnSampler(),
            nameof(AlwaysOffSampler) => new AlwaysOffSampler(),
            nameof(TraceIdRatioBasedSampler) => new TraceIdRatioBasedSampler(samplingRatio),
            _ => throw new ArgumentException($"Unsupported sampler type '{rootSamplerType}'"),
        };

        services
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(tracingBuilder =>
            {
                tracingBuilder
                    .SetSampler(new ParentBasedSampler(rootSampler))
                    .AddSource(Program.ActivitySource.Name)
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(o =>
                    {
                        o.Filter = (r) =>
                        {
                            // "/ready" and "/live" are this app's actual health probe routes (see
                            // Startup.Configure) - previously listed here as "/readyz"/"/livez"/
                            // "/healthz", which never matched anything and left every health
                            // check traced.
                            var ignoredPaths = new[] { "/ready", "/live", "/fhir/metadata" };

                            var path = r.Request.Path.Value!;
                            return !ignoredPaths.Any(path.Contains);
                        };
                    });

                services.Configure<AspNetCoreTraceInstrumentationOptions>(
                    configuration.GetSection("Tracing:AspNetCoreInstrumentation")
                );

                var endpoint =
                    configuration.GetValue<string>("Tracing:Otlp:Endpoint")
                    ?? throw new ArgumentException("Missing OTLP exporter endpoint URL");

                tracingBuilder.AddOtlpExporter(otlpOptions =>
                    otlpOptions.Endpoint = new Uri(endpoint)
                );
            });

        return services;
    }
}
