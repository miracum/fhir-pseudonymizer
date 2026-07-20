using System.Security.Cryptography;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Builds a FHIR Provenance resource documenting that a resource (or, for a Bundle, all of its
///     contained resources) was pseudonymized. <see cref="Provenance.Target" /> references what the
///     activity produced - each resource by its post-pseudonymization identity (its type stays the
///     same, but its id may have changed, e.g. when cryptoHash-ing <c>Resource.id</c>) - while
///     <see cref="Provenance.Entity" /> (role <c>source</c>, see <see cref="GetSourceEntity" />)
///     references what it was derived from - each resource's pre-pseudonymization identity. States
///     which de-identification operations (redact, cryptoHash, pseudonymize, ...) were actually
///     applied via <see cref="Provenance.Activity" />. The Provenance is given a deterministic id (a
///     SHA-256 hash of its targets' combined identity, preferring each target's <c>Resource.id</c>
///     and falling back to its <c>identifier.system|identifier.value</c>, see
///     <see cref="GetIdentityToken" />) so that re-pseudonymizing the same input later PUTs to the
///     same Provenance instead of creating a duplicate.
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
    ///     Creates a "transaction" Bundle containing a single Provenance resource documenting the
    ///     pseudonymization of <paramref name="pseudonymized" />. If it is a Bundle, the Provenance
    ///     targets every contained resource (each by its possibly-changed post-pseudonymization
    ///     <c>&lt;type&gt;/&lt;id&gt;</c>); otherwise it targets <paramref name="pseudonymized" />
    ///     itself. Returns null if there is nothing to reference (e.g. an empty bundle, or a
    ///     resource without an id). The entry is a PUT to <c>Provenance/&lt;id&gt;</c>, see
    ///     <see cref="ComputeProvenanceId" /> for how that id is derived.
    /// </summary>
    public static Bundle CreateBundle(
        Resource original,
        Resource pseudonymized,
        DateTimeOffset recorded
    )
    {
        var targets = GetTargets(original, pseudonymized).ToList();
        if (targets.Count == 0)
        {
            return null;
        }

        var provenance = CreateProvenance(targets, recorded);

        var bundle = new Bundle
        {
            Id = Guid.NewGuid().ToString(),
            Type = Bundle.BundleType.Transaction,
            Timestamp = recorded,
        };

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
        IReadOnlyList<(Resource Original, Resource Pseudonymized)> targets,
        DateTimeOffset recorded
    )
    {
        var provenance = new Provenance
        {
            Id = ComputeProvenanceId(targets),
            Target = targets
                .Select(target => new ResourceReference(
                    $"{target.Pseudonymized.TypeName}/{target.Pseudonymized.Id}"
                ))
                .ToList(),
            Entity = targets
                .Select(target => GetSourceEntity(target.Original))
                .Where(entity => entity is not null)
                .ToList(),
            Recorded = recorded,
            Agent =
            [
                new Provenance.AgentComponent
                {
                    Who = new ResourceReference { Display = AgentDisplay },
                },
            ],
        };

        var appliedOperations = targets
            .SelectMany(target =>
                GetNewlyAppliedAnonymizationOperations(target.Original, target.Pseudonymized)
            )
            .GroupBy(coding => coding.Code, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();

        if (appliedOperations.Count > 0)
        {
            provenance.Activity = new CodeableConcept { Coding = appliedOperations };
        }

        return provenance;
    }

    /// <summary>
    ///     Builds the <c>entity[role=source]</c> pointing back at <paramref name="original" />, the
    ///     pre-pseudonymization resource a target was derived from (as opposed to
    ///     <see cref="Provenance.Target" />, which points at the resource the activity produced -
    ///     the pseudonymized one). References it by <c>&lt;type&gt;/&lt;Resource.id&gt;</c> if it had
    ///     an id, otherwise by its first identifier (via <see cref="ResourceReference.Identifier" />,
    ///     meant for exactly this case: no resolvable id). Returns null if
    ///     <paramref name="original" /> is unavailable or has neither an id nor an identifier to
    ///     reference it by.
    /// </summary>
    private static Provenance.EntityComponent GetSourceEntity(Resource original)
    {
        ResourceReference what;
        if (!string.IsNullOrEmpty(original?.Id))
        {
            what = new ResourceReference($"{original.TypeName}/{original.Id}");
        }
        else
        {
            var identifier = GetFirstIdentifier(original);
            what = identifier is null ? null : new ResourceReference { Identifier = identifier };
        }

        return what is null
            ? null
            : new Provenance.EntityComponent
            {
                Role = Provenance.ProvenanceEntityRole.Source,
                What = what,
            };
    }

    /// <summary>
    ///     Derives a deterministic id for a Provenance covering <paramref name="targets" />, so that
    ///     re-pseudonymizing the same input later PUTs to the same Provenance instance instead of
    ///     accumulating duplicates: the SHA-256 hash (lowercase hex, which fits FHIR's 64-character
    ///     id limit exactly) of each target's identity token (see <see cref="GetIdentityToken" />),
    ///     joined in order. Targets without any identity token do not contribute to the hash. Falls
    ///     back to a random id if none of the targets have one to key off of.
    /// </summary>
    private static string ComputeProvenanceId(
        IReadOnlyList<(Resource Original, Resource Pseudonymized)> targets
    )
    {
        var tokens = targets.Select(GetIdentityToken).Where(token => token is not null).ToList();

        if (tokens.Count == 0)
        {
            return Guid.NewGuid().ToString();
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(string.Join(',', tokens)));
        return Convert.ToHexStringLower(hash);
    }

    /// <summary>
    ///     Builds the token identifying <paramref name="target" /> for <see cref="ComputeProvenanceId" />:
    ///     preferably <c>&lt;type&gt;/&lt;id&gt;</c> using the resource's pre-pseudonymization
    ///     <c>Resource.id</c>. Deliberately only considers the pre-pseudonymization id, not the
    ///     pseudonymized one: unlike the original id, the pseudonymized id is not guaranteed to be a
    ///     deterministic function of it (e.g. it could come from a non-deterministic "substitute"
    ///     rule), so trusting it here could break the whole point of a deterministic Provenance id.
    ///     If there is no pre-pseudonymization id, falls back to
    ///     <c>identifier.system|identifier.value</c> (matching the FHIR search token syntax) of the
    ///     first identifier found - preferring the pre-pseudonymization one since an identifier value
    ///     may itself have been pseudonymized (e.g. encrypted, which need not be deterministic).
    ///     Returns null if the target has neither an id nor an identifier.
    /// </summary>
    private static string GetIdentityToken((Resource Original, Resource Pseudonymized) target)
    {
        if (!string.IsNullOrEmpty(target.Original?.Id))
        {
            return $"{target.Pseudonymized.TypeName}/{target.Original.Id}";
        }

        var identifier =
            GetFirstIdentifier(target.Original) ?? GetFirstIdentifier(target.Pseudonymized);
        return identifier is null ? null : $"{identifier.System}|{identifier.Value}";
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
