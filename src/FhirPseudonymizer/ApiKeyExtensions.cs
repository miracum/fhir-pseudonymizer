using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.DependencyInjection;

namespace FhirPseudonymizer;

public static class ApiKeyExtensions
{
    public static IServiceCollection AddApiKeyAuth(this IServiceCollection services, string apiKey)
    {
        services
            .AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
            .AddApiKeyInHeaderOrQueryParams(options =>
            {
                options.Realm = "FHIR Pseudonymizer";
                options.KeyName = "X-Api-Key";
                options.IgnoreAuthenticationIfAllowAnonymous = true;

                options.Events = new ApiKeyEvents
                {
                    OnValidateKey = ctx =>
                    {
                        if (
                            string.IsNullOrWhiteSpace(apiKey)
                            || !apiKey.Equals(ctx.ApiKey, StringComparison.InvariantCulture)
                        )
                        {
                            ctx.ValidationFailed();
                            return Task.CompletedTask;
                        }

                        var claims = new[]
                        {
                            new Claim("ApiAccess", "Access to FHIR Pseudonymizer API"),
                        };

                        ctx.Principal = new ClaimsPrincipal(
                            new ClaimsIdentity(claims, ctx.Scheme.Name)
                        );
                        ctx.Success();
                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }
}
