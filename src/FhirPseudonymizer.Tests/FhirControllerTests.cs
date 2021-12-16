using System;
using System.Collections.Generic;
using Autofac.Extras.Moq;
using FhirPseudonymizer.Controllers;
using FluentAssertions;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Moq;
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

        // setup mock and create controller fixture
        using var mock = AutoMock.GetLoose();
        var controller = mock.Create<FhirController>();
        mock.Mock<IAnonymizerEngine>()
            .Setup(_ => _.AnonymizeResource(It.IsAny<Resource>(), It.IsAny<AnonymizerSettings>()))
            .Callback<Resource, AnonymizerSettings>((_, s) => ruleSettings = s?.DynamicRuleSettings);


        var parameters = new Parameters()
            .Add("settings", new[] { Tuple.Create<String, Base>(domainPrefix, domainPrefixValue) })
            .Add("resource", new Patient());

        controller.DeIdentify(parameters);

        ruleSettings.Should().ContainKey(domainPrefix).WhoseValue.Should().Be(domainPrefixValue);
    }
}
