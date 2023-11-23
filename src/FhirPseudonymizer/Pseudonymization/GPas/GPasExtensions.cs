using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using FhirPseudonymizer.Config;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public static class GPasExtensions
{
    public static IServiceCollection AddGPasClient(
        this IServiceCollection services,
        GPasConfig gPasConfig
    )
    {
        if (string.IsNullOrWhiteSpace(gPasConfig.Url?.AbsoluteUri))
        {
            throw new ValidationException("gPAS is enabled but the backend service URL is unset.");
        }

        var oAuthConfig = gPasConfig.Auth.OAuth;

        var isOAuthEnabled = oAuthConfig.TokenEndpoint is not null;
        if (isOAuthEnabled)
        {
            services
                .AddClientCredentialsTokenManagement()
                .AddClient(
                    $"{GPasFhirClient.HttpClientName}.oAuth.client",
                    client =>
                    {
                        client.TokenEndpoint = oAuthConfig.TokenEndpoint.AbsoluteUri;

                        client.ClientId = oAuthConfig.ClientId;
                        client.ClientSecret = oAuthConfig.ClientSecret;

                        client.Scope = oAuthConfig.Scope;
                        client.Resource = oAuthConfig.Resource;
                    }
                );
        }

        IHttpClientBuilder clientBuilder = null;
        if (isOAuthEnabled)
        {
            clientBuilder = services.AddClientCredentialsHttpClient(
                GPasFhirClient.HttpClientName,
                $"{GPasFhirClient.HttpClientName}.oAuth.client",
                client =>
                {
                    client.BaseAddress = gPasConfig.Url;
                }
            );
        }
        else
        {
            clientBuilder = services.AddHttpClient(
                GPasFhirClient.HttpClientName,
                (client) =>
                {
                    client.BaseAddress = gPasConfig.Url;

                    if (!string.IsNullOrEmpty(gPasConfig.Auth.Basic.Username))
                    {
                        var basicAuthString =
                            $"{gPasConfig.Auth.Basic.Username}:{gPasConfig.Auth.Basic.Password}";
                        var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(byteArray)
                        );
                    }
                }
            );
        }

        clientBuilder
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy(gPasConfig.RequestRetryCount))
            .UseHttpClientMetrics();

        services.AddTransient<IPseudonymServiceClient, GPasFhirClient>();

        return services;
    }

    private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }
}
