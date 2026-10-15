# Provenance

If `Kafka__Enabled=true` and `Kafka__ProvenanceTopic` is set, every successfully pseudonymized
resource - whether pseudonymized via the [Kafka consumer](kafka.md) or via the REST
`$de-identify` endpoint - also produces a [Provenance](https://hl7.org/fhir/R4/provenance.html)
audit trail: a single Provenance resource wrapped in a `transaction` Bundle and published to that
topic.

## Contents

Its `target` references what pseudonymization produced - every pseudonymized resource (for a
Bundle input, one target per contained resource; otherwise just the one resource), each by its
post-pseudonymization `<type>/<id>` (since e.g. cryptoHash-ing `Resource.id` changes it) - while
its `entity` (role `source`) references what each was derived from: the same resources'
pre-pseudonymization identity, by `<type>/<Resource.id>` if they had one, otherwise by their first
`identifier` (a `Reference.identifier`, for resources with no resolvable id). Its `activity` is
always `http://terminology.hl7.org/CodeSystem/iso-21089-lifecycle|deidentify` ("De-Identify
(Anononymize) Record Lifecycle Event").

Its `agent` identifies the FHIR Pseudonymizer itself: `type` is `performer`
(`http://terminology.hl7.org/CodeSystem/provenance-participant-type`) and `role` is `AUT`/"author
(originator)" (`http://terminology.hl7.org/CodeSystem/v3-ParticipationType`), the closest standard
fit for an automated system that transformed the content. `who` references a
[Device](https://hl7.org/fhir/R4/device.html) resource that
is included in the same Bundle, naming and versioning the FHIR Pseudonymizer (its version
truncated to `major.minor.build`, matching semver); that Device's id is the SHA-256 hash of its
name and version, so it - and the Provenance's reference to it - stays the same across restarts
and only changes on a version bump. It also carries a human-readable business identifier, e.g.
`fhir-pseudonymizer-v2.29.0`, under the
`https://miracum.github.io/fhir-pseudonymizer/identifiers/device-id` system.

## Idempotency

The Provenance is a `PUT` to `Provenance/<id>`, where `<id>` is the SHA-256 hash (lowercase hex)
of its targets' identity, joined together: for each target this is its pre-pseudonymization
`<type>/<Resource.id>` if it had one, otherwise its `identifier.system|identifier.value` (using
its first identifier). This makes re-pseudonymizing the same input idempotent: the resulting
Provenance always gets the same id, so applying the bundle to a FHIR server upserts the existing
record instead of accumulating duplicates. Targets with neither an id nor an identifier are
skipped when deriving the id; if none of them have either, the Provenance falls back to a random
id (so it will no longer be idempotent for that input).

Since a Bundle only ever contains this one Provenance (plus the Device), the wrapping Bundle is
given the Provenance's id, which also becomes the Kafka message key it's produced with - not the
source message's key - so that repeated provenance for the same input lands on the same partition
(e.g. for log compaction).

## Delivery

The provenance bundle carries the same headers as the source message (e.g. tracing context) when
read from Kafka; REST calls produce it without any.

For messages read from Kafka, the provenance bundle is delivered with the same guarantees as the
pseudonymized message: a message's offset is only committed once both have been acknowledged by
the broker, and if the provenance bundle can't be delivered, the original message is sent to the
dead letter topic (with `x-error-type: FhirPseudonymizer.Kafka.ProvenancePublishingException`) -
even though its pseudonymized version was already delivered.

For REST calls, producing the provenance bundle is best-effort: a failure is logged but does not
affect the pseudonymized resource being returned. Producing provenance bundles from the REST API
requires `Kafka__Enabled=true` and `Kafka__Client__BootstrapServers` to be set even if
`Kafka__Topics` (Kafka consuming) is left unset.
