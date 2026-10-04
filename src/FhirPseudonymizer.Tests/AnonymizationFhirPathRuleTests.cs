using Hl7.Fhir.Model;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;

namespace FhirPseudonymizer.Tests;

public class AnonymizationFhirPathRuleTests
{
    static AnonymizationFhirPathRuleTests()
    {
        AnonymizerEngine.InitializeFhirPathExtensionSymbols();
    }

    private static PocoNode CreatePatientNode()
    {
        var patient = new Patient
        {
            Id = "pid-1",
            Name = [new HumanName { Family = "Doe", Given = ["Jane"] }],
            Identifier =
            [
                new Identifier("https://example.com/mrn", "mrn-1")
                {
                    Type = new CodeableConcept(
                        "http://terminology.hl7.org/CodeSystem/v2-0203",
                        "MR"
                    ),
                },
                new Identifier("https://example.com/other", "other-1"),
            ],
            BirthDate = "1970-01-01",
        };

        return PocoNodeExtension.CreateRootNode(patient);
    }

    [Theory]
    [InlineData("nodesByType('HumanName')")]
    [InlineData("Patient.name.given")]
    [InlineData("Patient.identifier.where(system='https://example.com/other').value")]
    [InlineData(
        "nodesByType('Identifier').where(type.coding.where(system='http://terminology.hl7.org/CodeSystem/v2-0203' and code='MR').exists()).value"
    )]
    [InlineData("nodesByType('id')")]
    [InlineData("Patient.birthDate")]
    [InlineData("Patient.address")]
    public void Evaluate_SelectsTheSameNodesAsSelectingByTheExpressionString(string path)
    {
        var rule = AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule(
            new Dictionary<string, object> { ["path"] = path, ["method"] = "redact" }
        );
        var node = CreatePatientNode();

        var expected = node.Select(rule.Expression).Cast<PocoNode>().ToList();

        // PocoNode's record equality also compares its lazily-populated annotations, so compare
        // the positions in the resource they point at instead, like the anonymizer itself does
        rule.Evaluate(node).Should().Equal(expected, PocoNodeIdentityComparer.Instance.Equals);
        rule.Evaluate(node).Should().Equal(expected, PocoNodeIdentityComparer.Instance.Equals);
    }

    [Fact]
    public void Evaluate_AfterChangingTheExpression_UsesTheNewExpression()
    {
        var rule = AnonymizationFhirPathRule.CreateAnonymizationFhirPathRule(
            new Dictionary<string, object> { ["path"] = "Patient.name", ["method"] = "redact" }
        );
        var node = CreatePatientNode();
        rule.Evaluate(node).Should().ContainSingle();

        rule.Expression = "identifier";

        rule.Evaluate(node).Should().HaveCount(2);
    }
}
