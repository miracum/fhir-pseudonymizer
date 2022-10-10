using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FhirPseudonymizer.Config;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public static class GPasExtensions
{
    public static IServiceCollection AddGPasClient(this IServiceCollection services, GPasConfig gPasConfig)
    {
        services.AddHttpClient("gPAS", (client) =>
            {
                client.BaseAddress = gPasConfig.Url;

                if (!string.IsNullOrEmpty(gPasConfig.Auth.Basic.Username))
                {
                    var basicAuthString = $"{gPasConfig.Auth.Basic.Username}:{gPasConfig.Auth.Basic.Password}";
                    var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                }
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy(gPasConfig.RequestRetryCount))
            .UseHttpClientMetrics();

        services.AddTransient<IPseudonymServiceClient, GPasFhirClient>();

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
