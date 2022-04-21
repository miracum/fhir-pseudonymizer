using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Utility;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace FhirPseudonymizer
{
    public interface IGPasFhirClient
    {
        Task<string> GetOriginalValueFor(string pseudonym, string domain);
        Task<string> GetOrCreatePseudonymFor(string value, string domain);
    }

    public class GPasFhirClient : IGPasFhirClient
    {
        private static readonly Counter TotalGPasRequests = Metrics
            .CreateCounter("fhirpseudonymizer_gpas_requests_total",
                "Total number of requests against the gPas service.",
                new CounterConfiguration()
                {
                    LabelNames = new[] { "operation" },
                });

        private static readonly Counter TotalGPasRequestCacheMisses = Metrics
            .CreateCounter("fhirpseudonymizer_gpas_requests_cache_misses_total",
                "Total number of requests against gPas that could not be resolved via the internal cache.",
                new CounterConfiguration()
                {
                    LabelNames = new[] { "operation" },
                });

        private readonly ILogger<GPasFhirClient> logger;

        public GPasFhirClient(ILogger<GPasFhirClient> logger, IHttpClientFactory clientFactory, IConfiguration config)
        {
            this.logger = logger;

            Config = config;
            Client = clientFactory.CreateClient("gPAS");

            PseudonymCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = config.GetValue<long>("Cache:SizeLimit")
            });

            OriginalValueCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = config.GetValue<long>("Cache:SizeLimit")
            });

            SlidingExpiration = TimeSpan.FromMinutes(Config.GetValue<double>("Cache:SlidingExpirationMinutes", 5));
            AbsoluteExpiration = TimeSpan.FromMinutes(Config.GetValue<double>("Cache:AbsoluteExpirationMinutes", 10));

            var configGPasVersion = config.GetValue<string>("gPAS:Version");
            var supportedGPasVersion = SemVersion.Parse(configGPasVersion);

            if (supportedGPasVersion < SemVersion.Parse("1.10.2"))
            {
                GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV1;
                GetOriginalValueForResolver = GetOriginalValueForV1;
            }
            else if (supportedGPasVersion == SemVersion.Parse("1.10.2"))
            {
                GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV2;
                GetOriginalValueForResolver = GetOriginalValueForV2;
            }
            else
            {
                GetOrCreatePseudonymForResolver = GetOrCreatePseudonymForV2x;
                GetOriginalValueForResolver = GetOriginalValueForV2x;
            }

        }

        private IConfiguration Config { get; }
        private IMemoryCache PseudonymCache { get; }
        private IMemoryCache OriginalValueCache { get; }
        private HttpClient Client { get; }
        private FhirJsonParser FhirParser { get; } = new();
        private FhirJsonSerializer FhirSerializer { get; } = new();
        private TimeSpan SlidingExpiration { get; }
        private TimeSpan AbsoluteExpiration { get; }
        private Func<string, string, Task<string>> GetOrCreatePseudonymForResolver { get; }
        private Func<string, string, Task<string>> GetOriginalValueForResolver { get; }

        public async Task<string> GetOrCreatePseudonymFor(string value, string domain)
        {
            TotalGPasRequests.WithLabels(nameof(GetOrCreatePseudonymFor)).Inc();

            return await PseudonymCache.GetOrCreateAsync((value, domain), async entry =>
            {
                TotalGPasRequestCacheMisses.WithLabels(nameof(GetOrCreatePseudonymFor)).Inc();

                entry.SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug("Getting or creating pseudonym for {value} in {domain}", value, domain);

                return await GetOrCreatePseudonymForResolver(value, domain);
            });
        }

        public async Task<string> GetOriginalValueFor(string pseudonym, string domain)
        {
            TotalGPasRequests.WithLabels(nameof(GetOriginalValueFor)).Inc();

            return await OriginalValueCache.GetOrCreateAsync((pseudonym, domain), async entry =>
            {
                TotalGPasRequestCacheMisses.WithLabels(nameof(GetOriginalValueFor)).Inc();

                entry.SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug("Getting original value for pseudonym {pseudonym} from {domain}", pseudonym, domain);

                return await GetOriginalValueForResolver(pseudonym, domain);
            });
        }

        public async Task<string> GetOriginalValueForV1(string pseudonym, string domain)
        {
            var query = new Dictionary<string, string> { ["domain"] = domain, ["pseudonym"] = pseudonym };

            var response = await Client.GetAsync(QueryHelpers.AddQueryString("$de-pseudonymize", query));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var parameters = FhirParser.Parse<Parameters>(content);

            var original = parameters.GetSingleValue<FhirString>(pseudonym);
            if (original == null)
            {
                logger.LogWarning("Failed to de-pseudonymize. Returning original value.", pseudonym);
                return pseudonym;
            }

            return original.Value;
        }

        public async Task<string> GetOriginalValueForV2(string pseudonym, string domain)
        {
            var parameters = new Parameters()
                .Add("target", new FhirString(domain))
                .Add("pseudonym", new FhirString(pseudonym));

            var parametersBody = FhirSerializer.SerializeToString(parameters);
            using var content = new StringContent(parametersBody, Encoding.UTF8, "application/fhir+json");

            try
            {
                var response = await Client.PostAsync("$de-pseudonymize", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseParameters = FhirParser.Parse<Parameters>(responseContent);

                var pseudonymResultSet = responseParameters.Get("pseudonym-result-set").First();
                var originalPart = pseudonymResultSet.Part.Find(component => component.Name == "original");

                return originalPart.Value.ToString();
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Failed to de-pseudonymize. Returning original value.");
                return pseudonym;
            }
        }

        public async Task<string> GetOriginalValueForV2x(string pseudonym, string domain)
        {
            var parameters = new Parameters()
                .Add("target", new FhirString(domain))
                .Add("pseudonym", new FhirString(pseudonym));

            var parametersBody = FhirSerializer.SerializeToString(parameters);
            using var content = new StringContent(parametersBody, Encoding.UTF8, "application/fhir+json");

            var response = await Client.PostAsync("$dePseudonymize", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseParameters = FhirParser.Parse<Parameters>(responseContent);

            var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
            var original = firstResponseParameter?.Part.Find(part => part.Name == "original");
            var originalIdentifier = original?.Value as Identifier;
            if (originalIdentifier == null)
            {
                logger.LogWarning("Failed to de-pseudonymize. Returning original value.", pseudonym);
                return pseudonym;
            }

            return originalIdentifier.Value.ToString();
        }

        private async Task<string> GetOrCreatePseudonymForV1(string value, string domain)
        {
            var query = new Dictionary<string, string> { ["domain"] = domain, ["original"] = value };

            // this currently uses a HttpClient instead of the FhirClient to leverage
            // Polly, tracing, and metrics support. Once FhirClient allows for overring the HttpClient,
            // we can simplify this code a lot: https://github.com/FirelyTeam/firely-net-sdk/issues/1483
            var response = await Client.GetAsync(QueryHelpers.AddQueryString("$pseudonymize-allow-create", query));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var parameters = FhirParser.Parse<Parameters>(content);
            return parameters.GetSingleValue<FhirString>(value).Value;
        }

        private async Task<string> GetOrCreatePseudonymForV2(string value, string domain)
        {
            var parameters = new Parameters()
                .Add("target", new FhirString(domain))
                .Add("original", new FhirString(value));

            var parametersBody = FhirSerializer.SerializeToString(parameters);
            using var content = new StringContent(parametersBody, Encoding.UTF8, "application/fhir+json");
            var response = await Client.PostAsync("$pseudonymize-allow-create", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseParameters = FhirParser.Parse<Parameters>(responseContent);

            var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
            var pseudonym = firstResponseParameter?.Part.Find(part => part.Name == "pseudonym");
            if (pseudonym?.Value == null)
            {
                throw new InvalidOperationException("No pseudonym included in gPAS reponse.");
            }

            return pseudonym.Value.ToString();
        }

        private async Task<string> GetOrCreatePseudonymForV2x(string value, string domain)
        {
            var parameters = new Parameters()
                .Add("target", new FhirString(domain))
                .Add("original", new FhirString(value));

            var parametersBody = FhirSerializer.SerializeToString(parameters);
            using var content = new StringContent(parametersBody, Encoding.UTF8, "application/fhir+json");
            var response = await Client.PostAsync("$pseudonymizeAllowCreate", content);
            logger.LogDebug("Request to $pseudonymizeAllowCreate responded with: " + response);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseParameters = FhirParser.Parse<Parameters>(responseContent);

            var firstResponseParameter = responseParameters.Parameter.FirstOrDefault();
            var pseudonym = firstResponseParameter?.Part.Find(part => part.Name == "pseudonym");
            var pseudonymIdentifier = pseudonym?.Value as Identifier;
            if (pseudonymIdentifier == null)
            {
                throw new InvalidOperationException("No pseudonym included in gPAS reponse.");
            }

            return pseudonymIdentifier.Value.ToString();
        }
    }
}
