using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using FhirPseudonymizer.Kafka;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace FhirPseudonymizer.Tests.Kafka;

public class ProvenanceFactoryTests
{
    private static readonly DateTimeOffset Recorded = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private static readonly string ExpectedDeviceVersion =
        Assembly.GetAssembly(typeof(ProvenanceFactory))!.GetName().Version?.ToString(3)
        ?? "unknown";

    private static readonly string ExpectedDeviceId = Convert.ToHexStringLower(
        SHA256.HashData(
            Encoding.UTF8.GetBytes($"{ProvenanceFactory.AgentDisplay}|{ExpectedDeviceVersion}")
        )
    );

    private static Provenance GetProvenance(Bundle bundle) =>
        bundle.Entry.Select(e => e.Resource).OfType<Provenance>().Single();

    private static Device GetDevice(Bundle bundle) =>
        bundle.Entry.Select(e => e.Resource).OfType<Device>().Single();

    [Fact]
    public void CreateBundle_WithSingleResource_ReturnsBundleWithOneProvenanceTargetingIt()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        bundle.Should().NotBeNull();
        bundle.Type.Should().Be(Bundle.BundleType.Transaction);

        var provenance = GetProvenance(bundle);
        provenance.Target.Should().ContainSingle();
        provenance.Target.Single().Reference.Should().Be("Patient/hashed-123");
        provenance.Recorded.Should().Be(Recorded);
    }

    [Fact]
    public void CreateBundle_SetsTheBundlesIdToTheProvenancesId()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        bundle.Id.Should().Be(GetProvenance(bundle).Id);
    }

    [Fact]
    public void CreateBundle_IncludesThePseudonymizerDeviceAsAPutEntry()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var deviceEntry = bundle.Entry.Single(e => e.Resource is Device);
        var device = (Device)deviceEntry.Resource;

        device.Id.Should().Be(ExpectedDeviceId);
        device.DeviceName.Should().ContainSingle(n => n.Name == ProvenanceFactory.AgentDisplay);
        device.Version.Should().ContainSingle(v => v.Value == ExpectedDeviceVersion);
        device
            .Identifier.Should()
            .ContainSingle(i =>
                i.System == "https://miracum.github.io/fhir-pseudonymizer/identifiers/device-id"
                && i.Value == $"fhir-pseudonymizer-v{ExpectedDeviceVersion}"
            );

        deviceEntry.FullUrl.Should().Be($"Device/{device.Id}");
        deviceEntry.Request.Method.Should().Be(Bundle.HTTPVerb.PUT);
        deviceEntry.Request.Url.Should().Be($"Device/{device.Id}");
    }

    [Fact]
    public void CreateBundle_ReferencesThePseudonymizerDeviceAsTheAgentWho()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = GetProvenance(bundle);
        var device = GetDevice(bundle);

        provenance.Agent.Single().Who.Reference.Should().Be($"Device/{device.Id}");
    }

    [Fact]
    public void CreateBundle_CalledTwice_ReferencesTheSameDeviceIdBothTimes()
    {
        var firstDevice = GetDevice(
            ProvenanceFactory.CreateBundle(null, new Patient { Id = "1" }, Recorded)
        );
        var secondDevice = GetDevice(
            ProvenanceFactory.CreateBundle(null, new Patient { Id = "2" }, Recorded)
        );

        firstDevice.Id.Should().Be(secondDevice.Id);
    }

    [Fact]
    public void CreateBundle_WithSingleResource_SetsAgentDisplay()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = GetProvenance(bundle);
        provenance
            .Agent.Should()
            .ContainSingle(a => a.Who.Display == ProvenanceFactory.AgentDisplay);
    }

    [Fact]
    public void CreateBundle_WithSingleResource_SetsAgentTypeAndRole()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var agent = GetProvenance(bundle).Agent.Single();

        agent
            .Type.Coding.Should()
            .ContainSingle(c =>
                c.System == "http://terminology.hl7.org/CodeSystem/provenance-participant-type"
                && c.Code == "performer"
            );
        agent
            .Role.Should()
            .ContainSingle(role =>
                role.Coding.Any(c =>
                    c.System == "http://terminology.hl7.org/CodeSystem/v3-ParticipationType"
                    && c.Code == "AUT"
                )
            );
    }

    [Fact]
    public void CreateBundle_WithOriginalResourceHavingAnId_SetsSourceEntityReferencingItByTypeSlashId()
    {
        var original = new Patient { Id = "123" };
        var pseudonymized = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = GetProvenance(bundle);
        provenance.Entity.Should().ContainSingle();
        var entity = provenance.Entity.Single();
        entity.Role.Should().Be(Provenance.ProvenanceEntityRole.Source);
        entity.What.Reference.Should().Be("Patient/123");
    }

    [Fact]
    public void CreateBundle_WithOriginalResourceHavingNoId_SetsSourceEntityReferencingItByIdentifier()
    {
        var original = new Patient
        {
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };
        var pseudonymized = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var entity = GetProvenance(bundle).Entity.Single();
        entity.Role.Should().Be(Provenance.ProvenanceEntityRole.Source);
        entity.What.Reference.Should().BeNull();
        entity.What.Identifier.System.Should().Be("http://example.org/mrn");
        entity.What.Identifier.Value.Should().Be("12345");
    }

    [Fact]
    public void CreateBundle_WithNoOriginalIdOrIdentifierAvailable_OmitsSourceEntity()
    {
        var pseudonymized = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, pseudonymized, Recorded);

        GetProvenance(bundle).Entity.Should().BeEmpty();
    }

    [Fact]
    public void CreateBundle_WithBundleInput_SetsOneSourceEntityPerContainedResource()
    {
        var originalBundle = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent { Resource = new Patient { Id = "patient-1" } },
                new Bundle.EntryComponent { Resource = new Observation { Id = "obs-1" } },
            ],
        };
        var pseudonymizedBundle = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent { Resource = new Patient { Id = "hashed-patient-1" } },
                new Bundle.EntryComponent { Resource = new Observation { Id = "hashed-obs-1" } },
            ],
        };

        var bundle = ProvenanceFactory.CreateBundle(originalBundle, pseudonymizedBundle, Recorded);

        var sources = GetProvenance(bundle).Entity.Select(e => e.What.Reference).ToList();
        sources.Should().BeEquivalentTo(["Patient/patient-1", "Observation/obs-1"]);
    }

    [Fact]
    public void CreateBundle_SetsFixedDeidentifyActivity()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = GetProvenance(bundle);
        provenance
            .Activity.Coding.Should()
            .ContainSingle(c =>
                c.System == "http://terminology.hl7.org/CodeSystem/iso-21089-lifecycle"
                && c.Code == "deidentify"
                && c.Display == "De-Identify (Anononymize) Record Lifecycle Event"
            );
    }

    [Fact]
    public void CreateBundle_SetsTheSameFixedActivityRegardlessOfTheResourcesSecurityLabels()
    {
        var original = new Patient { Id = "123" };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Meta = new Meta { Security = [SecurityLabels.CRYTOHASH, SecurityLabels.PSEUDED] },
        };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = GetProvenance(bundle);
        provenance.Activity.Coding.Should().ContainSingle();
        provenance.Activity.Coding.Single().Code.Should().Be("deidentify");
    }

    [Fact]
    public void CreateBundle_WithBundleInput_ReturnsSingleProvenanceTargetingAllContainedResources()
    {
        var bundleInput = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent { Resource = new Patient { Id = "patient-1" } },
                new Bundle.EntryComponent { Resource = new Observation { Id = "obs-1" } },
            ],
        };

        var bundle = ProvenanceFactory.CreateBundle(null, bundleInput, Recorded);

        var provenance = GetProvenance(bundle);
        var targets = provenance.Target.Select(t => t.Reference).ToList();
        targets.Should().BeEquivalentTo(["Patient/patient-1", "Observation/obs-1"]);
    }

    [Fact]
    public void CreateBundle_WithBundleInput_ExcludesNestedBundlesAndProvenanceEntries()
    {
        var bundleInput = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent { Resource = new Patient { Id = "patient-1" } },
                new Bundle.EntryComponent { Resource = new Bundle { Id = "nested-bundle" } },
                new Bundle.EntryComponent
                {
                    Resource = new Provenance { Id = "existing-provenance" },
                },
            ],
        };

        var bundle = ProvenanceFactory.CreateBundle(null, bundleInput, Recorded);

        GetProvenance(bundle).Target.Single().Reference.Should().Be("Patient/patient-1");
    }

    [Fact]
    public void CreateBundle_WithResourceWithoutId_ReturnsNull()
    {
        var patient = new Patient();

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        bundle.Should().BeNull();
    }

    [Fact]
    public void CreateBundle_WithEmptyBundle_ReturnsNull()
    {
        var bundleInput = new Bundle();

        var bundle = ProvenanceFactory.CreateBundle(null, bundleInput, Recorded);

        bundle.Should().BeNull();
    }

    [Fact]
    public void CreateBundle_GivesTheProvenanceEntryAFullUrlMatchingItsId()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = GetProvenance(bundle);
        var entry = bundle.Entry.Single(e => e.Resource == provenance);
        entry.FullUrl.Should().Be($"Provenance/{provenance.Id}");
    }

    [Fact]
    public void CreateBundle_SetsAPutRequestToProvenanceSlashIdForTheProvenanceEntry()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = GetProvenance(bundle);
        var entry = bundle.Entry.Single(e => e.Resource == provenance);
        entry.Request.Method.Should().Be(Bundle.HTTPVerb.PUT);
        entry.Request.Url.Should().Be($"Provenance/{provenance.Id}");
    }

    [Fact]
    public void CreateBundle_WithIdentifiedResource_DerivesProvenanceIdFromSha256OfSystemPipeValue()
    {
        var patient = new Patient
        {
            Id = "hashed-123",
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };
        var expectedId = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes("http://example.org/mrn|12345"))
        );

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        GetProvenance(bundle).Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithSameIdentifier_ProducesTheSameProvenanceIdEachTime()
    {
        var patient = new Patient
        {
            Id = "hashed-123",
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };

        var firstId = GetProvenance(ProvenanceFactory.CreateBundle(null, patient, Recorded)).Id;
        var secondId = GetProvenance(
            ProvenanceFactory.CreateBundle(null, patient, Recorded.AddDays(1))
        ).Id;

        firstId.Should().Be(secondId);
    }

    [Fact]
    public void CreateBundle_WithOriginalResourceHavingAnId_DerivesProvenanceIdFromItOverEitherIdentifier()
    {
        // Resource.id takes priority over identifier; the pseudonymized id is deliberately not
        // considered at all (only the pre-pseudonymization one), since unlike it, the pseudonymized
        // id is not guaranteed to be a deterministic function of the original (e.g. it could come
        // from a non-deterministic "substitute" rule)
        var original = new Patient
        {
            Id = "123",
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Identifier = [new Identifier("http://example.org/mrn", "ciphertext-abc")],
        };
        var expectedId = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes("Patient/123"))
        );

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        GetProvenance(bundle).Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithOriginalResourceHavingNoId_FallsBackToPreferringOriginalsIdentifierOverPseudonymizedsIdentifier()
    {
        // the identifier value itself may have been pseudonymized (e.g. encrypted, which need
        // not be deterministic), so the id must be derived from the pre-pseudonymization identifier
        var original = new Patient
        {
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Identifier = [new Identifier("http://example.org/mrn", "ciphertext-abc")],
        };
        var expectedId = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes("http://example.org/mrn|12345"))
        );

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        GetProvenance(bundle).Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithoutAnyIdentifier_FallsBackToARandomId()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        Guid.TryParse(GetProvenance(bundle).Id, out _).Should().BeTrue();
    }

    [Fact]
    public void CreateBundle_WithBundleInput_DerivesProvenanceIdFromAllContainedResourcesIdentifiers()
    {
        var bundleInput = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent
                {
                    Resource = new Patient
                    {
                        Id = "patient-1",
                        Identifier = [new Identifier("http://example.org/mrn", "111")],
                    },
                },
                new Bundle.EntryComponent
                {
                    Resource = new Observation
                    {
                        Id = "obs-1",
                        Identifier = [new Identifier("http://example.org/obs-id", "222")],
                    },
                },
            ],
        };
        var expectedId = Convert.ToHexStringLower(
            SHA256.HashData(
                Encoding.UTF8.GetBytes("http://example.org/mrn|111,http://example.org/obs-id|222")
            )
        );

        var bundle = ProvenanceFactory.CreateBundle(null, bundleInput, Recorded);

        GetProvenance(bundle).Id.Should().Be(expectedId);
    }
}
