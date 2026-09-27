using Confluent.Kafka;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Tests.Kafka;

/// <summary>
///     The other provenance tests fake IAnonymizerEngine, so they cannot see how the real engine
///     treats the resource it is given: it anonymizes in place and hands back that same instance.
///     Whatever reaches <see cref="IProvenancePublisher.Publish" /> as the original therefore has
///     to have been snapshotted beforehand, or the Provenance's entity[role=source] ends up
///     describing the pseudonymized resource instead of the one it was derived from.
/// </summary>
public class ProvenanceWithRealEngineTests
{
    private const string HashIdConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: Patient.id
            method: cryptoHash
        parameters:
          cryptoHashKey: a-key
        """;

    private static AnonymizerEngine CreateEngine()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, HashIdConfig);
        return new AnonymizerEngine(AnonymizerConfigurationManager.CreateFromYamlConfigFile(path));
    }

    private static KafkaProvenancePublisher CreatePublisher()
    {
        return new KafkaProvenancePublisher(
            A.Fake<IProducer<byte[], string>>(),
            new KafkaConfig { ProvenanceTopic = "provenance-topic" },
            A.Fake<ILogger<KafkaProvenancePublisher>>()
        );
    }

    [Fact]
    public async Task AnonymizeResourceAsync_MutatesTheGivenResourceInPlace()
    {
        var resource = new Patient { Id = "original-id" };

        var anonymized = await CreateEngine().AnonymizeResourceAsync(resource);

        // Documents the engine contract the provenance call sites have to work around.
        anonymized.Should().BeSameAs(resource);
        resource.Id.Should().NotBe("original-id");
    }

    [Fact]
    public async Task CapturedPreImage_StillIdentifiesTheSourceResourceAfterAnonymization()
    {
        var resource = new Patient { Id = "original-id" };

        // What the call sites do: snapshot first, then anonymize the live instance.
        var preImage = CreatePublisher().CapturePreImage(resource);
        var anonymized = await CreateEngine().AnonymizeResourceAsync(resource);

        var bundle = ProvenanceFactory.CreateBundle(preImage, anonymized, DateTimeOffset.UtcNow);

        var provenance = bundle.Entry.Select(e => e.Resource).OfType<Provenance>().Single();
        provenance.Target.Should().ContainSingle(t => t.Reference == $"Patient/{anonymized.Id}");
        provenance.Entity.Should().ContainSingle(e => e.What.Reference == "Patient/original-id");
    }
}
