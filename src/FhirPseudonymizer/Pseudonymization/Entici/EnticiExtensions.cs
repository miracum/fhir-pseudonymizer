using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Duende.AccessTokenManagement;
using FhirPseudonymizer.Config;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization.Entici;

public static class EnticiExtensions
{
    public static IServiceCollection AddEnticiClient(
        this IServiceCollection services,
        EnticiConfig enticiConfig
    )
    {
        if (string.IsNullOrWhiteSpace(enticiConfig.Url?.AbsoluteUri))
        {
            throw new ValidationException(
                "entici is enabled but the backend service URL is unset."
            );
        }

        var oAuthConfig = enticiConfig.Auth.OAuth;

        var isOAuthEnabled = oAuthConfig.TokenEndpoint is not null;
        if (isOAuthEnabled)
        {
            services
                .AddClientCredentialsTokenManagement()
                .AddClient(
                    $"{EnticiFhirClient.HttpClientName}.oAuth.client",
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
                EnticiFhirClient.HttpClientName,
                ClientCredentialsClientName.Parse(
                    $"{EnticiFhirClient.HttpClientName}.oAuth.client"
                ),
                client =>
                {
                    client.BaseAddress = enticiConfig.Url;
                }
            );
        }
        else
        {
            clientBuilder = services.AddHttpClient(
                EnticiFhirClient.HttpClientName,
                (client) =>
                {
                    client.BaseAddress = enticiConfig.Url;

                    if (!string.IsNullOrEmpty(enticiConfig.Auth.Basic.Username))
                    {
                        var basicAuthString =
                            $"{enticiConfig.Auth.Basic.Username}:{enticiConfig.Auth.Basic.Password}";
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
            .AddPolicyHandler(GetRetryPolicy(enticiConfig.RequestRetryCount))
            .UseHttpClientMetrics();

        services.AddTransient<IPseudonymServiceClient, EnticiFhirClient>();

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
