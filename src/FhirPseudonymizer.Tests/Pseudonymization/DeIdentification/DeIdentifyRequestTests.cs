using FhirPseudonymizer.Pseudonymization.DeIdentification;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Tests.Pseudonymization.DeIdentification;

public class DeIdentifyRequestTests
{
    [Fact]
    public void FromFhirParameters_WithFullRequest_ParsesAllParts()
    {
        var parameters = new Parameters()
            .Add("fhirVersion", new FhirString("R4"))
            .Add(
                "fhirPathRules",
                new[]
                {
                    Tuple.Create<string, Base>("path", new FhirString("Patient.name")),
                    Tuple.Create<string, Base>("method", new FhirString("redact")),
                }
            )
            .Add(
                "fhirPathRules",
                new[]
                {
                    Tuple.Create<string, Base>("path", new FhirString("Resource.id")),
                    Tuple.Create<string, Base>("method", new FhirString("cryptoHash")),
                }
            )
            .Add(
                "parameters",
                new[]
                {
                    Tuple.Create<string, Base>("dateShiftKey", new FhirString("secret")),
                    Tuple.Create<string, Base>("dateShiftScope", new FhirString("resource")),
                    Tuple.Create<string, Base>("dateShiftFixedOffsetInDays", new FhirDecimal(30)),
                    Tuple.Create<string, Base>("cryptoHashKey", new FhirString("hash-key")),
                    Tuple.Create<string, Base>("encryptKey", new FhirString("encrypt-key")),
                    Tuple.Create<string, Base>("enablePartialAgesForRedact", new FhirBoolean(true)),
                    Tuple.Create<string, Base>(
                        "restrictedZipCodeTabulationAreas",
                        new FhirString("036")
                    ),
                    Tuple.Create<string, Base>(
                        "restrictedZipCodeTabulationAreas",
                        new FhirString("059")
                    ),
                }
            )
            .Add("resource", new Patient { Id = "example" });

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.FhirVersion.Should().Be("R4");

        request.FhirPathRules.Should().HaveCount(2);
        request.FhirPathRules[0].Path.Should().Be("Patient.name");
        request.FhirPathRules[0].Method.Should().Be("redact");
        request.FhirPathRules[1].Path.Should().Be("Resource.id");
        request.FhirPathRules[1].Method.Should().Be("cryptoHash");

        request.Parameters.Should().NotBeNull();
        request.Parameters.DateShiftKey.Should().Be("secret");
        request.Parameters.DateShiftScope.Should().Be("resource");
        request.Parameters.DateShiftFixedOffsetInDays.Should().Be(30);
        request.Parameters.CryptoHashKey.Should().Be("hash-key");
        request.Parameters.EncryptKey.Should().Be("encrypt-key");
        request.Parameters.EnablePartialAgesForRedact.Should().BeTrue();
        request.Parameters.RestrictedZipCodeTabulationAreas.Should().Equal("036", "059");

        request.Resource.Should().BeOfType<Patient>();
        ((Patient)request.Resource).Id.Should().Be("example");
    }

    [Fact]
    public void FromFhirParameters_WithoutOptionalParts_LeavesThemAtTheirDefault()
    {
        var parameters = new Parameters().Add("resource", new Patient());

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.FhirVersion.Should().BeEmpty();
        request.FhirPathRules.Should().BeEmpty();
        request.Parameters.Should().BeNull();
        request.Resource.Should().NotBeNull();
    }

    [Fact]
    public void FromFhirParameters_WithoutResourcePart_LeavesResourceNull()
    {
        var parameters = new Parameters().Add("fhirVersion", new FhirString("R4"));

        var request = DeIdentifyRequest.FromFhirParameters(parameters);

        request.Resource.Should().BeNull();
    }

    [Fact]
    public void FromFhirParameters_WithNullParameters_Throws()
    {
        Action act = () => DeIdentifyRequest.FromFhirParameters(null);

        act.Should().Throw<NullReferenceException>();
    }

    [Fact]
    public void ToFhirParameters_ThenFromFhirParameters_RoundTrips()
    {
        var original = new DeIdentifyRequest
        {
            FhirVersion = "R4",
            FhirPathRules =
            [
                new FhirPathRuleParameter { Path = "Patient.name", Method = "redact" },
            ],
            Parameters = new DeIdentifyParametersConfig
            {
                DateShiftKey = "secret",
                DateShiftScope = "resource",
                DateShiftFixedOffsetInDays = 30,
                CryptoHashKey = "hash-key",
                EncryptKey = "encrypt-key",
                EnablePartialAgesForRedact = true,
                EnablePartialDatesForRedact = false,
                EnablePartialZipCodesForRedact = true,
                RestrictedZipCodeTabulationAreas = ["036", "059"],
            },
            Resource = new Patient { Id = "example" },
        };

        var roundTripped = DeIdentifyRequest.FromFhirParameters(original.ToFhirParameters());

        roundTripped.Should().BeEquivalentTo(original);
    }
}
