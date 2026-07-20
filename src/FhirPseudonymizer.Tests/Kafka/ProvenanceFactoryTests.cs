using System.Security.Cryptography;
using System.Text;
using FhirPseudonymizer.Kafka;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace FhirPseudonymizer.Tests.Kafka;

public class ProvenanceFactoryTests
{
    private static readonly DateTimeOffset Recorded = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateBundle_WithSingleResource_ReturnsBundleWithOneProvenanceTargetingIt()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        bundle.Should().NotBeNull();
        bundle.Type.Should().Be(Bundle.BundleType.Transaction);
        bundle.Entry.Should().ContainSingle();

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Target.Should().ContainSingle();
        provenance.Target.Single().Reference.Should().Be("Patient/hashed-123");
        provenance.Recorded.Should().Be(Recorded);
    }

    [Fact]
    public void CreateBundle_WithSingleResource_SetsAgent()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance
            .Agent.Should()
            .ContainSingle(a => a.Who.Display == ProvenanceFactory.AgentDisplay);
    }

    [Fact]
    public void CreateBundle_WithOriginalResourceHavingAnId_SetsSourceEntityReferencingItByTypeSlashId()
    {
        var original = new Patient { Id = "123" };
        var pseudonymized = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
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

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        var entity = provenance.Entity.Single();
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

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Entity.Should().BeEmpty();
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

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        var sources = provenance.Entity.Select(e => e.What.Reference).ToList();
        sources.Should().BeEquivalentTo(["Patient/patient-1", "Observation/obs-1"]);
    }

    [Fact]
    public void CreateBundle_WithSecurityLabelsNewlyAddedByPseudonymization_SetsActivityFromThem()
    {
        var original = new Patient { Id = "123" };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Meta = new Meta { Security = [SecurityLabels.CRYTOHASH, SecurityLabels.PSEUDED] },
        };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance
            .Activity.Coding.Should()
            .BeEquivalentTo([SecurityLabels.CRYTOHASH, SecurityLabels.PSEUDED]);
    }

    [Fact]
    public void CreateBundle_WithSecurityLabelAlreadyPresentOnInput_ExcludesItFromActivity()
    {
        // simulates a resource that was already pseudonymized upstream before reaching this
        // service - its pre-existing PSEUDED tag must not be attributed to this run
        var original = new Patient
        {
            Id = "123",
            Meta = new Meta { Security = [SecurityLabels.PSEUDED] },
        };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Meta = new Meta { Security = [SecurityLabels.PSEUDED, SecurityLabels.CRYTOHASH] },
        };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Activity.Coding.Should().BeEquivalentTo([SecurityLabels.CRYTOHASH]);
    }

    [Fact]
    public void CreateBundle_WithOnlyPreExistingSecurityLabels_LeavesActivityUnset()
    {
        // nothing was actually applied by this pseudonymization run
        var original = new Patient
        {
            Id = "123",
            Meta = new Meta { Security = [SecurityLabels.PSEUDED] },
        };
        var pseudonymized = new Patient
        {
            Id = "123",
            Meta = new Meta { Security = [SecurityLabels.PSEUDED] },
        };

        var bundle = ProvenanceFactory.CreateBundle(original, pseudonymized, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Activity.Should().BeNull();
    }

    [Fact]
    public void CreateBundle_WithResourceCarryingUnrelatedSecurityLabel_ExcludesItFromActivity()
    {
        var unrelatedLabel = new Coding
        {
            System = "http://terminology.hl7.org/CodeSystem/v3-ActReason",
            Code = "HTEST",
        };
        var pseudonymized = new Patient
        {
            Id = "hashed-123",
            Meta = new Meta { Security = [SecurityLabels.PSEUDED, unrelatedLabel] },
        };

        var bundle = ProvenanceFactory.CreateBundle(null, pseudonymized, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Activity.Coding.Should().BeEquivalentTo([SecurityLabels.PSEUDED]);
    }

    [Fact]
    public void CreateBundle_WithResourceWithoutSecurityLabels_LeavesActivityUnset()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Activity.Should().BeNull();
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

        bundle.Entry.Should().ContainSingle();
        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        var targets = provenance.Target.Select(t => t.Reference).ToList();
        targets.Should().BeEquivalentTo(["Patient/patient-1", "Observation/obs-1"]);
    }

    [Fact]
    public void CreateBundle_WithBundleInput_PairsEntriesByPositionForActivityDiffing()
    {
        var originalBundle = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent
                {
                    Resource = new Patient
                    {
                        Id = "patient-1",
                        Meta = new Meta { Security = [SecurityLabels.PSEUDED] },
                    },
                },
            ],
        };
        var pseudonymizedBundle = new Bundle
        {
            Entry =
            [
                new Bundle.EntryComponent
                {
                    Resource = new Patient
                    {
                        Id = "hashed-1",
                        Meta = new Meta
                        {
                            Security = [SecurityLabels.PSEUDED, SecurityLabels.CRYTOHASH],
                        },
                    },
                },
            ],
        };

        var bundle = ProvenanceFactory.CreateBundle(originalBundle, pseudonymizedBundle, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance.Activity.Coding.Should().BeEquivalentTo([SecurityLabels.CRYTOHASH]);
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

        bundle.Entry.Should().ContainSingle();
        ((Provenance)bundle.Entry.Single().Resource)
            .Target.Single()
            .Reference.Should()
            .Be("Patient/patient-1");
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
    public void CreateBundle_GivesEachEntryAFullUrlMatchingItsProvenanceId()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var entry = bundle.Entry.Single();
        entry.FullUrl.Should().Be($"Provenance/{entry.Resource.Id}");
    }

    [Fact]
    public void CreateBundle_SetsAPutRequestToProvenanceSlashIdForEachEntry()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        var entry = bundle.Entry.Single();
        entry.Request.Method.Should().Be(Bundle.HTTPVerb.PUT);
        entry.Request.Url.Should().Be($"Provenance/{entry.Resource.Id}");
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

        bundle.Entry.Single().Resource.Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithSameIdentifier_ProducesTheSameProvenanceIdEachTime()
    {
        var patient = new Patient
        {
            Id = "hashed-123",
            Identifier = [new Identifier("http://example.org/mrn", "12345")],
        };

        var firstId = ProvenanceFactory
            .CreateBundle(null, patient, Recorded)
            .Entry.Single()
            .Resource.Id;
        var secondId = ProvenanceFactory
            .CreateBundle(null, patient, Recorded.AddDays(1))
            .Entry.Single()
            .Resource.Id;

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

        bundle.Entry.Single().Resource.Id.Should().Be(expectedId);
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

        bundle.Entry.Single().Resource.Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithoutAnyIdentifier_FallsBackToARandomId()
    {
        var patient = new Patient { Id = "hashed-123" };

        var bundle = ProvenanceFactory.CreateBundle(null, patient, Recorded);

        Guid.TryParse(bundle.Entry.Single().Resource.Id, out _).Should().BeTrue();
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

        bundle.Entry.Single().Resource.Id.Should().Be(expectedId);
    }

    [Fact]
    public void CreateBundle_WithBundleInput_UnionsAppliedOperationsAcrossAllContainedResources()
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
                new Bundle.EntryComponent
                {
                    Resource = new Patient
                    {
                        Id = "hashed-patient-1",
                        Meta = new Meta { Security = [SecurityLabels.CRYTOHASH] },
                    },
                },
                new Bundle.EntryComponent
                {
                    Resource = new Observation
                    {
                        Id = "hashed-obs-1",
                        Meta = new Meta { Security = [SecurityLabels.REDACT] },
                    },
                },
            ],
        };

        var bundle = ProvenanceFactory.CreateBundle(originalBundle, pseudonymizedBundle, Recorded);

        var provenance = bundle.Entry.Single().Resource.Should().BeOfType<Provenance>().Subject;
        provenance
            .Activity.Coding.Should()
            .BeEquivalentTo([SecurityLabels.CRYTOHASH, SecurityLabels.REDACT]);
    }
}
