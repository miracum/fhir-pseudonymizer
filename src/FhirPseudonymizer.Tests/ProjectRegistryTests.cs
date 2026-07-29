using FhirPseudonymizer.Config;
using FhirPseudonymizer.Projects;
using Microsoft.Extensions.Configuration;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests;

public class ProjectRegistryTests
{
    private const string SomeValidConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: Patient.id
            method: keep
        """;

    [Fact]
    public void Ctor_WithASizeLimitOfZero_ShouldFailFastNamingTheSetting()
    {
        var create = () =>
            new ProjectRegistry(
                A.Fake<IAnonymizerEngineFactory>(),
                new CacheConfig { SizeLimit = 0 }
            );

        create
            .Should()
            .Throw<InvalidOperationException>(
                "a zero-sized registry rejects every entry, turning each registration into "
                    + "a 503 that promises a retry which can never succeed"
            )
            .WithMessage("*ProjectCache:SizeLimit*");
    }

    [Fact]
    public void Register_AgainstAConfigurationNamingNoProjectCache_ShouldStoreTheProject()
    {
        // The shipped appsettings.json carries a limit, but nothing guarantees a deployment
        // layers that file — so the limit has to survive binding against a configuration that
        // says nothing about it, rather than falling to a zero that stores nothing.
        var appConfig = new AppConfig();
        new ConfigurationBuilder().Build().Bind(appConfig);

        using var registry = new ProjectRegistry(
            A.Fake<IAnonymizerEngineFactory>(),
            appConfig.ProjectCache
        );

        registry
            .Register("some-project", SomeValidConfig)
            .Should()
            .Be(ProjectRegistrationOutcome.Created);
    }

    [Fact]
    public async Task Register_AtAFullSmallRegistry_ShouldFreeASlotSoARetrySucceeds()
    {
        using var registry = new ProjectRegistry(
            A.Fake<IAnonymizerEngineFactory>(),
            new CacheConfig { SizeLimit = 2 }
        );
        registry.Register("first", SomeValidConfig).Should().Be(ProjectRegistrationOutcome.Created);
        registry
            .Register("second", SomeValidConfig)
            .Should()
            .Be(ProjectRegistrationOutcome.Created);

        var outcome = registry.Register("third", SomeValidConfig);

        outcome
            .Should()
            .Be(ProjectRegistrationOutcome.NotStored, "the registry was full at that moment");

        // The rejection's 503 promises that a retry will succeed shortly, because reaching the
        // limit triggers a compaction. It runs on a background thread, hence the polling.
        for (var i = 0; i < 100 && outcome == ProjectRegistrationOutcome.NotStored; i++)
        {
            await Task.Delay(50, TestContext.Current.CancellationToken);
            outcome = registry.Register("third", SomeValidConfig);
        }

        outcome.Should().Be(ProjectRegistrationOutcome.Created);
    }

    [Fact]
    public async Task Register_AtAFullRegistry_ShouldCostExactlyOneOtherProject()
    {
        // Every evicted Project is a caller whose next request 404s and who has to register
        // again, so how many an overflow costs is not academic. 40 tells the candidate rates
        // apart: a flat 5% frees two entries, one slot's worth frees one.
        const int sizeLimit = 40;
        using var registry = new ProjectRegistry(
            EngineFactory(),
            new CacheConfig { SizeLimit = sizeLimit }
        );
        for (var i = 0; i < sizeLimit; i++)
        {
            registry
                .Register($"project-{i}", SomeValidConfig)
                .Should()
                .Be(ProjectRegistrationOutcome.Created);
        }

        registry
            .Register("one-too-many", SomeValidConfig)
            .Should()
            .Be(ProjectRegistrationOutcome.NotStored, "the registry was full at that moment");

        // Registered exactly once: each rejection schedules its own compaction, so retrying here
        // would evict again and measure something else. The compaction runs on a background
        // thread, hence the polling.
        var survivors = sizeLimit;
        for (var i = 0; i < 100 && survivors == sizeLimit; i++)
        {
            await Task.Delay(50, TestContext.Current.CancellationToken);
            survivors = Enumerable
                .Range(0, sizeLimit)
                .Count(j => registry.TryGet($"project-{j}", out _));
        }

        survivors
            .Should()
            .Be(sizeLimit - 1, "making room for one Project should cost exactly one other");
    }

    /// <summary>
    ///     Returns real (if empty) Engines rather than the null a bare fake would, so a lookup
    ///     answers on the entry's presence instead of on how a cache treats a null value.
    /// </summary>
    private static IAnonymizerEngineFactory EngineFactory()
    {
        var factory = A.Fake<IAnonymizerEngineFactory>();
        A.CallTo(() => factory.Create(A<AnonymizerConfigurationManager>._))
            .ReturnsLazily(() =>
                new ProjectEngines(A.Fake<IAnonymizerEngine>(), A.Fake<IDePseudonymizerEngine>())
            );

        return factory;
    }
}
