using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Hl7.Fhir.Model;

namespace FhirPseudonymizer.Kafka;

/// <summary>
///     Builds a FHIR Provenance resource documenting that a resource (or, for a Bundle, all of its
///     contained resources) was pseudonymized. <see cref="Provenance.Target" /> references what the
///     activity produced - each resource by its post-pseudonymization identity (its type stays the
///     same, but its id may have changed, e.g. when cryptoHash-ing <c>Resource.id</c>) - while
///     <see cref="Provenance.Entity" /> (role <c>source</c>, see <see cref="GetSourceEntity" />)
///     references what it was derived from - each resource's pre-pseudonymization identity.
///     <see cref="Provenance.Activity" /> is always the fixed ISO/HL7 21089 "de-identify" lifecycle
///     event code (see <see cref="ActivityCoding" />). The Provenance is given a deterministic id (a
///     SHA-256 hash of its targets' combined identity, preferring each target's <c>Resource.id</c>
///     and falling back to its <c>identifier.system|identifier.value</c>, see
///     <see cref="GetIdentityToken" />) so that re-pseudonymizing the same input later PUTs to the
///     same Provenance instead of creating a duplicate. Since a Bundle only ever contains this one
///     Provenance, the wrapping Bundle is given that same id.
/// </summary>
public static class ProvenanceFactory
{
    public const string AgentDisplay = "FHIR Pseudonymizer";

    /// <summary>
    ///     Identifies the pseudonymizer's participation as having carried out the activity, per
    ///     <see cref="Provenance.AgentComponent.Type" />'s narrow, unambiguous binding.
    /// </summary>
    private static readonly Coding AgentType = new(
        "http://terminology.hl7.org/CodeSystem/provenance-participant-type",
        "performer",
        "Performer"
    );

    /// <summary>
    ///     The pseudonymizer's functional role with respect to the activity, per
    ///     <see cref="Provenance.AgentComponent.Role" />. Unlike <see cref="AgentType" />, that
    ///     binding is a broad, extensible grab-bag with no single obvious code for "automated
    ///     de-identification software"; "author (originator)" is the closest, commonly used fit for
    ///     an automated system that produced/transformed the content.
    /// </summary>
    private static readonly Coding AgentRole = new(
        "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
        "AUT",
        "author (originator)"
    );

    /// <summary>
    ///     The fixed <see cref="Provenance.Activity" /> every Provenance produced by this factory is
    ///     given, identifying the activity as an ISO/HL7 21089 "de-identify" record lifecycle event.
    /// </summary>
    private static readonly Coding ActivityCoding = new(
        "http://terminology.hl7.org/CodeSystem/iso-21089-lifecycle",
        "deidentify",
        "De-Identify (Anononymize) Record Lifecycle Event"
    );

    /// <summary>
    ///     The Device resource representing the FHIR Pseudonymizer itself, referenced by every
    ///     Provenance's <see cref="Provenance.AgentComponent.Who" /> (instead of just a display
    ///     string) and included in the same Bundle. Built once - its id is derived only from the
    ///     app's name and assembly version (see <see cref="BuildDeviceId" />), so it stays the same,
    ///     and PUTs idempotently, across every Provenance produced by this running instance, only
    ///     changing on a version bump.
    /// </summary>
    private static readonly Device PseudonymizerDevice = CreatePseudonymizerDevice();

    /// <summary>
    ///     Creates a "transaction" Bundle containing a single Provenance resource documenting the
    ///     pseudonymization of <paramref name="pseudonymized" />, alongside the
    ///     <see cref="PseudonymizerDevice" /> it references as its agent. If it is a Bundle, the
    ///     Provenance targets every contained resource (each by its possibly-changed
    ///     post-pseudonymization <c>&lt;type&gt;/&lt;id&gt;</c>); otherwise it targets
    ///     <paramref name="pseudonymized" /> itself. Returns null if there is nothing to reference
    ///     (e.g. an empty bundle, or a resource without an id). The Bundle's own id is set to the
    ///     (single) Provenance's id, and both entries are PUTs to their own <c>&lt;type&gt;/&lt;id&gt;</c>,
    ///     see <see cref="ComputeProvenanceId" /> and <see cref="BuildDeviceId" /> for how those ids
    ///     are derived.
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
            Id = provenance.Id,
            Type = Bundle.BundleType.Transaction,
            Timestamp = recorded,
        };

        bundle.Entry.Add(AsPutEntry(PseudonymizerDevice));
        bundle.Entry.Add(AsPutEntry(provenance));

        return bundle;
    }

    /// <summary>
    ///     Wraps <paramref name="resource" /> (which must already have an id) in a Bundle entry that
    ///     PUTs it to its own <c>&lt;type&gt;/&lt;id&gt;</c>, the pattern every resource in a
    ///     provenance Bundle is published with so applying the Bundle to a FHIR server upserts by id
    ///     instead of creating duplicates.
    /// </summary>
    private static Bundle.EntryComponent AsPutEntry(Resource resource)
    {
        var url = $"{resource.TypeName}/{resource.Id}";
        return new Bundle.EntryComponent
        {
            FullUrl = url,
            Resource = resource,
            Request = new Bundle.RequestComponent { Method = Bundle.HTTPVerb.PUT, Url = url },
        };
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
        return new Provenance
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
            Activity = new CodeableConcept { Coding = [ActivityCoding] },
            Agent =
            [
                new Provenance.AgentComponent
                {
                    Type = new CodeableConcept { Coding = [AgentType] },
                    Role = [new CodeableConcept { Coding = [AgentRole] }],
                    Who = new ResourceReference($"Device/{PseudonymizerDevice.Id}")
                    {
                        Display = AgentDisplay,
                    },
                },
            ],
        };
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
    ///     Builds the <see cref="PseudonymizerDevice" /> resource, naming and versioning it after
    ///     this running assembly.
    /// </summary>
    private static Device CreatePseudonymizerDevice()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

        return new Device
        {
            Id = BuildDeviceId(AgentDisplay, version),
            DeviceName =
            [
                new Device.DeviceNameComponent
                {
                    Name = AgentDisplay,
                    Type = DeviceNameType.UserFriendlyName,
                },
            ],
            Version = [new Device.VersionComponent { Value = version }],
        };
    }

    /// <summary>
    ///     Derives <see cref="PseudonymizerDevice" />'s id from its name and version alone: the
    ///     SHA-256 hash (lowercase hex, which fits FHIR's 64-character id limit exactly) of
    ///     <c>name|version</c>. This keeps the id - and so the Device PUT target - stable across
    ///     restarts and only changing when the app's version does.
    /// </summary>
    private static string BuildDeviceId(string name, string version)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{name}|{version}"));
        return Convert.ToHexStringLower(hash);
    }
}
