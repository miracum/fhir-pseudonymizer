using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Duende.AccessTokenManagement;
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
                        if (
                            oAuthConfig.TokenEndpoint is null
                            || oAuthConfig.ClientId is null
                            || oAuthConfig.ClientSecret is null
                        )
                        {
                            return;
                        }

                        client.TokenEndpoint = oAuthConfig.TokenEndpoint;
                        client.ClientId = ClientId.Parse(oAuthConfig.ClientId);
                        client.ClientSecret = ClientSecret.Parse(oAuthConfig.ClientSecret);

                        if (oAuthConfig.Scope is not null)
                        {
                            client.Scope = Scope.Parse(oAuthConfig.Scope);
                        }

                        if (oAuthConfig.Resource is not null)
                        {
                            client.Resource = Resource.Parse(oAuthConfig.Resource);
                        }
                    }
                );
        }

        IHttpClientBuilder clientBuilder = null;
        if (isOAuthEnabled)
        {
            clientBuilder = services.AddClientCredentialsHttpClient(
                GPasFhirClient.HttpClientName,
                ClientCredentialsClientName.Parse($"{GPasFhirClient.HttpClientName}.oAuth.client"),
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
