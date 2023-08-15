using FhirPseudonymizer.Controllers;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests;

public class FhirControllerTests
{
    [Fact]
    public void DeIdentify_ParsesDynamicSettings()
    {
        const string domainPrefix = "domain-prefix";
        var domainPrefixValue = new FhirString("test-");
        Dictionary<string, object> ruleSettings = null;

        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes((Resource _, AnonymizerSettings s) => ruleSettings = s?.DynamicRuleSettings);

        var controller = new FhirController(
            A.Fake<IConfiguration>(),
            A.Fake<ILogger<FhirController>>(),
            anonymizer,
            A.Fake<IDePseudonymizerEngine>()
        );

        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<string, Base>(domainPrefix, domainPrefixValue) })
            .Add("resource", new Patient());

        controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(domainPrefix).WhoseValue.Should().Be(domainPrefixValue);
    }

    [Fact]
    public void DeIdentify_WithExceptionThrownInAnonymizer_ShouldReturnInternalError()
    {
        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .Throws(new Exception("something went wrong"));

        var controller = new FhirController(
            A.Fake<IConfiguration>(),
            A.Fake<ILogger<FhirController>>(),
            anonymizer,
            A.Fake<IDePseudonymizerEngine>()
        );

        var response = controller.DeIdentify(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }

    [Fact]
    public void DePseudonymize_WithExceptionThrownInDePseudonymizer_ShouldReturnInternalError()
    {
        var dePseudonymizer = A.Fake<IDePseudonymizerEngine>();
        A.CallTo(
                () => dePseudonymizer.DePseudonymizeResource(A<Resource>._, A<AnonymizerSettings>._)
            )
            .Throws(new Exception("something went wrong"));

        var controller = new FhirController(
            A.Fake<IConfiguration>(),
            A.Fake<ILogger<FhirController>>(),
            A.Fake<IAnonymizerEngine>(),
            dePseudonymizer
        );

        var response = controller.DePseudonymize(new Bundle());

        response.StatusCode.Should().Be(500);

        response.Value.Should().BeOfType<OperationOutcome>();
    }
}
