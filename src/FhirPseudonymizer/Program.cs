using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace FhirPseudonymizer
{
    public static class Program
    {
        // Falls back to the OpenTelemetry SDK's own default ("unknown_service:...") behavior only
        // if this assembly somehow has no version, which doesn't happen outside of that fallback.
        internal static string ServiceVersion { get; } =
            typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown";

        // OTEL_SERVICE_NAME is the standard env var every OTel SDK/collector already knows about,
        // so it's read directly here rather than exposing a separate app-specific setting for the
        // same thing. Resource attributes set this way still lose to an explicit
        // ResourceBuilder.AddService() call regardless of which one runs first - AddService always
        // wins - so this has to be the one and only place that calls it, with the env var as input.
        internal static string ServiceName { get; } =
            Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "fhir-pseudonymizer";

        internal static ActivitySource ActivitySource { get; } =
            new ActivitySource("FhirPseudonymizer", ServiceVersion);

        internal static Meter Meter { get; } = new Meter("FhirPseudonymizer");

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AddSecretsDirectory)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.UseUtcTimestamp = true;
                        options.IncludeScopes = true;
                        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ ";
                    })
                );
        }

        public static void AddSecretsDirectory(
            HostBuilderContext context,
            IConfigurationBuilder config
        )
        {
            var secretsDirectory = context.Configuration.GetValue(
                "SecretsDirectory",
                "/run/secrets"
            );
            config.AddKeyPerFile(secretsDirectory, optional: true, reloadOnChange: true);
        }
    }
}
