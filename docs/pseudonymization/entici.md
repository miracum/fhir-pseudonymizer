# entici

Set `PseudonymizationService=entici` to create pseudonyms using
[entici](https://www.entici.org/).

| Environment Variable            | Description                                                                                               | Default |
| ------------------------------- | --------------------------------------------------------------------------------------------------------- | ------- |
| `entici__Url`                   | The entici service base URL for FHIR operations. Used if `PseudonymizationService` is set to `entici`.    | `""`    |
| `entici__Retry__Count`          | The number of times a failed request is retried, with exponential backoff.                                | `3`     |
| `entici__RequestTimeoutSeconds` | How long a single request may take before it is cancelled and, if `entici__Retry__Count` allows, retried. | `20`    |

When using entici as a pseudonymization backend, you need to set additional settings for each rule that uses the `pseudonymize` method. These can be set under a `entici` section inside the anonymization config:

```yaml
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.where(system='http://terminology.hl7.org/CodeSystem/v2-0203' and code='MR').exists()).value
    method: pseudonymize
    # the domain will be used as the system of the identifier when invoking the entici pseudonymize operation
    domain: https://fhir.example.com/identifiers/patient-id
    entici:
      # the type of FHIR resource this pseudonym should be associated with
      resourceType: Patient
      # (optional) the project within entici the pseudonym is a part of
      project: some-internal-entici-project-name
```

## OAuth

| Environment Variable                 | Description                       | Default |
| ------------------------------------ | --------------------------------- | ------- |
| `entici__Auth__OAuth__TokenEndpoint` | The URL of the token endpoint     | `""`    |
| `entici__Auth__OAuth__ClientId`      | The client ID                     | `""`    |
| `entici__Auth__OAuth__ClientSecret`  | The static (shared) client secret | `""`    |
| `entici__Auth__OAuth__Scope`         | The scope                         | `""`    |
| `entici__Auth__OAuth__Resource`      | The resource                      | `""`    |
