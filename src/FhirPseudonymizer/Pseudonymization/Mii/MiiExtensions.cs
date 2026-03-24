using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using Duende.AccessTokenManagement;
using FhirPseudonymizer.Config;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization.Mii;

public static class MiiExtensions
{
    public static IServiceCollection AddMiiClient(
        this IServiceCollection services,
        MiiConfig miiConfig
    )
    {
        if (string.IsNullOrWhiteSpace(miiConfig.Url?.AbsoluteUri))
        {
            throw new ValidationException(
                "Mii is enabled but the backend service URL is unset."
            );
        }

        var oAuthConfig = miiConfig.Auth.OAuth;

        var isOAuthEnabled = oAuthConfig.TokenEndpoint is not null;
        if (isOAuthEnabled)
        {
            services
                .AddClientCredentialsTokenManagement()
                .AddClient(
                    $"{MiiFhirClient.HttpClientName}.oAuth.client",
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

        var clientBuilder = isOAuthEnabled
            ? services.AddClientCredentialsHttpClient(
                MiiFhirClient.HttpClientName,
                ClientCredentialsClientName.Parse(
                    $"{MiiFhirClient.HttpClientName}.oAuth.client"
                ),
                client =>
                {
                    client.BaseAddress = miiConfig.Url;
                }
            )
            : services.AddHttpClient(
                MiiFhirClient.HttpClientName,
                (client) =>
                {
                    client.BaseAddress = miiConfig.Url;

                    if (!string.IsNullOrEmpty(miiConfig.Auth.Basic.Username))
                    {
                        var basicAuthString =
                            $"{miiConfig.Auth.Basic.Username}:{miiConfig.Auth.Basic.Password}";
                        var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                            "Basic",
                            Convert.ToBase64String(byteArray)
                        );
                    }
                }
            );

        clientBuilder
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(GetRetryPolicy(miiConfig.RequestRetryCount))
            .UseHttpClientMetrics();

        services.AddTransient<IPseudonymServiceClient, MiiFhirClient>();

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
