using System.ComponentModel.DataAnnotations;
using System.Text;
using Duende.AccessTokenManagement;
using FhirPseudonymizer.Config;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public static class VfpsExtensions
{
    internal const string OAuthClientName = "vfps.oAuth.client";

    internal static readonly TimeSpan DnsRefreshInterval = TimeSpan.FromSeconds(15);

    internal static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);

    // Pings an HTTP/2 connection after this long without receiving a frame on it, and drops it
    // if the ping isn't answered in time. Without pings a connection whose peer disappeared
    // without a FIN/RST - a replaced pod whose IP now blackholes traffic - looks healthy until
    // the kernel gives up on it, which can take many minutes.
    internal static readonly TimeSpan KeepAlivePingDelay = TimeSpan.FromSeconds(20);

    internal static readonly TimeSpan KeepAlivePingTimeout = TimeSpan.FromSeconds(10);

    public static IServiceCollection AddVfpsClient(
        this IServiceCollection services,
        VfpsConfig vfpsConfig
    )
    {
        if (string.IsNullOrWhiteSpace(vfpsConfig.Address?.AbsoluteUri))
        {
            throw new ValidationException(
                "Vfps is enabled but the backend service address is unset."
            );
        }

        var oAuthConfig = vfpsConfig.Auth.OAuth;

        var isOAuthEnabled = oAuthConfig.TokenEndpoint is not null;

        // Trimmed because this almost always arrives from a mounted secret or a copy-paste, and
        // a trailing newline in an Authorization header is rejected by the server as a malformed
        // token - a failure that reads like a wrong token rather than a stray byte.
        var accessToken = vfpsConfig.Auth.AccessToken?.Trim();
        var isAccessTokenConfigured = !string.IsNullOrEmpty(accessToken);

        // All three mechanisms set the same Authorization metadata, so configuring more than one
        // is a mistake rather than a preference. OAuth and basic auth have silently coexisted
        // (OAuth wins) since before the access token existed and are left alone; the new
        // combinations fail loudly instead of picking a winner nobody chose.
        if (isAccessTokenConfigured && isOAuthEnabled)
        {
            throw new ValidationException(
                "Vfps is configured with both an access token and an OAuth token endpoint. "
                    + "Use one or the other."
            );
        }

        if (isAccessTokenConfigured && !string.IsNullOrWhiteSpace(vfpsConfig.Auth.Basic.Username))
        {
            throw new ValidationException(
                "Vfps is configured with both an access token and basic auth credentials. "
                    + "Use one or the other."
            );
        }

        if (isOAuthEnabled)
        {
            if (
                string.IsNullOrWhiteSpace(oAuthConfig.ClientId)
                || string.IsNullOrWhiteSpace(oAuthConfig.ClientSecret)
            )
            {
                throw new ValidationException(
                    "Vfps OAuth is enabled but the client id or client secret is unset."
                );
            }

            services
                .AddClientCredentialsTokenManagement()
                .AddClient(
                    OAuthClientName,
                    client =>
                    {
                        client.TokenEndpoint = oAuthConfig.TokenEndpoint;
                        client.ClientId = ClientId.Parse(oAuthConfig.ClientId);
                        client.ClientSecret = ClientSecret.Parse(oAuthConfig.ClientSecret);

                        if (!string.IsNullOrEmpty(oAuthConfig.Scope))
                        {
                            client.Scope = Scope.Parse(oAuthConfig.Scope);
                        }

                        if (!string.IsNullOrEmpty(oAuthConfig.Resource))
                        {
                            client.Resource = Resource.Parse(oAuthConfig.Resource);
                        }
                    }
                );
        }

        var retryPolicy = new RetryPolicy
        {
            MaxAttempts = Math.Max(2, vfpsConfig.Retry.Count + 1),
            InitialBackoff = TimeSpan.FromSeconds(1),
            MaxBackoff = TimeSpan.FromSeconds(5),
            BackoffMultiplier = 1.5,
        };

        foreach (var statusCode in VfpsPseudonymServiceClient.TransientStatusCodes)
        {
            retryPolicy.RetryableStatusCodes.Add(statusCode);
        }

        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = retryPolicy,
        };

        var isClientSideLoadBalanced = string.Equals(
            vfpsConfig.Address.Scheme,
            "dns",
            StringComparison.OrdinalIgnoreCase
        );

        if (isClientSideLoadBalanced)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<ResolverFactory>(
                    new DnsResolverFactory(DnsRefreshInterval)
                )
            );
        }

        var clientBuilder = services
            .AddGrpcClient<PseudonymService.PseudonymServiceClient>(o =>
                o.Address = vfpsConfig.Address
            )
            .ConfigureChannel(o =>
            {
                o.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };

                if (isClientSideLoadBalanced)
                {
                    o.ServiceConfig.LoadBalancingConfigs.Add(new RoundRobinConfig());
                }

                if (vfpsConfig.UseTls)
                {
                    o.Credentials = new SslCredentials();
                }
                else
                {
                    o.Credentials = ChannelCredentials.Insecure;
                }

                o.UnsafeUseInsecureChannelCallCredentials =
                    vfpsConfig.UnsafeUseInsecureChannelCallCredentials;

                // A call cancelled by the caller's token - e.g. an aborted HTTP request - throws
                // an OperationCanceledException instead of an RpcException(Cancelled), so it's
                // handled like any other cancellation rather than as a failed call. A call whose
                // deadline was exceeded throws one too, then with the caller's token not
                // cancelled; VfpsPseudonymServiceClient reports that as transient.
                o.ThrowOperationCanceledOnCancellation = true;
            })
            .AddInterceptor(() =>
                new DefaultDeadlineInterceptor(TimeSpan.FromSeconds(vfpsConfig.DeadlineSeconds))
            )
            .AddCallCredentials(
                async (_, metadata, serviceProvider) =>
                {
                    if (isOAuthEnabled)
                    {
                        // The token manager caches the token and only hits the token
                        // endpoint again shortly before it expires, so this doesn't add a
                        // round trip per gRPC call.
                        var token = await serviceProvider
                            .GetRequiredService<IClientCredentialsTokenManager>()
                            .GetAccessTokenAsync(ClientCredentialsClientName.Parse(OAuthClientName))
                            .GetToken();

                        metadata.Add("Authorization", $"Bearer {token.AccessToken}");
                    }
                    else if (isAccessTokenConfigured)
                    {
                        // Sent as-is, with nothing to refresh: Vfps verifies this token against
                        // its own store, so there is no token endpoint in the picture at all.
                        metadata.Add("Authorization", $"Bearer {accessToken}");
                    }
                    else if (!string.IsNullOrEmpty(vfpsConfig.Auth.Basic.Username))
                    {
                        var basicAuthString =
                            $"{vfpsConfig.Auth.Basic.Username}:{vfpsConfig.Auth.Basic.Password}";
                        var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                        var basicAuthValue = Convert.ToBase64String(byteArray);

                        metadata.Add("Authorization", $"Basic {basicAuthValue}");
                    }
                }
            );

        // Modifies the client factory's own SocketsHttpHandler rather than replacing it, so its
        // gRPC defaults (multiple HTTP/2 connections, a pooled connection lifetime) stay.
        clientBuilder.ConfigurePrimaryHttpMessageHandler(
            (handler, _) =>
            {
                if (handler is not SocketsHttpHandler socketsHttpHandler)
                {
                    return;
                }

                socketsHttpHandler.KeepAlivePingDelay = KeepAlivePingDelay;
                socketsHttpHandler.KeepAlivePingTimeout = KeepAlivePingTimeout;
                // Also pings idle connections, so a dead one is dropped before a call lands on it
                socketsHttpHandler.KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always;

                if (isClientSideLoadBalanced)
                {
                    socketsHttpHandler.ConnectTimeout = ConnectTimeout;
                }
            }
        );

        services.AddTransient<VfpsPseudonymServiceClient>();
        services.AddTransient<IPseudonymServiceClient>(
            serviceProvider => new CachedPseudonymServiceClient(
                serviceProvider.GetRequiredService<VfpsPseudonymServiceClient>(),
                serviceProvider.GetRequiredService<IMemoryCache>(),
                serviceProvider.GetRequiredService<CacheConfig>()
            )
        );

        return services;
    }
}
