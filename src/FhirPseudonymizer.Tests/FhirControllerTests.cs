using System;
using System.Collections.Generic;
using FakeItEasy;
using FhirPseudonymizer.Controllers;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Xunit;

namespace FhirPseudonymizer.Tests;

public class FhirControllerTests
{
    [Fact]
    public void DeIdentify_ParsesDynamicSettings()
    {
        var domainPrefix = "domain-prefix";
        var domainPrefixValue = new FhirString("test-");
        Dictionary<string, object> ruleSettings = null;

        var anonymizer = A.Fake<IAnonymizerEngine>();
        A.CallTo(() => anonymizer.AnonymizeResource(A<Resource>._, A<AnonymizerSettings>._))
            .Invokes((Resource _, AnonymizerSettings s) => ruleSettings = s?.DynamicRuleSettings);

        var controller = new FhirController(A.Fake<IConfiguration>(), A.Fake<ILogger<FhirController>>(),
            anonymizer, A.Fake<IDePseudonymizerEngine>());

        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<String, Base>(domainPrefix, domainPrefixValue) })
            .Add("resource", new Patient());

        controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(domainPrefix).WhoseValue.Should().Be(domainPrefixValue);
    }
}
