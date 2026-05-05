using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Extensions.Caching.Memory;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class CachedPseudonymServiceClientTests
{
    [Fact]
    public async Task GetOrCreatePseudonymFor_WithSameInput_ShouldOnlyCallInnerClientOnce()
    {
        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() => innerClient.GetOrCreatePseudonymFor("value", "domain", null))
            .Returns("pseudonym");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var firstResult = await sut.GetOrCreatePseudonymFor("value", "domain");
        var secondResult = await sut.GetOrCreatePseudonymFor("value", "domain");

        firstResult.Should().Be("pseudonym");
        secondResult.Should().Be("pseudonym");
        A.CallTo(() => innerClient.GetOrCreatePseudonymFor("value", "domain", null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOriginalValueFor_WithSameInput_ShouldOnlyCallInnerClientOnce()
    {
        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() => innerClient.GetOriginalValueFor("pseudonym", "domain", null))
            .Returns("value");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var firstResult = await sut.GetOriginalValueFor("pseudonym", "domain");
        var secondResult = await sut.GetOriginalValueFor("pseudonym", "domain");

        firstResult.Should().Be("value");
        secondResult.Should().Be("value");
        A.CallTo(() => innerClient.GetOriginalValueFor("pseudonym", "domain", null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOriginalValueFor_WithSameKeyAsPseudonymization_ShouldNotReusePseudonymizationCacheEntry()
    {
        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() => innerClient.GetOrCreatePseudonymFor("same", "domain", null))
            .Returns("psn-same");
        A.CallTo(() => innerClient.GetOriginalValueFor("same", "domain", null))
            .Returns("orig-same");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var pseudonymResult = await sut.GetOrCreatePseudonymFor("same", "domain");
        var originalResult = await sut.GetOriginalValueFor("same", "domain");

        pseudonymResult.Should().Be("psn-same");
        originalResult.Should().Be("orig-same");
        A.CallTo(() => innerClient.GetOrCreatePseudonymFor("same", "domain", null))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => innerClient.GetOriginalValueFor("same", "domain", null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithDifferentSettings_ShouldNotShareCacheEntry()
    {
        var settingsA = new Dictionary<string, object> { ["resourceType"] = "Patient" };
        var settingsB = new Dictionary<string, object> { ["resourceType"] = "Encounter" };

        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor("same", "domain", settingsA);
        await sut.GetOrCreatePseudonymFor("same", "domain", settingsB);

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._
                )
            )
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithEquivalentSettings_ShouldShareCacheEntry()
    {
        var settingsA = new Dictionary<string, object> { ["resourceType"] = "Patient" };
        var settingsB = new Dictionary<string, object> { ["resourceType"] = "Patient" };

        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor("same", "domain", settingsA);
        await sut.GetOrCreatePseudonymFor("same", "domain", settingsB);

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._
                )
            )
            .MustHaveHappenedOnceExactly();
    }

    private static IMemoryCache CreateCache()
    {
        return new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 });
    }

    private static CacheConfig CreateCacheConfig()
    {
        return new CacheConfig { AbsoluteExpirationMinutes = 30, SlidingExpirationMinutes = 5 };
    }
}
