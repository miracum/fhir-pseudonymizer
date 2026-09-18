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
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor("value", "domain", null, A<CancellationToken>._)
            )
            .Returns("pseudonym");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var firstResult = await sut.GetOrCreatePseudonymFor(
            "value",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );
        var secondResult = await sut.GetOrCreatePseudonymFor(
            "value",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        firstResult.Should().Be("pseudonym");
        secondResult.Should().Be("pseudonym");
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor("value", "domain", null, A<CancellationToken>._)
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOriginalValueFor_WithSameInput_ShouldOnlyCallInnerClientOnce()
    {
        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOriginalValueFor("pseudonym", "domain", null, A<CancellationToken>._)
            )
            .Returns("value");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var firstResult = await sut.GetOriginalValueFor(
            "pseudonym",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );
        var secondResult = await sut.GetOriginalValueFor(
            "pseudonym",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        firstResult.Should().Be("value");
        secondResult.Should().Be("value");
        A.CallTo(() =>
                innerClient.GetOriginalValueFor("pseudonym", "domain", null, A<CancellationToken>._)
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOriginalValueFor_WithSameKeyAsPseudonymization_ShouldNotReusePseudonymizationCacheEntry()
    {
        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor("same", "domain", null, A<CancellationToken>._)
            )
            .Returns("psn-same");
        A.CallTo(() =>
                innerClient.GetOriginalValueFor("same", "domain", null, A<CancellationToken>._)
            )
            .Returns("orig-same");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var pseudonymResult = await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );
        var originalResult = await sut.GetOriginalValueFor(
            "same",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );

        pseudonymResult.Should().Be("psn-same");
        originalResult.Should().Be("orig-same");
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor("same", "domain", null, A<CancellationToken>._)
            )
            .MustHaveHappenedOnceExactly();
        A.CallTo(() =>
                innerClient.GetOriginalValueFor("same", "domain", null, A<CancellationToken>._)
            )
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
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsA,
            cancellationToken: TestContext.Current.CancellationToken
        );
        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsB,
            cancellationToken: TestContext.Current.CancellationToken
        );

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
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
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsA,
            cancellationToken: TestContext.Current.CancellationToken
        );
        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsB,
            cancellationToken: TestContext.Current.CancellationToken
        );

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithEquivalentNestedSettings_ShouldShareCacheEntry()
    {
        var settingsA = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Patient",
                ["project"] = "https://fhir.example.com/test",
            },
        };
        var settingsB = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Patient",
                ["project"] = "https://fhir.example.com/test",
            },
        };

        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsA,
            cancellationToken: TestContext.Current.CancellationToken
        );
        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsB,
            cancellationToken: TestContext.Current.CancellationToken
        );

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrCreatePseudonymFor_WithDifferentNestedSettings_ShouldNotShareCacheEntry()
    {
        var settingsA = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Patient",
                ["project"] = "https://fhir.example.com/test",
            },
        };
        var settingsB = new Dictionary<string, object>
        {
            ["entici"] = new Dictionary<object, object>
            {
                ["resourceType"] = "Encounter",
                ["project"] = "https://fhir.example.com/test",
            },
        };

        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .Returns("psn");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsA,
            cancellationToken: TestContext.Current.CancellationToken
        );
        await sut.GetOrCreatePseudonymFor(
            "same",
            "domain",
            settingsB,
            cancellationToken: TestContext.Current.CancellationToken
        );

        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "same",
                    "domain",
                    A<IReadOnlyDictionary<string, object>>._,
                    A<CancellationToken>._
                )
            )
            .MustHaveHappenedTwiceExactly();
    }

    // IMemoryCache.GetOrCreateAsync only commits an entry once the factory has set a value, so a
    // cancelled lookup must not poison the cache for the requests that follow it.
    [Fact]
    public async Task GetOrCreatePseudonymFor_WhenTheInnerClientIsCancelled_DoesNotCacheAnything()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var innerClient = A.Fake<IPseudonymServiceClient>();
        A.CallTo(() => innerClient.GetOrCreatePseudonymFor("value", "domain", null, cts.Token))
            .Throws(() => new OperationCanceledException(cts.Token));
        A.CallTo(() =>
                innerClient.GetOrCreatePseudonymFor(
                    "value",
                    "domain",
                    null,
                    TestContext.Current.CancellationToken
                )
            )
            .Returns("pseudonym");

        var sut = new CachedPseudonymServiceClient(innerClient, CreateCache(), CreateCacheConfig());

        var cancelled = async () =>
            await sut.GetOrCreatePseudonymFor("value", "domain", cancellationToken: cts.Token);
        await cancelled.Should().ThrowAsync<OperationCanceledException>();

        // the same key resolves normally afterwards rather than replaying the failure
        var result = await sut.GetOrCreatePseudonymFor(
            "value",
            "domain",
            cancellationToken: TestContext.Current.CancellationToken
        );
        result.Should().Be("pseudonym");
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
