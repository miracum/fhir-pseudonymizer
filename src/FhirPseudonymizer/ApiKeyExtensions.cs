using System.Security.Claims;
using AspNetCore.Authentication.ApiKey;

namespace FhirPseudonymizer;

public static class ApiKeyExtensions
{
    /// <summary>
    ///     Guards Project registration: a Config carries crypto keys, and unbounded registration
    ///     is a memory-exhaustion vector.
    /// </summary>
    public const string ProjectRegistrationPolicy = "ProjectRegistration";

    /// <summary>
    ///     Requires the API key for Project registration, but only when one is configured, so a
    ///     deployment that sets no key stays reachable.
    /// </summary>
    public static IServiceCollection AddProjectRegistrationAuth(
        this IServiceCollection services,
        string apiKey
    )
    {
        return services.AddAuthorization(options =>
            options.AddPolicy(
                ProjectRegistrationPolicy,
                policy =>
                {
                    if (string.IsNullOrWhiteSpace(apiKey))
                    {
                        policy.RequireAssertion(_ => true);
                        return;
                    }

                    policy.AddAuthenticationSchemes(ApiKeyDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                }
            )
        );
    }

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
