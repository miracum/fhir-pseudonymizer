using System.Reflection;
using OpenTelemetry;
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
        var assembly = Assembly.GetExecutingAssembly().GetName();
        var assemblyVersion = assembly.Version?.ToString() ?? "unknown";
        var tracingExporter =
            configuration.GetValue<string>("Tracing:Exporter")?.ToLowerInvariant() ?? "jaeger";
        var serviceName = configuration.GetValue("Tracing:ServiceName", assembly.Name) ?? "fhir-pseudonymizer";

        // Build a resource configuration action to set service information.
        void configureResource(ResourceBuilder r) =>
            r.AddService(
                serviceName: serviceName,
                serviceVersion: assemblyVersion,
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
                            var ignoredPaths = new[]
                            {
                                "/healthz",
                                "/readyz",
                                "/livez",
                                "/fhir/metadata"
                            };

                            var path = r.Request.Path.Value!;
                            return !ignoredPaths.Any(path.Contains);
                        };
                    });

                services.Configure<AspNetCoreInstrumentationOptions>(
                    configuration.GetSection("Tracing:AspNetCoreInstrumentation")
                );

                switch (tracingExporter)
                {
                    case "jaeger":
                        tracingBuilder.AddJaegerExporter();
                        services.Configure<JaegerExporterOptions>(
                            configuration.GetSection("Tracing:Jaeger")
                        );
                        break;

                    case "otlp":
                        var endpoint =
                            configuration.GetValue<string>("Tracing:Otlp:Endpoint")
                            ?? throw new ArgumentException("Missing OTLP exporter endpoint URL");

                        tracingBuilder.AddOtlpExporter(
                            otlpOptions => otlpOptions.Endpoint = new Uri(endpoint)
                        );
                        break;
                }
            });

        return services;
    }
}
