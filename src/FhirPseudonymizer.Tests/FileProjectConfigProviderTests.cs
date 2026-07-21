using FhirPseudonymizer.Config;
using FhirPseudonymizer.Projects;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Tests;

public sealed class FileProjectConfigProviderTests : IDisposable
{
    private const string RedactIdentifierConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('Identifier').value
            method: redact
        """;

    private const string KeepIdentifierConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('Identifier').value
            method: keep
        """;

    private readonly string configsDir;
    private readonly IAnonymizerEngineFactory engineFactory = A.Fake<IAnonymizerEngineFactory>();

    static FileProjectConfigProviderTests()
    {
        // The rules are FHIRPath expressions against symbols the engine registers at startup;
        // parsing a config that uses nodesByType() needs them present. Idempotent.
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();
    }

    public FileProjectConfigProviderTests()
    {
        configsDir = Path.Join(Path.GetTempPath(), "fp-projects-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(configsDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(configsDir))
        {
            Directory.Delete(configsDir, recursive: true);
        }
    }

    private FileProjectConfigProvider CreateProvider()
    {
        return new FileProjectConfigProvider(
            configsDir,
            engineFactory,
            new CacheConfig { SizeLimit = 128 }
        );
    }

    private void WriteConfig(string name, string yaml)
    {
        File.WriteAllText(Path.Join(configsDir, $"{name}.yaml"), yaml);
    }

    private static ProjectEngines SomeEngines()
    {
        return new ProjectEngines(A.Fake<IAnonymizerEngine>(), A.Fake<IDePseudonymizerEngine>());
    }

    [Fact]
    public void TryGetEngines_ForNameWithNoConfigFile_ReturnsFalse()
    {
        var provider = CreateProvider();

        var found = provider.TryGetEngines("absent", out var engines);

        found.Should().BeFalse();
        engines.Should().BeNull();
    }

    [Fact]
    public void TryGetEngines_ForNameWithConfigFile_ReturnsEnginesBuiltFromThatFile()
    {
        var built = SomeEngines();
        A.CallTo(() => engineFactory.Create(A<AnonymizerConfigurationManager>._)).Returns(built);
        WriteConfig("study-a", RedactIdentifierConfig);
        var provider = CreateProvider();

        var found = provider.TryGetEngines("study-a", out var engines);

        found.Should().BeTrue();
        engines.Should().BeSameAs(built);
    }

    [Fact]
    public void TryGetEngines_CalledTwiceForAnUnchangedFile_BuildsTheEnginesOnce()
    {
        A.CallTo(() => engineFactory.Create(A<AnonymizerConfigurationManager>._))
            .Returns(SomeEngines());
        WriteConfig("study-a", RedactIdentifierConfig);
        var provider = CreateProvider();

        provider.TryGetEngines("study-a", out _);
        provider.TryGetEngines("study-a", out _);

        A.CallTo(() => engineFactory.Create(A<AnonymizerConfigurationManager>._))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void TryGetEngines_AfterTheFileContentChanges_RebuildsFromTheNewContent()
    {
        var first = SomeEngines();
        var second = SomeEngines();
        A.CallTo(() => engineFactory.Create(A<AnonymizerConfigurationManager>._))
            .ReturnsNextFromSequence(first, second);
        WriteConfig("study-a", RedactIdentifierConfig);
        var provider = CreateProvider();

        provider.TryGetEngines("study-a", out var before);
        WriteConfig("study-a", KeepIdentifierConfig);
        provider.TryGetEngines("study-a", out var after);

        before.Should().BeSameAs(first);
        after.Should().BeSameAs(second);
    }

    [Theory]
    [InlineData("", "empty")]
    [InlineData("fhirPathRules: [unclosed", "malformed-yaml")]
    [InlineData(
        "fhirVersion: R4\nfhirPathRules:\n  - path: Patient.id\n    method: nonsense",
        "unknown-method"
    )]
    [InlineData("fhirVersion: R4\nfhirPathRules: [null]", "null-rule")]
    public void TryGetEngines_ForAConfigThatCannotBuildEngines_ThrowsInvalidProjectConfig(
        string yaml,
        string caseName
    )
    {
        WriteConfig($"broken-{caseName}", yaml);
        var provider = CreateProvider();

        var act = () => provider.TryGetEngines($"broken-{caseName}", out _);

        act.Should().Throw<InvalidProjectConfigException>();
    }

    [Theory]
    [InlineData("encrypt", "encryptKey")]
    [InlineData("cryptoHash", "cryptoHashKey")]
    [InlineData("dateShift", "dateShiftKey")]
    public void TryGetEngines_ForAConfigMissingItsMethodsKey_ThrowsNamingTheParameter(
        string method,
        string missingParameter
    )
    {
        var yaml = $"""
            fhirVersion: R4
            fhirPathRules:
              - path: nodesByType('Identifier').value
                method: {method}
            """;
        WriteConfig($"missing-{method}", yaml);
        var provider = CreateProvider();

        var act = () => provider.TryGetEngines($"missing-{method}", out _);

        act.Should().Throw<InvalidProjectConfigException>().WithMessage($"*{missingParameter}*");
    }

    [Fact]
    public void TryGetEngines_ForANameEscapingTheDirectory_DoesNotReadOutsideIt()
    {
        // A file the mounted directory does not contain; a '..' in the name must not reach it.
        var outside = Path.Join(Path.GetDirectoryName(configsDir)!, "escaped.yaml");
        File.WriteAllText(outside, RedactIdentifierConfig);
        try
        {
            var provider = CreateProvider();

            var found = provider.TryGetEngines("../escaped", out var engines);

            found.Should().BeFalse();
            engines.Should().BeNull();
        }
        finally
        {
            File.Delete(outside);
        }
    }

    [Fact]
    public void TryGetEngines_ForAConfigWithTheYmlExtension_ResolvesIt()
    {
        A.CallTo(() => engineFactory.Create(A<AnonymizerConfigurationManager>._))
            .Returns(SomeEngines());
        File.WriteAllText(Path.Join(configsDir, "ymlproj.yml"), RedactIdentifierConfig);
        var provider = CreateProvider();

        var found = provider.TryGetEngines("ymlproj", out var engines);

        found.Should().BeTrue();
        engines.Should().NotBeNull();
    }
}
