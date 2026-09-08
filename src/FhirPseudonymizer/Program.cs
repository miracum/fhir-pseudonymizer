using System.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace FhirPseudonymizer
{
    public static class Program
    {
        internal static ActivitySource ActivitySource { get; } =
            new ActivitySource(
                "FhirPseudonymizer",
                typeof(Program).Assembly.GetName().Version.ToString()
            );

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(AddSecretsDirectory)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>().ConfigureKestrel(ConfigureMaxRequestBodySize)
                )
                .ConfigureLogging(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.UseUtcTimestamp = true;
                        options.IncludeScopes = true;
                        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ ";
                    })
                );
        }

        public static void ConfigureMaxRequestBodySize(
            WebHostBuilderContext context,
            KestrelServerOptions options
        )
        {
            var maxRequestBodySize = context.Configuration.GetValue<long?>("MaxRequestBodySize");
            if (maxRequestBodySize.HasValue)
            {
                options.Limits.MaxRequestBodySize = maxRequestBodySize;
            }
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
