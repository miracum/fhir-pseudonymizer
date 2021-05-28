using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FhirPseudonymizer
{
    public static class Program
    {
        internal static ActivitySource ActivitySource { get; }
            = new ActivitySource("FhirPseudonymizer", typeof(Program).Assembly.GetName().Version.ToString());

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureLogging(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.UseUtcTimestamp = true;
                        options.IncludeScopes = true;
                        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ ";
                        options.SingleLine = true;
                    }));
        }
    }
}
