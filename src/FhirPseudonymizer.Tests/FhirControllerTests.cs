using FhirPseudonymizer.Config;
using FhirPseudonymizer.Controllers;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Projects;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests;

public class FhirControllerTests
{
    private static ServerEngines ServerEnginesOf(
        IAnonymizerEngine anonymizer,
        IDePseudonymizerEngine dePseudonymizer
    )
    {
        return new ServerEngines(new ProjectEngines(anonymizer, dePseudonymizer));
    }

    [Fact]
    public async Task DeIdentify_ParsesDynamicSettings()
    {
        const string domainPrefix = "domain-prefix";
        var domainPrefixValue = new FhirString("test-");
        Dictionary<string, object> ruleSettings = null;

        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes((Resource _, AnonymizerSettings s) => ruleSettings = s?.DynamicRuleSettings)
            .Returns(new Patient());

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(anonymizer, A.Fake<IDePseudonymizerEngine>()),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectConfigProvider>()
        );

        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<string, Base>(domainPrefix, domainPrefixValue) })
            .Add("resource", new Patient());

        await controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(domainPrefix).WhoseValue.Should().Be(domainPrefixValue);
    }

    [Fact]
    public async Task DeIdentify_ParsesDateShiftFixedOffsetInDaysSetting()
    {
        const string settingKey = "dateShiftFixedOffsetInDays";
        var settingValue = new Integer(30);
        Dictionary<string, object> ruleSettings = null;

        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes((Resource _, AnonymizerSettings s) => ruleSettings = s?.DynamicRuleSettings)
            .Returns(new Patient());

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(anonymizer, A.Fake<IDePseudonymizerEngine>()),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectConfigProvider>()
        );

        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<string, Base>(settingKey, settingValue) })
            .Add("resource", new Patient());

        await controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(settingKey).WhoseValue.Should().Be(settingValue);
    }

    [Fact]
    public async Task DeIdentify_WithExceptionThrownInAnonymizer_ShouldReturnInternalError()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Throws(new Exception("something went wrong"));

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(anonymizer, A.Fake<IDePseudonymizerEngine>()),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectConfigProvider>()
        );

        var response = await controller.DeIdentify(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public async Task DeIdentify_PublishesProvenanceForTheOriginalAndAnonymizedResource()
    {
        var original = new Patient { Id = "123" };
        var anonymized = new Patient { Id = "hashed-123" };
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._))
            .Returns(anonymized);

        var provenancePublisher = A.Fake<IProvenancePublisher>();

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(anonymizer, A.Fake<IDePseudonymizerEngine>()),
            provenancePublisher,
            A.Fake<IProjectConfigProvider>()
        );

        await controller.DeIdentify(original);

        A.CallTo(() => provenancePublisher.Publish(original, anonymized, null))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task DePseudonymize_DoesNotPublishProvenance()
    {
        var dePseudonymizer = A.Fake<IDePseudonymizerEngine>();
        A.CallTo(() =>
                dePseudonymizer.DePseudonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._)
            )
            .Returns(new Patient());

        var provenancePublisher = A.Fake<IProvenancePublisher>();

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(A.Fake<IAnonymizerEngine>(), dePseudonymizer),
            provenancePublisher,
            A.Fake<IProjectConfigProvider>()
        );

        await controller.DePseudonymize(new Patient { Id = "123" });

        A.CallTo(() =>
                provenancePublisher.Publish(
                    A<Resource>._,
                    A<Resource>._,
                    A<Confluent.Kafka.Headers>._
                )
            )
            .MustNotHaveHappened();
    }

    [Fact]
    public async Task DePseudonymize_WithExceptionThrownInDePseudonymizer_ShouldReturnInternalError()
    {
        var dePseudonymizer = A.Fake<IDePseudonymizerEngine>();
        A.CallTo(() =>
                dePseudonymizer.DePseudonymizeResourceAsync(A<Resource>._, A<AnonymizerSettings>._)
            )
            .Throws(new Exception("something went wrong"));

        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(A.Fake<IAnonymizerEngine>(), dePseudonymizer),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectConfigProvider>()
        );

        var response = await controller.DePseudonymize(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }
}
