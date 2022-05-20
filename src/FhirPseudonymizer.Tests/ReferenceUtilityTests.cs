using Xunit;

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
        Assert.Equal(ReferenceUtility.TransformReferenceId(uri, _ => "xxx"), uri.Replace("123", "xxx"));
    }

}
