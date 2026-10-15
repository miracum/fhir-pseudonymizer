using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;

namespace FhirPseudonymizer.Tests.Config;

public class PseudonymizationServiceConfigTests
{
    private const string PseudonymizeConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: Resource.id
            method: cryptoHash
          - path: nodesByType('Identifier').value
            method: Pseudonymize
            domain: test
        """;

    private const string NoPseudonymizeConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: Resource.id
            method: cryptoHash
        """;

    [Fact]
    public void AppConfig_WithoutPseudonymizationServiceSet_ShouldDefaultToNone()
    {
        var appConfig = new AppConfig();
        new ConfigurationBuilder().Build().Bind(appConfig);

        appConfig.PseudonymizationService.Should().Be(PseudonymizationServiceType.None);
    }

    [Fact]
    public void AppConfig_WithRetryCountSetPerService_ShouldBindItToEachServicesRetryConfig()
    {
        var appConfig = new AppConfig();
        new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    ["gPAS:Retry:Count"] = "1",
                    ["Vfps:Retry:Count"] = "2",
                    ["entici:Retry:Count"] = "3",
                    ["Mii:Retry:Count"] = "4",
                }
            )
            .Build()
            .Bind(appConfig);

        appConfig.GPas.Retry.Count.Should().Be(1);
        appConfig.Vfps.Retry.Count.Should().Be(2);
        appConfig.Entici.Retry.Count.Should().Be(3);
        appConfig.Mii.Retry.Count.Should().Be(4);
    }

    [Fact]
    public void LogErrorIfPseudonymizationServiceIsMissing_WithPseudonymizeRuleAndNoService_ShouldLogError()
    {
        var logger = A.Fake<ILogger>();

        AnonymizerEngineExtensions.LogErrorIfPseudonymizationServiceIsMissing(
            AnonymizerConfigurationManager.CreateFromYamlConfigString(PseudonymizeConfig),
            PseudonymizationServiceType.None,
            logger
        );

        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log)
                && call.GetArgument<LogLevel>(0) == LogLevel.Error
            )
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(PseudonymizeConfig, PseudonymizationServiceType.gPAS)]
    [InlineData(NoPseudonymizeConfig, PseudonymizationServiceType.None)]
    public void LogErrorIfPseudonymizationServiceIsMissing_WithServiceOrWithoutPseudonymizeRule_ShouldNotLog(
        string anonymizationConfig,
        PseudonymizationServiceType pseudonymizationService
    )
    {
        var logger = A.Fake<ILogger>();

        AnonymizerEngineExtensions.LogErrorIfPseudonymizationServiceIsMissing(
            AnonymizerConfigurationManager.CreateFromYamlConfigString(anonymizationConfig),
            pseudonymizationService,
            logger
        );

        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log))
            .MustNotHaveHappened();
    }
}
