using System.Security.Claims;
using System.Text.Json;
using FhirPseudonymizer.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace FhirPseudonymizer;

public static class AuthExtensions
{
    /// <summary>
    ///     Guards the operations that revert de-identification: <c>$re-identify</c> and its
    ///     deprecated <c>$de-pseudonymize</c> alias. Requires a JWT that passes
    ///     <see cref="JwtAuthConfig" />'s checks.
    /// </summary>
    public const string ReIdentifyPolicy = "ReIdentify";

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        AuthConfig authConfig
    )
    {
        var jwtConfig = authConfig.Jwt;
        if (jwtConfig.IsEnabled && string.IsNullOrWhiteSpace(jwtConfig.Audience))
        {
            throw new InvalidOperationException(
                "Auth:Jwt:Audience must be set if Auth:Jwt:Authority is. Without it, tokens "
                    + "the issuer handed out for any of its other clients would be accepted."
            );
        }

        // Registered even if no Authority is set: without any signing keys to validate them
        // against, every token is then rejected, so the protected operations stay locked rather
        // than falling back to having no authentication at all.
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                if (!jwtConfig.IsEnabled)
                {
                    return;
                }

                options.Authority = jwtConfig.Authority;
                options.Audience = jwtConfig.Audience;
                if (!string.IsNullOrWhiteSpace(jwtConfig.MetadataAddress))
                {
                    options.MetadataAddress = jwtConfig.MetadataAddress;
                }
                options.RequireHttpsMetadata = jwtConfig.RequireHttpsMetadata;

                // Keep the claims named as in the token (scope, roles, ...) instead of mapping
                // them onto the legacy WS-Federation claim type URIs.
                options.MapInboundClaims = false;
            });

        services
            .AddAuthorizationBuilder()
            .AddPolicy(
                ReIdentifyPolicy,
                policy =>
                    policy
                        .RequireAuthenticatedUser()
                        .RequireAssertion(context =>
                            (
                                string.IsNullOrWhiteSpace(jwtConfig.RequiredScope)
                                || HasScope(context.User, jwtConfig.RequiredScope)
                            )
                            && (
                                string.IsNullOrWhiteSpace(jwtConfig.RequiredRole)
                                || HasRole(context.User, jwtConfig.RequiredRole)
                            )
                        )
            );

        return services;
    }

    public static void LogInformationIfReIdentifyIsDisabled(AuthConfig authConfig, ILogger logger)
    {
        if (!authConfig.Jwt.IsEnabled)
        {
            logger.LogInformation(
                "Auth:Jwt:Authority is not set, so $re-identify rejects every request."
            );
        }
    }

    // Scopes are a single space-separated claim - "scope" per RFC 9068, "scp" in Entra ID.
    private static bool HasScope(ClaimsPrincipal user, string scope) =>
        user.FindAll(claim => claim.Type is "scope" or "scp")
            .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(scope, StringComparer.Ordinal);

    // A JSON array claim, such as "roles", becomes one claim per element. Keycloak instead puts
    // realm roles in an object, {"roles": [...]}, which arrives as a single claim of raw JSON.
    private static bool HasRole(ClaimsPrincipal user, string role) =>
        user.FindAll("roles").Any(claim => claim.Value == role)
        || user.FindAll("realm_access").Any(claim => RealmAccessHasRole(claim.Value, role));

    private static bool RealmAccessHasRole(string realmAccessJson, string role)
    {
        try
        {
            using var realmAccess = JsonDocument.Parse(realmAccessJson);
            return realmAccess.RootElement.ValueKind == JsonValueKind.Object
                && realmAccess.RootElement.TryGetProperty("roles", out var roles)
                && roles.ValueKind == JsonValueKind.Array
                && roles
                    .EnumerateArray()
                    .Any(r => r.ValueKind == JsonValueKind.String && r.GetString() == role);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
