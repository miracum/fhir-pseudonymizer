# gPAS

Set `PseudonymizationService=gPAS` to create pseudonyms using
[gPAS](https://www.ths-greifswald.de/en/researchers-general-public/gpas/) through its TTP FHIR
Gateway. The rule's `domain` is the gPAS domain.

| Environment Variable          | Description                                                                                                                                              | Default |
| ----------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | ------- |
| `gPAS__Url`                   | The gPAS TTP FHIR Gateway URL. E.g. `http://localhost:8080/ttp-fhir/fhir/gpas/` for gPAS `2023.1.0`. Used if `PseudonymizationService` is set to `gPAS`. | `""`    |
| `gPAS__Retry__Count`          | The number of times a failed request is retried, with exponential backoff.                                                                               | `3`     |
| `gPAS__RequestTimeoutSeconds` | How long a single request may take before it is cancelled and, if `gPAS__Retry__Count` allows, retried.                                                  | `20`    |

## Basic auth

| Environment Variable          | Description                                     | Default |
| ----------------------------- | ----------------------------------------------- | ------- |
| `gPAS__Auth__Basic__Username` | The HTTP basic auth username to connect to gPAS | `""`    |
| `gPAS__Auth__Basic__Password` | The HTTP basic auth password to connect to gPAS | `""`    |

## OAuth

| Environment Variable               | Description                       | Default |
| ---------------------------------- | --------------------------------- | ------- |
| `gPAS__Auth__OAuth__TokenEndpoint` | The URL of the token endpoint     | `""`    |
| `gPAS__Auth__OAuth__ClientId`      | The client ID                     | `""`    |
| `gPAS__Auth__OAuth__ClientSecret`  | The static (shared) client secret | `""`    |
| `gPAS__Auth__OAuth__Scope`         | The scope                         | `""`    |
| `gPAS__Auth__OAuth__Resource`      | The resource                      | `""`    |
