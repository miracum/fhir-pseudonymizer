using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;

namespace FhirPseudonymizer.Tests;

public class CryptoHashProcessorTests
{
    public static IEnumerable<object[]> GetProcessData()
    {
        yield return new object[]
        {
            new FhirString("12345"),
            "098fe201710ca56e73dfb56cb0c610a66900add818c6d625b44b91eaafe79022",
        };
        yield return new object[]
        {
            new ResourceReference("Patient/12345"),
            "Patient/098fe201710ca56e73dfb56cb0c610a66900add818c6d625b44b91eaafe79022",
        };
        yield return new object[]
        {
            new FhirUri("Patient/12345"),
            "Patient/098fe201710ca56e73dfb56cb0c610a66900add818c6d625b44b91eaafe79022",
        };
    }

    public static IEnumerable<object[]> GetTruncatedProcessData()
    {
        yield return new object[] { new FhirString("12345"), "098fe201710ca56e73dfb56cb0c610a6" };
        yield return new object[]
        {
            new ResourceReference("Patient/12345"),
            "Patient/098fe201710ca56e73dfb56cb0c610a6",
        };
        yield return new object[]
        {
            new FhirUri("Patient/12345"),
            "Patient/098fe201710ca56e73dfb56cb0c610a6",
        };
    }

    [Theory]
    [MemberData(nameof(GetProcessData))]
    public void Process_HashesIdPart(DataType element, string expected)
    {
        var processor = new CryptoHashProcessor("test");

        var node = ElementNode.FromElement(element.ToTypedElement());
        while (!node.HasValue())
        {
            node = node.Children().CastElementNodes().First();
        }

        processor.Process(node);

        node.Value.ToString().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(GetTruncatedProcessData))]
    public void Process_WithTruncatedHashLengthSet_HashHasMaxLength(
        DataType element,
        string expected
    )
    {
        var processor = new CryptoHashProcessor("test");

        var node = ElementNode.FromElement(element.ToTypedElement());
        while (!node.HasValue())
        {
            node = node.Children().CastElementNodes().First();
        }

        processor.Process(
            node,
            settings: new Dictionary<string, object>() { { "truncateToMaxLength", 32 } }
        );

        node.Value.ToString().Should().Be(expected);
    }
}
