using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Polly.Extensions.Http;
using Prometheus;

namespace FhirPseudonymizer
{
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
            // services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options =>
            {
                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[] { "application/fhir+json" });
            });

            var apiKey = Configuration.GetValue<string>("ApiKey");
            services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
                .AddApiKeyInHeaderOrQueryParams(options =>
                {
                    options.Realm = "FHIR Pseudonymizer";
                    options.KeyName = "X-Api-Key";
                    options.IgnoreAuthenticationIfAllowAnonymous = true;

                    options.Events = new ApiKeyEvents
                    {
                        OnValidateKey = ctx =>
                        {
                            if (string.IsNullOrWhiteSpace(apiKey) || !apiKey.Equals(ctx.ApiKey, StringComparison.InvariantCulture))
                            {
                                ctx.ValidationFailed();
                                return Task.CompletedTask;
                            }

                            var claims = new[]
                               {
                                   new Claim("ApiAccess", "Access to FHIR Pseudonymizer API")
                               };
                            ctx.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ctx.Scheme.Name));
                            ctx.Success();
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient("gPAS", client =>
                {
                    client.BaseAddress = new Uri(Configuration["gPAS:Url"]);

                    if (Configuration["gPAS:Auth:Basic:Username"] != null)
                    {
                        var basicAuthString = $"{Configuration["gPAS:Auth:Basic:Username"]}:{Configuration["gPAS:Auth:Basic:Password"]}";
                        var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(byteArray));
                    }
                }).SetHandlerLifetime(TimeSpan.FromMinutes(5))
                  .AddPolicyHandler(GetRetryPolicy(Configuration.GetValue<int>("gPAS:RequestRetryCount")))
                  .UseHttpClientMetrics();

            services.AddTransient<IGPasFhirClient, GPasFhirClient>();

            AnonymizerEngine.InitializeFhirPathExtensionSymbols();

            var anonConfigManager = AnonymizerConfigurationManager.CreateFromYamlConfigFile(Configuration["AnonymizationEngineConfigPath"]);
            // add the anon config as an additional service to allow mocking it
            services.AddSingleton(_ => anonConfigManager);

            services.AddSingleton<IAnonymizerEngine>(sp =>
            {
                var anonConfig = sp.GetService<AnonymizerConfigurationManager>();
                var engine = new AnonymizerEngine(anonConfig);

                if (!string.IsNullOrWhiteSpace(Configuration["gPAS:Url"]))
                {
                    var gpasFhirClient = sp.GetService<IGPasFhirClient>();
                    engine.AddProcessor("pseudonymize", new GPasPseudonymizationProcessor(gpasFhirClient));
                }

                return engine;
            });

            services.AddSingleton<IDePseudonymizerEngine>(sp =>
            {
                var anonConfig = sp.GetService<AnonymizerConfigurationManager>();
                var engine = new DePseudonymizerEngine(anonConfig);

                if (!string.IsNullOrWhiteSpace(Configuration["gPAS:Url"]))
                {
                    var gpasFhirClient = sp.GetService<IGPasFhirClient>();
                    engine.AddProcessor("pseudonymize", new GPasDePseudonymizationProcessor(gpasFhirClient));
                }

                engine.AddProcessor("encrypt", new DecryptProcessor(anonConfig.GetParameterConfiguration().EncryptKey));
                return engine;
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, new FhirInputFormatter());
                options.OutputFormatters.Insert(0, new FhirOutputFormatter());
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "FHIR Pseudonymizer", Version = "v2" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHealthChecks()
                .AddCheck("live", () => HealthCheckResult.Healthy());

            if (Configuration.GetValue<bool>("Tracing:Enabled"))
            {
                services.AddOpenTelemetryTracing(builder =>
                {
                    var serviceName = Environment.GetEnvironmentVariable("JAEGER_SERVICE_NAME") ??
                        Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ??
                        Configuration.GetValue<string>("Tracing:ServiceName");

                    builder
                        .AddAspNetCoreInstrumentation(o =>
                        {
                            o.Filter = (r) =>
                            {
                                var ignoredPaths = new[]
                                {
                                     "/health",
                                     "/ready",
                                     "/live",
                                     "/fhir/metadata"
                                };

                                var path = r.Request.Path.Value;
                                return !ignoredPaths.Any(path.Contains);
                            };
                        })
                        .AddSource(Program.ActivitySource.Name)
                        .AddHttpClientInstrumentation()
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                        .AddJaegerExporter();
                });

                services.Configure<JaegerExporterOptions>(Configuration.GetSection("Tracing:Jaeger"));
            }
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v2/swagger.json", "FhirPseudonymizer v2"));

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseHealthChecks("/ready");
            app.UseHealthChecks("/live", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("live", StringComparison.InvariantCultureIgnoreCase),
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
        }
    }
}
