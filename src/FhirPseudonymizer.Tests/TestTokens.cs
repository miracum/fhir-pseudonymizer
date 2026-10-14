using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Issues access tokens the way an OpenID Connect provider would, for a service configured
///     with <see cref="Issuer" /> as its Auth:Jwt:Authority - see <see cref="TrustTestIssuer" />.
/// </summary>
public static class TestTokens
{
    public const string Issuer = "https://idp.example.test/realms/test";
    public const string Audience = "fhir-pseudonymizer";

    public static readonly SecurityKey SigningKey = CreateSigningKey("test-key");

    public static AuthenticationHeaderValue BearerHeader(string token = null) =>
        new("Bearer", token ?? Create());

    public static string Create(
        string issuer = Issuer,
        string audience = Audience,
        Dictionary<string, object> claims = null,
        DateTime? expires = null,
        SecurityKey signingKey = null
    )
    {
        var expiresAt = expires ?? DateTime.UtcNow.AddHours(1);
        var issuedAt = expiresAt.AddHours(-2);

        return new JsonWebTokenHandler().CreateToken(
            new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Expires = expiresAt,
                Claims = claims ?? [],
                SigningCredentials = new SigningCredentials(
                    signingKey ?? SigningKey,
                    SecurityAlgorithms.RsaSha256
                ),
            }
        );
    }

    public static SecurityKey CreateSigningKey(string keyId) =>
        new RsaSecurityKey(RSA.Create(2048)) { KeyId = keyId };

    /// <summary>
    ///     Stands in for the issuer's discovery document, so no request for it ever leaves the
    ///     test: set before JwtBearer's own post-configuration runs, that wraps it in a static
    ///     configuration manager instead of fetching the Authority's metadata.
    /// </summary>
    public static void TrustTestIssuer(IServiceCollection services) =>
        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                var configuration = new OpenIdConnectConfiguration { Issuer = Issuer };
                configuration.SigningKeys.Add(SigningKey);
                options.Configuration = configuration;
            }
        );
}
