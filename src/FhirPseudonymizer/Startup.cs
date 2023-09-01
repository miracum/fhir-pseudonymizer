using System.Reflection;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.Entici;
using FhirPseudonymizer.Pseudonymization.GPas;
using FhirPseudonymizer.Pseudonymization.Vfps;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Prometheus;

namespace FhirPseudonymizer;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var appConfig = new AppConfig();
        Configuration.Bind(appConfig);

        if (appConfig.EnableMetrics)
        {
            services.AddMetricServer(options => options.Port = appConfig.MetricsPort);
        }

        services.AddSingleton(_ => appConfig);
        services.AddSingleton(_ => appConfig.GPas);
        services.AddSingleton(_ => appConfig.Features);

        services.AddApiKeyAuth(appConfig.ApiKey);

        // Required by the OAuth implementation. Could be used for all cache implementations later.
        services.AddDistributedMemoryCache();

        switch (appConfig.PseudonymizationService)
        {
            case PseudonymizationServiceType.gPAS:
                services.AddSingleton<IMemoryCache>(
                    _ =>
                        new MemoryCache(
                            new MemoryCacheOptions { SizeLimit = appConfig.GPas.Cache.SizeLimit }
                        )
                );
                services.AddGPasClient(appConfig.GPas);
                break;
            case PseudonymizationServiceType.Vfps:
                services.AddVfpsClient(appConfig.Vfps);
                break;
            case PseudonymizationServiceType.entici:
                services.AddEnticiClient(appConfig.Entici);
                break;
            case PseudonymizationServiceType.None:
                services.AddTransient<IPseudonymServiceClient, NoopPseudonymServiceClient>();
                break;
        }

        services.AddAnonymizerEngine(appConfig);

        services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/fhir+json" }
            );
        });

        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddControllers(options =>
        {
            var useSystemTextJsonFhirSerializer = Configuration.GetValue(
                "UseSystemTextJsonFhirSerializer",
                false
            );
            options.InputFormatters.Insert(
                0,
                new FhirInputFormatter(useSystemTextJsonFhirSerializer)
            );
            options.OutputFormatters.Insert(
                0,
                new FhirOutputFormatter(useSystemTextJsonFhirSerializer)
            );
        });

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v2", new OpenApiInfo { Title = "FHIR Pseudonymizer", Version = "v2" });

            // Set the comments path for the Swagger JSON and UI.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services
            .AddHealthChecks()
            .AddCheck("live", () => HealthCheckResult.Healthy())
            .ForwardToPrometheus();

        var isTracingEnabled =
            Configuration.GetValue("Tracing:IsEnabled", false)
            || Configuration.GetValue("Tracing:Enabled", false);
        if (isTracingEnabled)
        {
            services.AddTracing(Configuration);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseMiddleware<RequestCompression>();
        app.UseResponseCompression();

        app.UseSwagger();
        app.UseSwaggerUI(
            c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "FhirPseudonymizer v2")
        );

        app.UseRouting();

        app.UseHttpMetrics();
        app.UseGrpcMetrics();

        app.UseHealthChecks("/ready");
        app.UseHealthChecks(
            "/live",
            new HealthCheckOptions
            {
                Predicate = r =>
                    r.Name.Contains("live", StringComparison.InvariantCultureIgnoreCase),
            }
        );

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));
            endpoints.MapControllers();
            endpoints.MapMetrics();
        });
    }
}
