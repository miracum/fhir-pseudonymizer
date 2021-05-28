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

namespace FhirPseudonymizer
{
    public interface IGPasFhirClient
    {
        Task<string> GetOriginalValueFor(string pseudonym, string domain);
        Task<string> GetOrCreatePseudonymFor(string value, string domain);
    }

    public class GPasFhirClient : IGPasFhirClient
    {
        private readonly ILogger<GPasFhirClient> logger;

        private readonly bool useGpasV2FhirApi;

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
            if (supportedGPasVersion >= SemVersion.Parse("1.10.2"))
            {
                logger.LogInformation("Using gPAS >= 1.10.2 FHIR API. Configured gPAS version: {gpasVersion}",
                    configGPasVersion);
                useGpasV2FhirApi = true;
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

        public async Task<string> GetOrCreatePseudonymFor(string value, string domain)
        {
            return await PseudonymCache.GetOrCreateAsync((value, domain), async entry =>
            {
                entry.SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug("Getting or creating pseudonym for {value} in {domain}", value, domain);

                if (useGpasV2FhirApi)
                {
                    return await GetOrCreatePseudonymForV2(value, domain);
                }

                return await GetOrCreatePseudonymForV1(value, domain);
            });
        }

        public async Task<string> GetOriginalValueFor(string pseudonym, string domain)
        {
            return await OriginalValueCache.GetOrCreateAsync((pseudonym, domain), async entry =>
            {
                entry.SetSize(1)
                    .SetSlidingExpiration(SlidingExpiration)
                    .SetAbsoluteExpiration(AbsoluteExpiration);

                logger.LogDebug("Getting original value for pseudonym {pseudonym} from {domain}", pseudonym, domain);

                if (useGpasV2FhirApi)
                {
                    return await GetOriginalValueForV2(pseudonym, domain);
                }

                return await GetOriginalValueForV1(pseudonym, domain);
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

                return responseParameters.GetSingleValue<FhirString>(pseudonym).Value;
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Failed to de-pseudonymize. Returning original value.");
                return pseudonym;
            }
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
            var content = new StringContent(parametersBody, Encoding.UTF8, "application/fhir+json");
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
    }
}
