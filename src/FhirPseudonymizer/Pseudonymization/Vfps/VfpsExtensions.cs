using System;
using System.Text;
using System.Threading.Tasks;
using FhirPseudonymizer.Config;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vfps.Protos;

namespace FhirPseudonymizer.Pseudonymization.Vfps;

public static class VfpsExtensions
{
    public static IServiceCollection AddVfpsClient(
        this IServiceCollection services,
        VfpsConfig vfpsConfig
    )
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 3,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable, StatusCode.Internal }
            }
        };

        services
            .AddGrpcClient<PseudonymService.PseudonymServiceClient>(
                o => o.Address = vfpsConfig.Address
            )
            .ConfigureChannel(o =>
            {
                o.ServiceConfig = new ServiceConfig { MethodConfigs = { defaultMethodConfig } };

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
            })
            .AddCallCredentials(
                (_, metadata) =>
                {
                    if (!string.IsNullOrEmpty(vfpsConfig.Auth.Basic.Username))
                    {
                        var basicAuthString =
                            $"{vfpsConfig.Auth.Basic.Username}:{vfpsConfig.Auth.Basic.Password}";
                        var byteArray = Encoding.UTF8.GetBytes(basicAuthString);
                        var basicAuthValue = Convert.ToBase64String(byteArray);

                        metadata.Add("Authorization", $"Basic {basicAuthValue}");
                    }

                    return Task.CompletedTask;
                }
            );

        services.AddTransient<IPseudonymServiceClient, VfpsPseudonymServiceClient>();

        return services;
    }
}
