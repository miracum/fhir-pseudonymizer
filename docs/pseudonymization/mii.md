# MII

Set `PseudonymizationService=Mii` to create pseudonyms using any backend that implements the
[MII Pseudonymization Implementation Guide](https://medizininformatik-initiative.github.io/mii-interface-module-pseudonymization/)
`2026.1.0`. The client uses the `$pseudonymize` and `$de-pseudonymize` operations. It sends and
reads the parameters `context`, `original` and `pseudonym` as `Identifier` values only. The
`domain` of a rule becomes the value of the `context` identifier.

| Environment Variable         | Description                                                                                            | Default |
| ---------------------------- | ------------------------------------------------------------------------------------------------------ | ------- |
| `Mii__Url`                   | The base URL of the MII pseudonymization service. Used if `PseudonymizationService` is set to `Mii`.   | `""`    |
| `Mii__Retry__Count`          | The number of times a failed request is retried, with exponential backoff.                             | `3`     |
| `Mii__RequestTimeoutSeconds` | How long a single request may take before it is cancelled and, if `Mii__Retry__Count` allows, retried. | `20`    |

When using the Mii service as a pseudonymization backend, you can optionally set the identifier systems used in the requests for each rule that uses the `pseudonymize` method. These can be set under a `mii` section inside the anonymization config. If a system is not set, the corresponding identifier is sent with only a `value`:

```yaml
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.where(system='http://terminology.hl7.org/CodeSystem/v2-0203' and code='MR').exists()).value
    method: pseudonymize
    # the domain will be used as the value of the "context" identifier
    domain: Transfer1
    mii:
      # the identifier system set on the "context" identifier (optional)
      contextSystem: https://fhir.example.com/identifiers/context
      # the identifier system set on the "original" identifier (optional)
      originalSystem: https://fhir.example.com/identifiers/patient-id
```

## Basic auth

| Environment Variable         | Description                                             | Default |
| ---------------------------- | ------------------------------------------------------- | ------- |
| `Mii__Auth__Basic__Username` | The HTTP basic auth username to connect to the service. | `""`    |
| `Mii__Auth__Basic__Password` | The HTTP basic auth password to connect to the service. | `""`    |

## OAuth

| Environment Variable              | Description                       | Default |
| --------------------------------- | --------------------------------- | ------- |
| `Mii__Auth__OAuth__TokenEndpoint` | The URL of the token endpoint     | `""`    |
| `Mii__Auth__OAuth__ClientId`      | The client ID                     | `""`    |
| `Mii__Auth__OAuth__ClientSecret`  | The static (shared) client secret | `""`    |
| `Mii__Auth__OAuth__Scope`         | The scope                         | `""`    |
| `Mii__Auth__OAuth__Resource`      | The resource                      | `""`    |
