namespace Microsoft.Health.Fhir.Anonymizer.Core.Utility;

public class ReferenceUtilityTests
{
    [Theory]
    [InlineData("Patient/123")]
    [InlineData("Encounter?identifier=123")]
    [InlineData("Patient?identifier=http://fhir.test.de/sid/patient-id|123")]
    public void IsResourceReference_MatchesConditionalReferences(string uri)
    {
        Assert.True(ReferenceUtility.IsResourceReference(uri));
    }

    [Theory]
    [InlineData("Patient/123")]
    [InlineData("Encounter?identifier=123")]
    [InlineData("Patient?identifier=http://fhir.test.de/sid/patient-id|123")]
    [InlineData("identifier=http://fhir.test.de/sid/patient-id|123")]
    public void TransformReferenceId_MatchesConditionalReferences(string uri)
    {
        Assert.Equal(
            ReferenceUtility.TransformReferenceId(uri, _ => "xxx"),
            uri.Replace("123", "xxx")
        );
    }

    [Theory]
    [InlineData("https://example.de/fhir/Patient/asd")]
    [InlineData("https://exa-mple.de/fhir/Patient/asd")]
    [InlineData("https://my-fhir.example.com/Patient/asd")]
    [InlineData("https://example.org/fhir-r4/Patient/asd")]
    [InlineData("https://example.org/fhir_r4/Patient/asd")]
    [InlineData("https://example.org/~team/Patient/asd")]
    [InlineData("https://example.org:8080/fhir/Patient/asd")]
    public void IsResourceReference_MatchesLiteralUrlsWithUnreservedCharacters(string uri)
    {
        Assert.True(ReferenceUtility.IsResourceReference(uri));
    }

    [Theory]
    [InlineData("https://exa-mple.de/fhir/Patient/asd", "https://exa-mple.de/fhir/Patient/xxx")]
    [InlineData(
        "https://example.org/fhir-r4/Patient/asd",
        "https://example.org/fhir-r4/Patient/xxx"
    )]
    [InlineData(
        "https://exa-mple.de/fhir/Encounter/asd/_history/2",
        "https://exa-mple.de/fhir/Encounter/xxx/_history/2"
    )]
    public void TransformReferenceId_HashesOnlyTheIdOfHyphenatedUrls(string uri, string expected)
    {
        Assert.Equal(expected, ReferenceUtility.TransformReferenceId(uri, _ => "xxx"));
    }

    [Theory]
    [InlineData("https://example.org/fhir/sid/patient-id")]
    [InlineData("http://fhir.de/sid/gkv/kvid-10")]
    [InlineData("https://example.org/fhir/Patient")]
    [InlineData("urn:oid:1.2.840.113619.2.55")]
    [InlineData("just-a-plain-string")]
    public void IsResourceReference_StillRejectsNonReferences(string uri)
    {
        Assert.False(ReferenceUtility.IsResourceReference(uri));
    }
}
