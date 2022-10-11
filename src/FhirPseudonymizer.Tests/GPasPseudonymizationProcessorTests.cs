using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.FhirPath;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Xunit;

namespace FhirPseudonymizer.Tests;

public class GPasPseudonymizationProcessorTests
{
    public static IEnumerable<object[]> GetProcessData()
    {
        yield return new object[] { "foo-", "bar", new FhirString("12345"), "foo-bar" };
        yield return new object[] { null, "bar", new FhirString("12345"), "bar" };
        yield return new object[] { "foo-", null, new ResourceReference("Patient/12345"), "foo-Patient" };
        yield return new object[] { null, null, new ResourceReference("Patient/12345"), "Patient" };
    }

    [Theory]
    [MemberData(nameof(GetProcessData))]
    public void Process_SupportsDomainPrefixSetting(string domainPrefix, string domainName, DataType element,
        string expectedDomain)
    {
        var psnClient = A.Fake<IPseudonymServiceClient>();
        var processor = new PseudonymizationProcessor(psnClient);

        var node = ElementNode.FromElement(element.ToTypedElement());
        while (!node.HasValue())
        {
            node = node.Children().CastElementNodes().First();
        }

        processor.Process(node, null,
            new Dictionary<string, object> { { "domain", domainName }, { "domain-prefix", domainPrefix } });

        A.CallTo(() => psnClient.GetOrCreatePseudonymFor(A<string>._, expectedDomain))
            .MustHaveHappenedOnceExactly();
    }
}
