using FhirPseudonymizer.Config;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;

namespace FhirPseudonymizer.Tests.Pseudonymization;

public class GPasPseudonymizationProcessorTests
{
    public static IEnumerable<object[]> GetProcessData()
    {
        foreach (var enableConditionalReferencePseudonymization in new[] { true, false })
        {
            yield return new object[]
            {
                "foo-",
                "bar",
                new FhirString("12345"),
                "foo-bar",
                enableConditionalReferencePseudonymization
            };
            yield return new object[]
            {
                null,
                "bar",
                new FhirString("12345"),
                "bar",
                enableConditionalReferencePseudonymization
            };
            yield return new object[]
            {
                "foo-",
                null,
                new ResourceReference("Patient/12345"),
                "foo-Patient",
                enableConditionalReferencePseudonymization
            };
            yield return new object[]
            {
                null,
                null,
                new ResourceReference("Patient/12345"),
                "Patient",
                enableConditionalReferencePseudonymization
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetProcessData))]
    public void Process_SupportsDomainPrefixSetting(
        string domainPrefix,
        string domainName,
        DataType element,
        string expectedDomain,
        bool enableConditionalReferencePseudonymization
    )
    {
        var features = new FeatureManagement()
        {
            ConditionalReferencePseudonymization = enableConditionalReferencePseudonymization,
        };
        var psnClient = A.Fake<IPseudonymServiceClient>();
        var processor = new PseudonymizationProcessor(psnClient, features);

        var node = ElementNode.FromElement(element.ToTypedElement());
        while (!node.HasValue())
        {
            node = node.Children().CastElementNodes().First();
        }

        processor.Process(
            node,
            null,
            new Dictionary<string, object>
            {
                { "domain", domainName },
                { "domain-prefix", domainPrefix }
            }
        );

        A.CallTo(
                () =>
                    psnClient.GetOrCreatePseudonymFor(
                        A<string>._,
                        expectedDomain,
                        A<IReadOnlyDictionary<string, object>>._
                    )
            )
            .MustHaveHappenedOnceExactly();
    }
}
