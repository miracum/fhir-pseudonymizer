using FhirPseudonymizer.Pseudonymization.Mii;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Tests.Pseudonymization;

/// <summary>
/// The fixtures are the example instances of the MII Pseudonymization Implementation Guide
/// 2026.1.0. They are the contract between this client and an MII backend.
/// </summary>
public class MiiParametersTests
{
    private const string DePseudonymizeResponseExample = """
        {
            "resourceType": "Parameters",
            "id": "DePseudonymizeResponseWithIdentifierExample",
            "parameter": [
                {
                    "name": "original",
                    "part": [
                        {
                            "name": "context",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "Transfer1"
                            }
                        },
                        {
                            "name": "value",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "D1CL0CAL1"
                            }
                        },
                        {
                            "name": "pseudonym",
                            "valueIdentifier": {
                                "system": "https://sample/psn-system",
                                "value": "H3RAU56A8E"
                            }
                        }
                    ]
                }
            ]
        }
        """;

    [Fact]
    public void FromFhirParameters_WithIgDePseudonymizeResponse_ShouldReadTheOriginalValue()
    {
        var parameters = new FhirJsonParser().Parse<Parameters>(DePseudonymizeResponseExample);

        var response = MiiDePseudonymizeResponse.FromFhirParameters(parameters);

        response.Original.Should().HaveCount(1);
        response.Original[0].Value.Value.Should().Be("D1CL0CAL1");
    }
}
