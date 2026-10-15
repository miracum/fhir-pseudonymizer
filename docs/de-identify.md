# De-identification

## `$de-identify`

The server provides a `/fhir/$de-identify` operation to de-identify received FHIR resources
according to the rules in its `anonymization.yaml`. There is no default config: the service fails to
start unless you either mount a file and set `AnonymizationEngineConfigPath`, or pass the rules
via `AnonymizationEngineConfigInline`; see [Configuration](configuration.md). See
[Tools for Health Data Anonymization](https://github.com/microsoft/Tools-for-Health-Data-Anonymization)
for more details on the rule configuration. An
[example config](https://github.com/miracum/fhir-pseudonymizer/blob/master/src/FhirPseudonymizer/anonymization.yaml)
that redacts names and encrypts patient and visit identifiers is available as a starting point.

The repository contains a sample configuration to help meet the requirements of the HIPAA Safe
Harbor Method (2)(i):
[hipaa-anonymization.yaml](https://github.com/miracum/fhir-pseudonymizer/blob/master/src/FhirPseudonymizer/hipaa-anonymization.yaml).
Neither sample is included in the container image; download the file, mount it into the container
and point `AnonymizationEngineConfigPath` at it.

All methods defined in Tools for Health Data Anonymization are supported. For example, to clamp
a patient's birthdate to 01/01/1930 if they were born before January 1st 1931:

```yaml
fhirPathRules:
  - path: Patient.birthDate
    method: generalize
    cases:
      "$this < @1931-01-01": "@1930-01-01"
    otherValues: keep
```

## The `pseudonymize` method

A `pseudonymize` method was added to the list of anonymization methods. It uses either
[gPAS](pseudonymization/gpas.md), [Vfps](pseudonymization/vfps.md), [entici](pseudonymization/entici.md) or an
[MII-compliant service](pseudonymization/mii.md) to create pseudonyms and replaces the values in the
resource with them. Choose which one with the `PseudonymizationService` setting.

For example, the following rule replaces all identifiers of type
`http://terminology.hl7.org/CodeSystem/v2-0203|MR` with a pseudonym generated in the `PATIENT`
domain:

```yaml
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.system='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value
    method: pseudonymize
    domain: PATIENT
```

If the `domain` setting is omitted and an ID or reference is pseudonymized, the resource name is
used as the pseudonym domain. For example, pseudonymizing `"reference": "Patient/123"` tries to
create a pseudonym for `123` in the `Patient` domain.

When using Vfps, `domain` can instead also be set as `namespace`.

## The `remove` method

The existing `redact` only ever clears scalar values and, for a composite or backbone element,
recurses into it and clears each child field individually. `remove` deletes the matched node
outright, including anything nested within it. This makes it possible to drop entire resources
from a Bundle, or to target contained resources, which `redact` cannot do. For example, to remove
every `Patient` entry from a Bundle:

```yaml
fhirPathRules:
  - path: Bundle.entry.where(resource is Patient)
    method: remove
```

!!! warning

    `remove` is experimental and may change or be removed in a future version.

## Error responses

If de-identification fails, `$de-identify` responds with an `OperationOutcome` and a status code
that tells the caller whether retrying makes sense:

| Status                      | Meaning                                                                                                                                                                                                                                                              |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `422 Unprocessable Entity`  | The request is well-formed, but its content can't be processed: the error was caused by the request itself, e.g. by a value that the pseudonymization service rejects since it doesn't match the namespace's required pattern. Retrying the same request won't help. |
| `503 Service Unavailable`   | A service the request depends on, e.g. the pseudonymization service, is temporarily unavailable. The error wasn't caused by the request, so it may be retried later.                                                                                                 |
| `500 Internal Server Error` | Any other, unexpected, error.                                                                                                                                                                                                                                        |

Errors returned by the pseudonymization service are mapped onto these: one rejecting the request
(Vfps with gRPC status `InvalidArgument`, `FailedPrecondition`, `NotFound`, `AlreadyExists` or
`OutOfRange`, or gPAS, entici or MII with a 4xx status other than 401, 403, 408 and 429) becomes a
422, and an outage that persists even after the HTTP/gRPC client's own retries becomes a 503.

## `$re-identify`

The `/fhir/$re-identify` operation reverts the `pseudonymize` and `encrypt` methods applied to
any resource. Other methods, such as `cryptoHash`, `redact` or `dateShift`, can't be reverted.
Accessing this endpoint requires [authentication](#authentication).

It accepts the same [dynamic rule settings and config](dynamic-config.md#re-identification) as
`$de-identify`, which have to match the ones the resource was de-identified with. Unlike
`$de-identify`, it doesn't add any security labels to the resource, but keeps the ones it already
has.

!!! note "Deprecated: `$de-pseudonymize`"

    `/fhir/$de-pseudonymize` is the operation's former name. It still works as an alias, but is
    deprecated and will be removed in a future release.

!!! warning

    If decryption or de-pseudonymization of a value fails, the original value is returned. This
    behavior may change or be made configurable in the future.

### Authentication

`$re-identify` requires an OAuth 2.0 access token in JWT format, issued by the OpenID Connect
provider configured using `Auth__Jwt__Authority` and sent in the `Authorization: Bearer` header.
If `Auth__Jwt__Authority` isn't set, every request is rejected with `401 Unauthorized`.

A token must be signed by the issuer, not be expired and contain `Auth__Jwt__Audience` in its
`aud` claim. Optionally, `Auth__Jwt__RequiredScope` and `Auth__Jwt__RequiredRole` additionally
restrict access to tokens with that scope or role. A valid token without it is rejected with
`403 Forbidden`. See the
[configuration reference](configuration.md) for all settings.

For example, to accept tokens issued by Keycloak to clients with the `re-identifier` realm role:

```sh
Auth__Jwt__Authority=https://keycloak.example.com/realms/example
Auth__Jwt__Audience=fhir-pseudonymizer
Auth__Jwt__RequiredRole=re-identifier
```

Keycloak doesn't add a client's ID to the `aud` claim of tokens issued to other clients by default.
Add an _Audience_ mapper with _Included Client Audience_ set to `fhir-pseudonymizer` to the
calling client, or to a client scope assigned to it. A caller can then obtain a token using the
client credentials grant:

```sh
TOKEN=$(curl -s \
  -d grant_type=client_credentials \
  -d client_id=my-client \
  -d client_secret=... \
  https://keycloak.example.com/realms/example/protocol/openid-connect/token | jq -r .access_token)

curl -X POST http://localhost:8080/fhir/\$re-identify \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/fhir+json" \
  -d @pseudonymized-patient.json
```
