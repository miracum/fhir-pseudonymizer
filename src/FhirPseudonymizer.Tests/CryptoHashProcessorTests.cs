using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

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

    public static IEnumerable<object[]> GetUuidProcessData()
    {
        yield return new object[]
        {
            new FhirString("12345"),
            "098fe201-710c-856e-b3df-b56cb0c610a6",
        };
        yield return new object[]
        {
            new ResourceReference("Patient/12345"),
            "Patient/098fe201-710c-856e-b3df-b56cb0c610a6",
        };
        yield return new object[]
        {
            new FhirUri("Patient/12345"),
            "Patient/098fe201-710c-856e-b3df-b56cb0c610a6",
        };
    }

    [Theory]
    [MemberData(nameof(GetUuidProcessData))]
    public async Task Process_WithOutputFormatUuid_ReplacesHexDigestWithUuid(
        DataType element,
        string expected
    )
    {
        var result = await ProcessAsync(
            element,
            new Dictionary<string, object> { { "outputFormat", "uuid" } }
        );

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(CryptoHashAlgorithm.HmacSha256)]
    [InlineData(CryptoHashAlgorithm.Blake3)]
    public async Task Process_WithOutputFormatUuid_IsVersion8UuidDerivedFromHexDigest(
        CryptoHashAlgorithm algorithm
    )
    {
        var processor = new CryptoHashProcessor("test", algorithm);
        var hexNode = PocoNodeOrList.Root(new FhirString("12345"));
        var uuidNode = PocoNodeOrList.Root(new FhirString("12345"));

        await processor.ProcessAsync(hexNode);
        await processor.ProcessAsync(
            uuidNode,
            settings: new Dictionary<string, object> { { "outputFormat", "UUID" } }
        );

        var hex = hexNode.GetValue().ToString();
        var uuid = Guid.Parse(uuidNode.GetValue().ToString());
        uuid.Version.Should().Be(8);
        (uuid.Variant & 0b1100).Should().Be(0b1000);
        uuid.ToString("N")
            .Should()
            .Be(CryptoHashUtility.ToUuid(Convert.FromHexString(hex)).Replace("-", ""));
    }

    [Fact]
    public async Task Process_WithOutputFormatHex_KeepsHexDigest()
    {
        var result = await ProcessAsync(
            new FhirString("12345"),
            new Dictionary<string, object> { { "outputFormat", "hex" } }
        );

        result.Should().Be("098fe201710ca56e73dfb56cb0c610a66900add818c6d625b44b91eaafe79022");
    }

    [Theory]
    [InlineData("base64")]
    [InlineData("1")]
    [InlineData(null)]
    public void ValidateRuleSettings_WithUnsupportedOutputFormat_Throws(string outputFormat)
    {
        var rule = new Dictionary<string, object>
        {
            { "path", "Resource.id" },
            { "method", "cryptoHash" },
            { "outputFormat", outputFormat },
        };

        var act = () => CryptoHashSetting.ValidateRuleSettings(rule);

        act.Should().Throw<AnonymizerConfigurationErrorsException>();
    }

    [Fact]
    public void ValidateRuleSettings_WithOutputFormatUuidAndTruncation_Throws()
    {
        var rule = new Dictionary<string, object>
        {
            { "path", "Resource.id" },
            { "method", "cryptoHash" },
            { "outputFormat", "uuid" },
            { "truncateToMaxLength", 16 },
        };

        var act = () => CryptoHashSetting.ValidateRuleSettings(rule);

        act.Should().Throw<AnonymizerConfigurationErrorsException>().WithMessage("*truncat*");
    }

    [Theory]
    [InlineData("hex")]
    [InlineData("uuid")]
    public void ValidateRuleSettings_WithSupportedOutputFormat_DoesNotThrow(string outputFormat)
    {
        var rule = new Dictionary<string, object>
        {
            { "path", "Resource.id" },
            { "method", "cryptoHash" },
            { "outputFormat", outputFormat },
        };

        var act = () => CryptoHashSetting.ValidateRuleSettings(rule);

        act.Should().NotThrow();
    }
}

public class CryptoHashUtilityToUuidTests
{
    [Fact]
    public void ToUuid_SetsVersionAndVariantAndKeepsOtherBits()
    {
        var bytes = Enumerable.Repeat((byte)0xFF, 16).ToArray();

        CryptoHashUtility.ToUuid(bytes).Should().Be("ffffffff-ffff-8fff-bfff-ffffffffffff");
    }

    [Fact]
    public void ToUuid_UsesOnlyFirst16BytesInNetworkOrder()
    {
        var bytes = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();

        CryptoHashUtility.ToUuid(bytes).Should().Be("00010203-0405-8607-8809-0a0b0c0d0e0f");
    }
}
