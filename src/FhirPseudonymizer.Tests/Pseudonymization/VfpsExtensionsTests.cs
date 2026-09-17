using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Duende.AccessTokenManagement;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization.Vfps;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vfps.Protos;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class VfpsExtensionsTests
{
    private static readonly Uri VfpsAddress = new("http://vfps:8081");

    [Fact]
    public void AddVfpsClient_WithUnsetAddress_ShouldThrow()
    {
        var act = () => new ServiceCollection().AddVfpsClient(new VfpsConfig());

        act.Should().Throw<ValidationException>().WithMessage("*address is unset*");
    }

    [Fact]
    public void AddVfpsClient_WithTokenEndpointButNoClientCredentials_ShouldThrow()
    {
        var config = new VfpsConfig
        {
            Address = VfpsAddress,
            Auth = new()
            {
                OAuth = new() { TokenEndpoint = new Uri("http://keycloak/token"), ClientId = "id" },
            },
        };

        var act = () => new ServiceCollection().AddVfpsClient(config);

        act.Should()
            .Throw<ValidationException>()
            .WithMessage("*client id or client secret is unset*");
    }

    [Fact]
    public void AddVfpsClient_WithTheDefaultAppSettings_ShouldNotEnableOAuth()
    {
        // the shipped appsettings.json leaves every auth setting as an empty string, which has
        // to keep binding to a disabled OAuth config rather than a half-configured one.
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["Vfps:Address"] = VfpsAddress.AbsoluteUri,
                    ["Vfps:Auth:OAuth:TokenEndpoint"] = "",
                    ["Vfps:Auth:OAuth:ClientId"] = "",
                    ["Vfps:Auth:OAuth:ClientSecret"] = "",
                }
            )
            .Build();

        var appConfig = new AppConfig();
        config.Bind(appConfig);

        var act = () => new ServiceCollection().AddLogging().AddVfpsClient(appConfig.Vfps);

        act.Should().NotThrow();
    }

    [Fact]
    public async Task AddVfpsClient_WithOAuthConfigured_ShouldSendAccessTokenAsBearerMetadata()
    {
        var tokenEndpoint = new CapturingHandler(TokenResponse("the-access-token"));
        var vfps = new CapturingHandler(TrailersOnlyGrpcResponse());

        var config = new VfpsConfig
        {
            Address = VfpsAddress,
            UnsafeUseInsecureChannelCallCredentials = true,
            Auth = new()
            {
                OAuth = new()
                {
                    TokenEndpoint = new Uri("http://keycloak/token"),
                    ClientId = "fhir-pseudonymizer",
                    ClientSecret = "very-secret",
                    Scope = "vfps",
                },
            },
        };

        await InvokeCreateAsync(config, vfps, tokenEndpoint);

        vfps.LastAuthorizationHeader.Should().Be("Bearer the-access-token");
        tokenEndpoint.LastRequest.Should().NotBeNull();
    }

    [Fact]
    public async Task AddVfpsClient_WithBasicAuthConfigured_ShouldSendBasicAuthMetadata()
    {
        var vfps = new CapturingHandler(TrailersOnlyGrpcResponse());

        var config = new VfpsConfig
        {
            Address = VfpsAddress,
            UnsafeUseInsecureChannelCallCredentials = true,
            Auth = new()
            {
                Basic = new() { Username = "user", Password = "pass" },
            },
        };

        await InvokeCreateAsync(config, vfps);

        var expected = Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass"));
        vfps.LastAuthorizationHeader.Should().Be($"Basic {expected}");
    }

    [Fact]
    public async Task AddVfpsClient_WithOAuthAndBasicAuthConfigured_ShouldPreferOAuth()
    {
        var tokenEndpoint = new CapturingHandler(TokenResponse("the-access-token"));
        var vfps = new CapturingHandler(TrailersOnlyGrpcResponse());

        var config = new VfpsConfig
        {
            Address = VfpsAddress,
            UnsafeUseInsecureChannelCallCredentials = true,
            Auth = new()
            {
                Basic = new() { Username = "user", Password = "pass" },
                OAuth = new()
                {
                    TokenEndpoint = new Uri("http://keycloak/token"),
                    ClientId = "fhir-pseudonymizer",
                    ClientSecret = "very-secret",
                },
            },
        };

        await InvokeCreateAsync(config, vfps, tokenEndpoint);

        vfps.LastAuthorizationHeader.Should().Be("Bearer the-access-token");
    }

    [Fact]
    public async Task AddVfpsClient_WithoutAnyAuthConfigured_ShouldNotSendAuthorizationMetadata()
    {
        var vfps = new CapturingHandler(TrailersOnlyGrpcResponse());

        var config = new VfpsConfig
        {
            Address = VfpsAddress,
            UnsafeUseInsecureChannelCallCredentials = true,
        };

        await InvokeCreateAsync(config, vfps);

        vfps.LastAuthorizationHeader.Should().BeNull();
    }

    private static async Task InvokeCreateAsync(
        VfpsConfig config,
        CapturingHandler vfpsHandler,
        CapturingHandler tokenEndpointHandler = null
    )
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddVfpsClient(config);

        services
            .AddGrpcClient<PseudonymService.PseudonymServiceClient>()
            .ConfigurePrimaryHttpMessageHandler(() => vfpsHandler);

        if (tokenEndpointHandler is not null)
        {
            services
                .AddHttpClient(ClientCredentialsTokenManagementDefaults.BackChannelHttpClientName)
                .ConfigurePrimaryHttpMessageHandler(() => tokenEndpointHandler);
        }

        var client = services
            .BuildServiceProvider()
            .GetRequiredService<PseudonymService.PseudonymServiceClient>();

        // the fake backend always answers with PermissionDenied - all that matters here is
        // the metadata the call was made with, which the handler captured on its way out.
        var act = async () =>
            await client.CreateAsync(
                new PseudonymServiceCreateRequest { Namespace = "test", OriginalValue = "test" }
            );

        await act.Should().ThrowAsync<RpcException>();
    }

    private static Func<HttpResponseMessage> TokenResponse(string accessToken) =>
        () =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    $$"""
                    {"access_token":"{{accessToken}}","token_type":"Bearer","expires_in":3600}
                    """,
                    Encoding.UTF8,
                    "application/json"
                ),
            };

    private static Func<HttpResponseMessage> TrailersOnlyGrpcResponse() =>
        () =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Version = HttpVersion.Version20,
                Content = new ByteArrayContent([])
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/grpc") },
                },
            };

            response.Headers.Add("grpc-status", ((int)StatusCode.PermissionDenied).ToString());
            return response;
        };

    private sealed class CapturingHandler(Func<HttpResponseMessage> respond) : HttpMessageHandler
    {
        public HttpRequestMessage LastRequest { get; private set; }

        public string LastAuthorizationHeader { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            LastRequest = request;
            LastAuthorizationHeader = request.Headers.TryGetValues("Authorization", out var values)
                ? string.Join(' ', values)
                : null;

            return Task.FromResult(respond());
        }
    }
}
