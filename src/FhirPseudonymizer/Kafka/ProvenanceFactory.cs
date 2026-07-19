using System.Security.Cryptography;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Builds FHIR Provenance resources documenting that a resource was pseudonymized, referencing
///     it by its post-pseudonymization identity (its type stays the same, but its id may have
///     changed, e.g. when cryptoHash-ing <c>Resource.id</c>), and stating which de-identification
///     operations (redact, cryptoHash, pseudonymize, ...) were actually applied to it via
///     <see cref="Provenance.Activity" />. Each Provenance is given a deterministic id (a SHA-256
///     hash of the source resource's <c>identifier.system|identifier.value</c>) so that
///     re-pseudonymizing the same resource later PUTs to the same Provenance instead of creating a
///     duplicate.
/// </summary>
public static class ProvenanceFactory
{
    public const string AgentDisplay = "FHIR Pseudonymizer";

    /// <summary>
    ///     The <see cref="Meta.Security" /> codes the anonymizer's <c>AddSecurityTag</c> step (see
    ///     <c>ElementNodeOperationExtensions.AddSecurityTag</c>) tags a resource with for each
    ///     de-identification operation actually applied to it (redact, cryptoHash, pseudonymize,
    ///     ...). Only these are ever considered for a Provenance's <see cref="Provenance.Activity" />.
    /// </summary>
    private static readonly HashSet<string> AnonymizationSecurityLabelCodes = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        SecurityLabels.REDACT.Code,
        SecurityLabels.ABSTRED.Code,
        SecurityLabels.CRYTOHASH.Code,
        SecurityLabels.ENCRYPT.Code,
        SecurityLabels.PERTURBED.Code,
        SecurityLabels.SUBSTITUTED.Code,
        SecurityLabels.GENERALIZED.Code,
        SecurityLabels.PSEUDED.Code,
    };

    /// <summary>
    ///     Creates a "transaction" Bundle containing one Provenance resource per pseudonymized
    ///     resource, targeting it by its (possibly changed) post-pseudonymization
    ///     <c>&lt;type&gt;/&lt;id&gt;</c>. If <paramref name="pseudonymized" /> is a Bundle, one
    ///     Provenance is created per contained resource (paired by position with
    ///     <paramref name="original" />'s entries); otherwise a single Provenance targets
    ///     <paramref name="pseudonymized" /> itself. Returns null if there is nothing to reference
    ///     (e.g. an empty bundle, or a resource without an id). Each entry is a PUT to
    ///     <c>Provenance/&lt;id&gt;</c>, see <see cref="ComputeProvenanceId" /> for how that id is
    ///     derived.
    /// </summary>
    public static Bundle CreateBundle(
        Resource original,
        Resource pseudonymized,
        DateTimeOffset recorded
    )
    {
        var provenances = GetTargets(original, pseudonymized)
            .Select(target => CreateProvenance(target.Original, target.Pseudonymized, recorded))
            .ToList();

        if (provenances.Count == 0)
        {
            return null;
        }

        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Transaction,
            Timestamp = recorded,
        };

        foreach (var provenance in provenances)
        {
            bundle.Entry.Add(
                new Bundle.EntryComponent
                {
                    FullUrl = $"Provenance/{provenance.Id}",
                    Resource = provenance,
                    Request = new Bundle.RequestComponent
                    {
                        Method = Bundle.HTTPVerb.PUT,
                        Url = $"Provenance/{provenance.Id}",
                    },
                }
            );
        }

        return bundle;
    }

    /// <summary>
    ///     Pairs up each pseudonymized resource with its pre-pseudonymization counterpart, so
    ///     <see cref="CreateProvenance" /> can tell which security labels were newly added versus
    ///     already present on the input. Bundle entries are paired by position, since the
    ///     anonymizer neither reorders nor adds/removes entries.
    /// </summary>
    private static IEnumerable<(Resource Original, Resource Pseudonymized)> GetTargets(
        Resource original,
        Resource pseudonymized
    )
    {
        if (pseudonymized is Bundle pseudonymizedBundle)
        {
            var originalEntries = (original as Bundle)?.Entry;
            return pseudonymizedBundle
                .Entry.Select(
                    (entry, index) =>
                        (
                            Original: originalEntries != null && index < originalEntries.Count
                                ? originalEntries[index].Resource
                                : null,
                            Pseudonymized: entry.Resource
                        )
                )
                .Where(pair =>
                    pair.Pseudonymized is not (null or Bundle or Provenance)
                    && !string.IsNullOrEmpty(pair.Pseudonymized.Id)
                );
        }

        return !string.IsNullOrEmpty(pseudonymized?.Id) ? [(original, pseudonymized)] : [];
    }

    private static Provenance CreateProvenance(
        Resource original,
        Resource target,
        DateTimeOffset recorded
    )
    {
        var provenance = new Provenance
        {
            Id = ComputeProvenanceId(original, target),
            Target = [new ResourceReference($"{target.TypeName}/{target.Id}")],
            Recorded = recorded,
            Agent =
            [
                new Provenance.AgentComponent
                {
                    Who = new ResourceReference { Display = AgentDisplay },
                },
            ],
        };

        var appliedOperations = GetNewlyAppliedAnonymizationOperations(original, target);
        if (appliedOperations.Count > 0)
        {
            provenance.Activity = new CodeableConcept { Coding = appliedOperations };
        }

        return provenance;
    }

    /// <summary>
    ///     Derives a deterministic id for the Provenance about <paramref name="target" />, so that
    ///     re-pseudonymizing the same source resource later PUTs to the same Provenance instance
    ///     instead of accumulating duplicates: the SHA-256 hash (lowercase hex, which fits FHIR's
    ///     64-character id limit exactly) of that resource's first identifier, formatted as
    ///     <c>identifier.system|identifier.value</c> (matching the FHIR search token syntax).
    ///     Prefers <paramref name="original" />'s identifier over <paramref name="target" />'s, since
    ///     the identifier value itself may have been pseudonymized (e.g. encrypted) and an
    ///     encryption method need not be deterministic. Falls back to a random id if neither has an
    ///     identifier to key off of.
    /// </summary>
    private static string ComputeProvenanceId(Resource original, Resource target)
    {
        var identifier = GetFirstIdentifier(original) ?? GetFirstIdentifier(target);
        if (identifier is null)
        {
            return Guid.NewGuid().ToString();
        }

        var token = $"{identifier.System}|{identifier.Value}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexStringLower(hash);
    }

    private static Identifier GetFirstIdentifier(Resource resource)
    {
        return resource is IIdentifiable<List<Identifier>> identifiable
            ? identifiable.Identifier?.FirstOrDefault()
            : null;
    }

    /// <summary>
    ///     Reads back which de-identification operations were actually applied to
    ///     <paramref name="target" /> by comparing its security labels against
    ///     <paramref name="original" />'s: a label only counts if it is one of the known
    ///     anonymization codes (<see cref="AnonymizationSecurityLabelCodes" />) AND was not already
    ///     present before pseudonymization. This keeps a Provenance's activity accurate even if the
    ///     input resource already carried its own (e.g. previously pseudonymized upstream) security
    ///     labels - only labels the pseudonymizer itself added are reflected. Returns empty if
    ///     nothing new was added, e.g. when <c>Anonymization__ShouldAddSecurityTag</c> is disabled.
    /// </summary>
    private static List<Coding> GetNewlyAppliedAnonymizationOperations(
        Resource original,
        Resource target
    )
    {
        var preExistingCodes = new HashSet<string>(
            original?.Meta?.Security?.Select(coding => coding.Code) ?? [],
            StringComparer.OrdinalIgnoreCase
        );

        return target
                .Meta?.Security.Where(coding =>
                    AnonymizationSecurityLabelCodes.Contains(coding.Code)
                    && !preExistingCodes.Contains(coding.Code)
                )
                .ToList()
            ?? [];
    }
}
