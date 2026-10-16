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
    public async Task Process_HashesIdPart(DataType element, string expected)
    {
        var processor = new CryptoHashProcessor("test");

        var node = PocoNodeOrList.Root(element);
        while (node.GetValue() == null)
        {
            node = node.Children().CastPocoNodes().First();
        }

        await processor.ProcessAsync(node);

        node.GetValue().ToString().Should().Be(expected);
    }

    private static async Task<string> ProcessAsync(
        DataType element,
        Dictionary<string, object> settings = null
    )
    {
        var processor = new CryptoHashProcessor("test");

        var node = PocoNodeOrList.Root(element);
        while (node.GetValue() == null)
        {
            node = node.Children().CastPocoNodes().First();
        }

        await processor.ProcessAsync(node, settings: settings);

        return node.GetValue().ToString();
    }

    [Fact]
    public async Task Process_UriLookingLikeReference_HashesOnlyIdByDefault()
    {
        var result = await ProcessAsync(new FhirUri("http://example.com/fhir/NamingSystem/test"));

        result.Should().StartWith("http://example.com/fhir/NamingSystem/").And.NotEndWith("/test");
    }

    [Theory]
    [InlineData(true)]
    [InlineData("true")]
    public async Task Process_UriLookingLikeReferenceWithTransformWholeUri_HashesWholeUri(
        object transformWholeUri
    )
    {
        const string uri = "http://example.com/fhir/NamingSystem/test";
        var expected = await ProcessAsync(new FhirString(uri));

        var result = await ProcessAsync(
            new FhirUri(uri),
            new Dictionary<string, object> { { "transformWholeUri", transformWholeUri } }
        );

        result.Should().Be(expected);
        result.Should().NotContain("example.com");
    }

    [Fact]
    public async Task Process_UriLookingLikeReferenceWithTransformWholeUriAndTruncation_TruncatesWholeUriHash()
    {
        const string uri = "http://example.com/fhir/NamingSystem/test";
        var full = await ProcessAsync(new FhirString(uri));

        var result = await ProcessAsync(
            new FhirUri(uri),
            new Dictionary<string, object>
            {
                { "transformWholeUri", true },
                { "truncateToMaxLength", 16 },
            }
        );

        result.Should().Be(full[..16]);
    }

    [Fact]
    public async Task Process_ReferenceStringWithTransformWholeUri_StillHashesOnlyId()
    {
        var result = await ProcessAsync(
            new ResourceReference("Patient/12345"),
            new Dictionary<string, object> { { "transformWholeUri", true } }
        );

        result
            .Should()
            .Be("Patient/098fe201710ca56e73dfb56cb0c610a66900add818c6d625b44b91eaafe79022");
    }

    [Theory]
    [MemberData(nameof(GetTruncatedProcessData))]
    public async Task Process_WithTruncatedHashLengthSet_HashHasMaxLength(
        DataType element,
        string expected
    )
    {
        var processor = new CryptoHashProcessor("test");

        var node = PocoNodeOrList.Root(element);
        while (node.GetValue() == null)
        {
            node = node.Children().CastPocoNodes().First();
        }

        await processor.ProcessAsync(
            node,
            settings: new Dictionary<string, object>() { { "truncateToMaxLength", 32 } }
        );

        node.GetValue().ToString().Should().Be(expected);
    }
}
