# Dynamic rules and config

Anonymization and pseudonymization rules in the `anonymization.yaml` can be overridden and/or
extended on a per-request basis, so a single deployment can serve many projects.

## Dynamic rule settings

Pseudonymization supports a `domain-prefix` rule setting which configures the target domain by
providing its value as part of the request body.

The following example uses a single service configuration to support multiple projects which
have the same basic domain names, prefixed by a project name.

### Example

gPAS domains for patient IDs:

| project |          domain |
| ------- | --------------: |
| miracum | miracum-patient |
| test    |    test-patient |

`anonymization.yml`:

```yml
---
fhirVersion: R4
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.system='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value
    method: pseudonymize
    domain: patient
```

Providing the prefix (i.e. `miracum-` or `test-`) via the request, pseudonymization can be done
with the same rules for different projects.

#### Request body

Rule settings are provided using a `Parameters` resource with a `settings` parameter whose parts
consist of the settings key and value. The `resource` parameter must contain the actual target
resource.

The following request body and the (fixed) configuration settings above result in the target
domain `miracum-patient`.

```json
{
  "resourceType": "Parameters",
  "parameter": [
    {
      "name": "settings",
      "part": [
        {
          "name": "domain-prefix",
          "valueString": "miracum-"
        }
      ]
    },
    {
      "name": "resource",
      "resource": {
        "resourceType": "Bundle",
        "type": "transaction",
        "entry": [
          {
            "fullUrl": "urn:uuid:3bc44de3-069d-442d-829b-f3ef68cae371",
            "resource": {
              "resourceType": "Patient",
              "identifier": [
                {
                  "type": {
                    "coding": [
                      {
                        "system": "http://terminology.hl7.org/CodeSystem/v2-0203",
                        "code": "MR"
                      }
                    ]
                  },
                  "system": "http://acme.org/mrns",
                  "value": "12345"
                }
              ],
              "name": [
                {
                  "family": "Jameson",
                  "given": ["J", "Jonah"]
                }
              ],
              "gender": "male"
            },
            "request": {
              "method": "POST",
              "url": "Patient/12345"
            }
          }
        ]
      }
    }
  ]
}
```

The domain name could also have been replaced completely by overriding the `domain` setting with
the desired value. This works for all rule settings regardless of the `method` value.

## Dynamic config

To set a per-request dynamic config, add a `config` parameter alongside `resource` in the
`Parameters` request body: an `Attachment` whose `data` is a base64-encoded YAML anonymization
config:

```json
{
  "resourceType": "Parameters",
  "parameter": [
    {
      "name": "config",
      "valueAttachment": {
        "contentType": "application/yaml",
        "data": "ZmhpclZlcnNpb246IFI0CmZoaXJQYXRoUnVsZXM6CiAgLSBwYXRoOiBQYXRpZW50Lm5hbWUKICAgIG1ldGhvZDogcmVkYWN0Cg=="
      }
    },
    {
      "name": "resource",
      "resource": {
        "resourceType": "Patient",
        "name": [{ "family": "Doe", "given": ["John"] }]
      }
    }
  ]
}
```

`config` and `settings` can be combined in the same request: the dynamic rule settings are
applied on top of whichever rule set - static or per-request - ends up being used.

## Re-identification

[`$re-identify`](de-identify.md#re-identify) accepts the same `Parameters` request body, with
`settings` and `config`. To revert a resource, send the same ones it was de-identified with:
pseudonyms are looked up in the domain the `settings` resolve to, and only the rules of the given
`config` are reverted. With different ones, pseudonyms are looked up in the wrong domain, and values
de-identified by rules the used config lacks are returned as-is.

## Date shift offset

`dateShiftFixedOffsetInDays` can be set in two places, and both can be used at the same time:

- statically, as a `dateShiftFixedOffsetInDays` key in the `anonymization.yaml`'s `parameters`
  section, applying to every request.
- dynamically, as a per-request `settings` value on the `dateshift` rule (see
  [Dynamic rule settings](#dynamic-rule-settings) above), applying only to that request.

If both are set, the dynamic, per-request value takes precedence over the static config value. If
neither is set, dates are shifted by the default hash-derived offset instead of a fixed one.
