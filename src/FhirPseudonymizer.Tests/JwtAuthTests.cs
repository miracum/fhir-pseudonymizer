using System.Net;
using System.Net.Http.Headers;

namespace FhirPseudonymizer.Tests;

public class JwtAuthTests
{
    private const string Scope = "fhir-pseudonymizer:re-identify";
    private const string Role = "re-identifier";

    [Theory]
    [InlineData("/fhir/$re-identify")]
    [InlineData("/fhir/$de-pseudonymize")]
    public async Task PostReIdentify_WithValidToken_ShouldSucceed(string url)
    {
        using var factory = CreateFactory(requiredScope: Scope);
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            url,
            TestTokens.Create(claims: new() { ["scope"] = $"openid {Scope} profile" })
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostReIdentify_WithRequiredScopeInScpClaim_ShouldSucceed()
    {
        using var factory = CreateFactory(requiredScope: Scope);
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(claims: new() { ["scp"] = Scope })
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostReIdentify_WithoutRequiredScope_ShouldReturnForbidden()
    {
        using var factory = CreateFactory(requiredScope: Scope);
        using var client = factory.CreateClient();

        // A scope that merely contains the required one as a substring mustn't count.
        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(claims: new() { ["scope"] = $"openid {Scope}-other" })
        );

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PostReIdentify_WithRequiredRoleInRolesClaim_ShouldSucceed()
    {
        using var factory = CreateFactory(requiredRole: Role);
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(claims: new() { ["roles"] = new[] { "other", Role } })
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostReIdentify_WithRequiredRoleInKeycloakRealmAccess_ShouldSucceed()
    {
        using var factory = CreateFactory(requiredRole: Role);
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(
                claims: new()
                {
                    ["realm_access"] = new Dictionary<string, object>
                    {
                        ["roles"] = new[] { "offline_access", Role },
                    },
                }
            )
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostReIdentify_WithoutRequiredRole_ShouldReturnForbidden()
    {
        using var factory = CreateFactory(requiredRole: Role);
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(
                claims: new()
                {
                    ["roles"] = new[] { "other" },
                    ["realm_access"] = new Dictionary<string, object>
                    {
                        ["roles"] = new[] { "offline_access" },
                    },
                }
            )
        );

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task PostReIdentify_WithWrongAudience_ShouldReturnUnauthorized()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(audience: "some-other-client")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostReIdentify_WithWrongIssuer_ShouldReturnUnauthorized()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(issuer: "https://idp.example.test/realms/other")
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostReIdentify_WithExpiredToken_ShouldReturnUnauthorized()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Further in the past than the default 5 minutes of allowed clock skew.
        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(expires: DateTime.UtcNow.AddMinutes(-10))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostReIdentify_WithTokenSignedByUnknownKey_ShouldReturnUnauthorized()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$re-identify",
            TestTokens.Create(signingKey: TestTokens.CreateSigningKey("unknown-key"))
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("/fhir/$re-identify")]
    [InlineData("/fhir/$de-pseudonymize")]
    public async Task PostReIdentify_WithoutJwtAuthConfigured_ShouldRejectEveryToken(string url)
    {
        using var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["Auth:Jwt:Authority"] = "",
                ["Auth:Jwt:Audience"] = "",
            },
            TrustTestTokenIssuer = false,
        };
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(client, url, TestTokens.Create());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostReIdentify_WithoutToken_ShouldReturnUnauthorized()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            "/fhir/$re-identify",
            CreatePatientContent(),
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostDeIdentify_WithInvalidToken_ShouldStillBeAllowed()
    {
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var response = await PostPatientAsync(
            client,
            "/fhir/$de-identify",
            TestTokens.Create(audience: "some-other-client")
        );

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Startup_WithJwtAuthorityButNoAudience_ShouldFailToStart()
    {
        using var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["Auth:Jwt:Audience"] = "",
            },
        };

        var act = () => factory.CreateClient();

        act.Should().Throw<InvalidOperationException>().WithMessage("*Auth:Jwt:Audience*");
    }

    private static CustomWebApplicationFactory<Startup> CreateFactory(
        string requiredScope = "",
        string requiredRole = ""
    ) =>
        new()
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["Auth:Jwt:RequiredScope"] = requiredScope,
                ["Auth:Jwt:RequiredRole"] = requiredRole,
            },
        };

    private static StringContent CreatePatientContent()
    {
        var content = new StringContent(
            """{ "resourceType": "Patient", "id": "glossy", "gender": "female" }"""
        );
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");
        return content;
    }

    private static Task<HttpResponseMessage> PostPatientAsync(
        HttpClient client,
        string url,
        string token
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = CreatePatientContent(),
        };
        request.Headers.Authorization = TestTokens.BearerHeader(token);
        return client.SendAsync(request, TestContext.Current.CancellationToken);
    }
}
