using System.Reflection;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Projects;
using FhirPseudonymizer.Pseudonymization;
using FhirPseudonymizer.Pseudonymization.Entici;
using FhirPseudonymizer.Pseudonymization.GPas;
using FhirPseudonymizer.Pseudonymization.Mii;
using FhirPseudonymizer.Pseudonymization.Vfps;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
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
        appConfig = appConfig with
        {
            Kafka = appConfig.Kafka with
            {
                Topics = KafkaExtensions.NormalizeTopics(Configuration, appConfig.Kafka.Topics),
            },
        };

        if (appConfig.EnableMetrics)
        {
            services.AddMetricServer(options => options.Port = appConfig.MetricsPort);
        }

        services.AddSingleton(_ => appConfig);
        services.AddSingleton(_ => appConfig.GPas);
        services.AddSingleton(_ => appConfig.Features);
        services.AddSingleton(_ => appConfig.Anonymization);

        services.AddApiKeyAuth(appConfig.ApiKey);

        // A deployment with neither a config of its own nor a directory of Project configs can
        // serve nothing, so it fails fast here rather than 400-ing every request that arrives.
        if (
            !AnonymizerEngineExtensions.HasStartupConfig(appConfig)
            && string.IsNullOrEmpty(appConfig.ProjectConfigsDirectory)
        )
        {
            throw new InvalidOperationException(
                "No anonymization config is set. Set AnonymizationEngineConfigPath or "
                    + "AnonymizationEngineConfigInline for a server config, and/or "
                    + "ProjectConfigsDirectory for per-project configs. With both blank this server "
                    + "could serve no request."
            );
        }

        // Required by the OAuth implementation. Could be used for all cache implementations later.
        services.AddDistributedMemoryCache();
        var cacheConfig = appConfig.Cache;
        services.AddSingleton(cacheConfig);
        services.AddSingleton<IMemoryCache>(_ => new MemoryCache(
            new MemoryCacheOptions { SizeLimit = cacheConfig.SizeLimit }
        ));

        switch (appConfig.PseudonymizationService)
        {
            case PseudonymizationServiceType.gPAS:
                services.AddGPasClient(appConfig.GPas);
                break;
            case PseudonymizationServiceType.Vfps:
                services.AddVfpsClient(appConfig.Vfps);
                break;
            case PseudonymizationServiceType.entici:
                services.AddEnticiClient(appConfig.Entici);
                break;
            case PseudonymizationServiceType.Mii:
                services.AddMiiClient(appConfig.Mii);
                break;
            case PseudonymizationServiceType.None:
                services.AddTransient<IPseudonymServiceClient, NoopPseudonymServiceClient>();
                break;
        }

        services.AddSingleton<IAnonymizerEngineFactory, AnonymizerEngineFactory>();

        // Constructed by hand because it owns its own MemoryCache and CacheConfig, which the
        // container cannot tell apart from the pseudonym cache's by type.
        services.AddSingleton<IProjectConfigProvider>(sp => new FileProjectConfigProvider(
            appConfig.ProjectConfigsDirectory,
            sp.GetRequiredService<IAnonymizerEngineFactory>(),
            appConfig.ProjectCache
        ));

        services.AddAnonymizerEngine(appConfig);

        services.AddSingleton(_ => appConfig.Kafka);

        var provenanceEnabled = !string.IsNullOrWhiteSpace(appConfig.Kafka.ProvenanceTopic);
        if (appConfig.Kafka.Topics.Count > 0 || provenanceEnabled)
        {
            services.AddKafkaProducer(appConfig.Kafka);
        }

        if (appConfig.Kafka.Topics.Count > 0)
        {
            // A consumed message names no project, and there is no caller to answer nor client to
            // re-register the config it would need — so the consumer can only run the server's own
            // rules. Refuse the ambiguous combination rather than start a consumer with no engine.
            if (!AnonymizerEngineExtensions.HasStartupConfig(appConfig))
            {
                throw new InvalidOperationException(
                    "Kafka topics are configured to be consumed, but this server was started "
                        + "without an anonymization config of its own. Messages consumed from Kafka "
                        + "name no project and can only be anonymized with the server's own config. "
                        + "Set AnonymizationEngineConfigPath or AnonymizationEngineConfigInline, or "
                        + "stop configuring Kafka:Topics."
                );
            }

            services.AddKafkaConsumer(appConfig.Kafka);
        }

        if (provenanceEnabled)
        {
            services.AddSingleton<IProvenancePublisher, KafkaProvenancePublisher>();
        }
        else
        {
            services.AddSingleton<IProvenancePublisher, NoopProvenancePublisher>();
        }

        services.AddRequestDecompression();

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
        // Announced at startup because blanking the config settings is the only thing separating
        // this mode from a misconfiguration that would otherwise have refused to start.
        if (app.ApplicationServices.GetRequiredService<ServerEngines>().Engines is null)
        {
            app.ApplicationServices.GetRequiredService<ILogger<Startup>>()
                .LogWarning(
                    "Started without an anonymization config of its own. Every $de-identify request "
                        + "must name a project via a 'project' parameter; requests naming none are "
                        + "refused."
                );
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRequestDecompression();

        app.UseResponseCompression();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "FhirPseudonymizer v2")
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
