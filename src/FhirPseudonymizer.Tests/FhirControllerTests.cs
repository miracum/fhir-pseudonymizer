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
    private static ProjectEngines ServerEnginesOf(
        IAnonymizerEngine anonymizer,
        IDePseudonymizerEngine dePseudonymizer
    )
    {
        return new ProjectEngines(anonymizer, dePseudonymizer);
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
            A.Fake<IProjectRegistry>()
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
            A.Fake<IProjectRegistry>()
        );

        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<string, Base>(settingKey, settingValue) })
            .Add("resource", new Patient());

        await controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(settingKey).WhoseValue.Should().Be(settingValue);
    }

    /// <summary>
    ///     A name the registration endpoint would reject cannot name a registered Project, so
    ///     <c>$de-identify</c> refuses it outright. That keeps it out of the log and out of the
    ///     response, which is what bounds both: nothing but the body size limit bounds the name a
    ///     Parameters body carries.
    /// </summary>
    [Theory]
    [InlineData("innocent\nERROR: forged log entry", "a line break would forge a second log entry")]
    [InlineData("has a space", "a space is outside the allowed set")]
    public async Task DeIdentify_WithAnUnusableProjectName_ShouldRejectItWithoutRepeatingIt(
        string projectName,
        string because
    )
    {
        var logger = new RecordingLogger<FhirController>();
        var controller = ControllerWith(logger);

        var parameters = new Parameters()
            .Add("project", new FhirString(projectName))
            .Add("resource", new Patient());

        var response = await controller.DeIdentify(parameters);

        response.StatusCode.Should().Be(400, because);
        logger.Messages.Should().OnlyContain(message => !message.Contains('\n'));
        response
            .Value.Should()
            .BeOfType<OperationOutcome>()
            .Which.Issue.Should()
            .ContainSingle()
            .Which.Diagnostics.Should()
            .NotContain(projectName, "a rejected name is the caller's, not something to echo");
    }

    [Fact]
    public async Task DeIdentify_WithAFloodingProjectName_ShouldNotFloodTheLogOrTheResponse()
    {
        var logger = new RecordingLogger<FhirController>();
        var controller = ControllerWith(logger);

        var parameters = new Parameters()
            .Add("project", new FhirString(new string('a', 10_000)))
            .Add("resource", new Patient());

        var response = await controller.DeIdentify(parameters);

        response.StatusCode.Should().Be(400, "10 000 characters is past the 64 a name may hold");
        logger.Messages.Should().OnlyContain(message => message.Length < 200);
        response
            .Value.Should()
            .BeOfType<OperationOutcome>()
            .Which.Issue.Should()
            .ContainSingle()
            .Which.Diagnostics.Length.Should()
            .BeLessThan(500, "a caller must not be able to size the response from the body");
    }

    /// <summary>
    ///     A Project-less controller: its registry holds nothing, so any name reaching it is
    ///     unknown.
    /// </summary>
    private static FhirController ControllerWith(RecordingLogger<FhirController> logger)
    {
        return new FhirController(
            A.Fake<AnonymizationConfig>(),
            logger,
            ServerEnginesOf(A.Fake<IAnonymizerEngine>(), A.Fake<IDePseudonymizerEngine>()),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectRegistry>()
        );
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
            A.Fake<IProjectRegistry>()
        );

        var response = await controller.DeIdentify(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public async Task DeIdentify_WithParametersCarryingNoResource_ShouldReturnBadRequest()
    {
        var controller = new FhirController(
            A.Fake<AnonymizationConfig>(),
            A.Fake<ILogger<FhirController>>(),
            ServerEnginesOf(A.Fake<IAnonymizerEngine>(), A.Fake<IDePseudonymizerEngine>()),
            A.Fake<IProvenancePublisher>(),
            A.Fake<IProjectRegistry>()
        );

        var response = await controller.DeIdentify(new Parameters());

        response.StatusCode.Should().Be(400);

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
            A.Fake<IProjectRegistry>()
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
            A.Fake<IProjectRegistry>()
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
            A.Fake<IProjectRegistry>()
        );

        var response = await controller.DePseudonymize(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }
}
