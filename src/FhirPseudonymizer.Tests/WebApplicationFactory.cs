using System.Linq;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FhirPseudonymizer.Tests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // remove the existing context configuration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(GPasFhirClient));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var gpas = A.Fake<IGPasFhirClient>();
                A.CallTo(() => gpas.GetOrCreatePseudonymFor(A<string>._, A<string>._))
                    .ReturnsLazily((string original, string domain) => $"pseuded-{original}@{domain}");
                A.CallTo(() => gpas.GetOriginalValueFor(A<string>._, A<string>._))
                    .ReturnsLazily((string pseudonym, string domain) => $"original-{pseudonym}@{domain}");

                services.AddTransient(_ => gpas);
            });
        }
    }
}
