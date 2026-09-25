using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace FhirPseudonymizer.Tests;

public class ResourceNodeIndexTests
{
    static ResourceNodeIndexTests()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();
    }

    private static Patient CreatePatient()
    {
        return new Patient
        {
            Id = "pid-1",
            Name =
            [
                new HumanName { Family = "Doe", Given = ["Jane"] },
                new HumanName { Family = "Roe" },
            ],
            Identifier =
            [
                new Identifier("https://example.com/mrn", "mrn-1"),
                new Identifier("https://example.com/other", "other-1"),
            ],
            Contact =
            [
                new Patient.ContactComponent { Name = new HumanName { Family = "Contact" } },
            ],
            // a sub-resource, which neither nodesByType nor nodesByName descend into
            Contained =
            [
                new Practitioner { Id = "pr-1", Name = [new HumanName { Family = "Doc" }] },
            ],
        };
    }

    [Theory]
    [InlineData("HumanName")]
    [InlineData("Identifier")]
    [InlineData("string")]
    [InlineData("Practitioner")]
    [InlineData("NoSuchType")]
    public void NodesByType_WithTheIndexInUse_ReturnsTheSameNodesAsWalkingTheResource(
        string typeName
    )
    {
        var resource = PocoNodeExtension.CreateRootNode(CreatePatient());
        var walked = FhirPathSymbolExtensions.NodesByType(resource, typeName).ToList();

        var index = new ResourceNodeIndex(resource);
        List<PocoNode> indexed;
        using (index.Use())
        {
            indexed = [.. FhirPathSymbolExtensions.NodesByType(resource, typeName)];
        }

        indexed.Should().Equal(walked, PocoNodeIdentityComparer.Instance.Equals);
    }

    [Theory]
    [InlineData("name")]
    [InlineData("family")]
    [InlineData("value")]
    [InlineData("contained")]
    public void NodesByName_WithTheIndexInUse_ReturnsTheSameNodesAsWalkingTheResource(string name)
    {
        var resource = PocoNodeExtension.CreateRootNode(CreatePatient());
        var walked = FhirPathSymbolExtensions.NodesByName(resource, name).ToList();

        var index = new ResourceNodeIndex(resource);
        List<PocoNode> indexed;
        using (index.Use())
        {
            indexed = [.. FhirPathSymbolExtensions.NodesByName(resource, name)];
        }

        indexed.Should().Equal(walked, PocoNodeIdentityComparer.Instance.Equals);
    }

    [Fact]
    public void NodesByType_CalledOnPartOfTheResource_OnlyReturnsNodesOfThatPart()
    {
        var resource = PocoNodeExtension.CreateRootNode(CreatePatient());
        var contact = resource.ChildrenByName("contact").Single();

        var index = new ResourceNodeIndex(resource);
        List<PocoNode> humanNames;
        using (index.Use())
        {
            humanNames = [.. FhirPathSymbolExtensions.NodesByType(contact, "HumanName")];
        }

        humanNames
            .Should()
            .ContainSingle()
            .Which.Poco.Should()
            .BeSameAs(((Patient)resource.Poco).Contact[0].Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task AnonymizeResourceAsync_AfterARuleReplacingNodes_LaterNodesByTypeRulesOnlySeeTheCurrentOnes(
        bool substituteNamesFirst
    )
    {
        // The first rule indexes the resource (via nodesByType) before replacing the names'
        // children. The replacement nodes are marked as already processed, so the second rule
        // only finds strings to hash if it still goes by the names' old, now detached, children.
        var substituteRule = substituteNamesFirst
            ? """
                  - path: nodesByType('HumanName')
                    method: substitute
                    replaceWith: '{"family": "Anonymous"}'
                """
            : "";
        var configManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
            $"""
            fhirVersion: R4
            fhirPathRules:
            {substituteRule}
              - path: nodesByType('string')
                method: cryptoHash
            parameters:
              dateShiftKey: ""
              dateShiftScope: resource
              cryptoHashKey: "secret"
              encryptKey: ""
              enablePartialAgesForRedact: true
              enablePartialDatesForRedact: true
              enablePartialZipCodesForRedact: true
              restrictedZipCodeTabulationAreas: []
            """
        );
        var engine = new AnonymizerEngine(configManager);
        var patient = new Patient
        {
            Name = [new HumanName { Family = "Doe", Given = ["Jane"] }],
            Gender = AdministrativeGender.Female,
        };

        var anonymized = (Patient)
            await engine.AnonymizeResourceAsync(
                patient,
                new AnonymizerSettings { ShouldAddSecurityTag = true },
                TestContext.Current.CancellationToken
            );

        var securityCodes = anonymized.Meta.Security.Select(coding => coding.Code).ToList();
        if (substituteNamesFirst)
        {
            anonymized.Name.Should().ContainSingle().Which.Family.Should().Be("Anonymous");
            securityCodes.Should().NotContain(SecurityLabels.CRYTOHASH.Code);
        }
        else
        {
            securityCodes.Should().Contain(SecurityLabels.CRYTOHASH.Code);
        }
    }
}
