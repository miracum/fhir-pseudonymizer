using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using FhirPseudonymizer.Config;
using Polly;
using Polly.Extensions.Http;
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
                    "Entici.oAuth.client",
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
                "Entici",
                "Entici.oAuth.client",
                client =>
                {
                    client.BaseAddress = enticiConfig.Url;
                }
            );
        }
        else
        {
            clientBuilder = services.AddHttpClient(
                "Entici",
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

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount = 3)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }
}
