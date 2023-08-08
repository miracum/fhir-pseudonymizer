using System.Net.Http.Headers;
using System.Text;
using FhirPseudonymizer.Config;
using Polly;
using Polly.Extensions.Http;
using Prometheus;

namespace FhirPseudonymizer.Pseudonymization.GPas;

public static class GPasExtensions
{
    public static IServiceCollection AddGPasClient(
        this IServiceCollection services,
        GPasConfig gPasConfig
    )
    {
        var oAuthConfig = gPasConfig.Auth.OAuth;
        if (oAuthConfig.TokenUrl is not null)
        {
            services
                .AddClientCredentialsTokenManagement()
                .AddClient(
                    "gPAS.oauth.client",
                    client =>
                    {
                        client.TokenEndpoint = oAuthConfig.TokenUrl.AbsoluteUri;

                        client.ClientId = oAuthConfig.ClientId;
                        client.ClientSecret = oAuthConfig.ClientSecret;

                        client.Scope = oAuthConfig.Scope;
                        client.Resource = oAuthConfig.Resource;
                    }
                );
        }

        // TODO: use either this or basic auth depending on the settings. or even both?
        IHttpClientBuilder clientBuilder = null;
        clientBuilder = services.AddClientCredentialsHttpClient(
            "gPAS",
            "gPAS.oauth.client",
            client =>
            {
                client.BaseAddress = gPasConfig.Url;
            }
        );

        clientBuilder = services.AddHttpClient(
            "gPAS",
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

        clientBuilder
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
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
            );
    }
}
