using System.Diagnostics.Metrics;
using FhirPseudonymizer.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace FhirPseudonymizer.Tests;

public class MemoryCacheMetricsTests(CustomWebApplicationFactory<Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private const string MeterName = "Microsoft.Extensions.Caching.Memory.MemoryCache";

    [Fact]
    public void PseudonymCache_ReportsRequestsTaggedWithItsName()
    {
        var cache = factory.Services.GetRequiredService<IMemoryCache>();

        cache.Set("present", "value", new MemoryCacheEntryOptions { Size = 1 });
        cache.TryGetValue("present", out _);
        cache.TryGetValue("missing", out _);

        var requests = RecordRequests("pseudonyms");

        requests.Should().ContainKey("hit").WhoseValue.Should().BeGreaterThanOrEqualTo(1);
        requests.Should().ContainKey("miss").WhoseValue.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void AnonymizerEngineCache_ReportsUnderItsOwnName()
    {
        var cache = factory.Services.GetRequiredKeyedService<IMemoryCache>(
            AnonymizerConfigCacheKeys.AnonymizerConfig
        );

        cache.TryGetValue("missing", out _);

        RecordRequests("anonymizer_engines")
            .Should()
            .ContainKey("miss")
            .WhoseValue.Should()
            .BeGreaterThanOrEqualTo(1);
    }

    // dotnet.cache.requests of the named cache, by dotnet.cache.request.result
    private Dictionary<string, long> RecordRequests(string cacheName)
    {
        // Only this app's meters: other test classes run their own app instances in parallel.
        var meterFactory = factory.Services.GetRequiredService<IMeterFactory>();
        var requests = new Dictionary<string, long>();

        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, l) =>
        {
            if (
                instrument.Meter.Name == MeterName
                && ReferenceEquals(instrument.Meter.Scope, meterFactory)
                && instrument.Name == "dotnet.cache.requests"
            )
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>(
            (_, value, tags, _) =>
            {
                string name = null;
                string result = null;
                foreach (var tag in tags)
                {
                    if (tag.Key == "dotnet.cache.name")
                    {
                        name = tag.Value as string;
                    }
                    else if (tag.Key == "dotnet.cache.request.result")
                    {
                        result = tag.Value as string;
                    }
                }

                if (name == cacheName && result is not null)
                {
                    requests[result] = value;
                }
            }
        );
        listener.Start();
        listener.RecordObservableInstruments();

        return requests;
    }
}
