using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace FhirPseudonymizer.Tests;

public class ProgramTests
{
    private static WebHostBuilderContext CreateContext(
        IDictionary<string, string> settings
    )
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new WebHostBuilderContext { Configuration = configuration };
    }

    [Fact]
    public void ConfigureMaxRequestBodySize_WithConfiguredValue_SetsKestrelLimit()
    {
        var context = CreateContext(
            new Dictionary<string, string> { ["MaxRequestBodySize"] = "123456789" }
        );
        var options = new KestrelServerOptions();

        Program.ConfigureMaxRequestBodySize(context, options);

        options.Limits.MaxRequestBodySize.Should().Be(123456789);
    }

    [Fact]
    public void ConfigureMaxRequestBodySize_WithoutConfiguredValue_KeepsKestrelDefault()
    {
        var context = CreateContext(new Dictionary<string, string>());
        var options = new KestrelServerOptions();
        var defaultValue = options.Limits.MaxRequestBodySize;

        Program.ConfigureMaxRequestBodySize(context, options);

        options.Limits.MaxRequestBodySize.Should().Be(defaultValue);
    }
}
