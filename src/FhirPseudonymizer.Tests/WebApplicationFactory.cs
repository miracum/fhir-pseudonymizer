using FhirPseudonymizer.Pseudonymization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FhirPseudonymizer.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public IDictionary<string, string> CustomInMemorySettings { get; set; } = null;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // remove the existing context configuration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(IPseudonymServiceClient)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var psnClient = A.Fake<IPseudonymServiceClient>();
                A.CallTo(
                        () =>
                            psnClient.GetOrCreatePseudonymFor(
                                A<string>._,
                                A<string>._,
                                A<IReadOnlyDictionary<string, object>>._
                            )
                    )
                    .ReturnsLazily(
                        (
                            string original,
                            string domain,
                            IReadOnlyDictionary<string, object> settings
                        ) => $"pseuded-{original}@{domain}"
                    );
                A.CallTo(
                        () =>
                            psnClient.GetOriginalValueFor(
                                A<string>._,
                                A<string>._,
                                A<IReadOnlyDictionary<string, object>>._
                            )
                    )
                    .ReturnsLazily(
                        (
                            string pseudonym,
                            string domain,
                            IReadOnlyDictionary<string, object> settings
                        ) => $"original-{pseudonym}@{domain}"
                    );

                services.AddTransient(_ => psnClient);
            });

            if (CustomInMemorySettings is not null)
            {
                builder.ConfigureAppConfiguration(
                    (context, configBuilder) =>
                        configBuilder.AddInMemoryCollection(CustomInMemorySettings)
                );
            }
        }
    }
}
