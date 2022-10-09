# FHIR® Pseudonymizer

<p align="center"><img width="100" src="docs/img/logo.png" alt="FHIR® Pseudonymizer Logo"></p>

> Send a FHIR® resource to `/fhir/$de-identify` get it back anonymized and/or pseudonymized.

Based on the brilliant [FHIR Tools for Anonymization](https://github.com/microsoft/FHIR-Tools-for-Anonymization/).

## Usage

```sh
docker run --rm -i -p 8080:8080 ghcr.io/miracum/fhir-pseudonymizer:latest
curl -X POST -H "Content-Type:application/fhir+json" "http://localhost:8080/fhir/\$de-identify" -d @benchmark/observation.json
```

Container images are pushed to the following registries:

- `ghcr.io/miracum/fhir-pseudonymizer:latest`
- `quay.io/miracum/fhir-pseudonymizer:latest`
- `harbor.miracum.org/miracum-etl/fhir-pseudonymizer:latest`

For deployment in Kubernetes see <https://github.com/miracum/charts/tree/master/charts/fhir-gateway> for a Helm Chart using the FHIR Pseudonymizer as one of its components.

### API Endpoints

An OpenAPI definition for the FHIR operation endpoints is available at `/swagger/`:

![Screenshot of the OpenAPI specification](docs/img/openapi.png)

#### `$de-identify`

The server provides a `/fhir/$de-identify` operation to de-identfiy received FHIR resources according to the configuration in the [anonymization.yaml](src/FhirPseudonymizer/anonymization.yaml) rules. See <https://github.com/microsoft/FHIR-Tools-for-Anonymization/> for more details on the anonymization rule configuration.

The service comes with a sample configuration file to help meet the requirements of HIPAA Safe Harbor Method (2)(i): [hipaa-anonymization.yaml](src/FhirPseudonymizer/hipaa-anonymization.yaml).This configuration can be used by setting `ANONYMIZATIONENGINECONFIGPATH=/etc/hipaa-anonymization.yaml`.

A new `pseudonymize` method was added to the default list of anonymization methods linked above. It uses [gPAS](https://www.ths-greifswald.de/en/researchers-general-public/gpas/) to create pseudonyms and replace the values in the resource with them.
For example, the following rule replaces all identifiers of type `http://terminology.hl7.org/CodeSystem/v2-0203|MR` with a pseudonym generated in the `PATIENT` domain.

```yaml
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.system='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value
    method: pseudonymize
    domain: PATIENT
```

Note that if the `domain` setting is omitted, and an ID or reference is pseudonymized, then the resource name is used as the pseudonym domain. For example, pseudonymizing `"reference": "Patient/123"` will try to create a pseudonym for `123` in the `Patient` domain.

Note that all methods defined in [FHIR-Tools-for-Anonymization](https://github.com/microsoft/FHIR-Tools-for-Anonymization/) are supported. For example, to clamp a patient's birthdate if they were born before January 1st 1931 to 01/01/1930, use:

```yaml
fhirPathRules:
  - path: Patient.birthDate
    method: generalize
    cases:
      "$this < @1931-01-01": "@1930-01-01"
    otherValues: keep
```

#### `$de-pseudonymize`

The `/fhir/$de-pseudonymize` operation is used to revert the `pseudonymize` and `encrypt` methods applied to any resource.
Accessing this endpoint requires authentication. So make sure to set the `APIKEY` env var.

> ⚠ if decryption or de-pseudonymization of a value fails, then the original value is returned. This behavior may change or be made configurable in the future.

#### `/metrics`

While not part of the "user" API, the application exposes metrics in the Prometheus format at the `/metrics` endpoint.

## Configuration

You can configure the anonymization and pseudonymization rules in the `anonymization.yaml` config file.
It's mounted at `/etc/anonymization.yaml` within the container by default.
See <https://github.com/microsoft/FHIR-Tools-for-Anonymization> for details on the syntax and options.

Additionally, there are some optional configuration values that can be set as environment variables:

| Environment Variable              | Description                                                                                                                                                                                                              | Default                     |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ | --------------------------- |
| `gPAS__Url`                       | The gPAS TTP FHIR Gateway URL. Only required if any of the anonymization.yaml rules use the `pseudonymize` method.                                                                                                       | `""`                        |
| `gPAS__Auth__Basic__Username`     | The HTTP basic auth username to connect to gPAS                                                                                                                                                                          | `""`                        |
| `gPAS__Auth__Basic__Password`     | The HTTP basic auth password to connect to gPAS                                                                                                                                                                          | `""`                        |
| `AnonymizationEngineConfigPath`   | Path to the `anonymization.yaml` that contains the rules to transform the resources.                                                                                                                                     | `"/etc/anonymization.yaml"` |
| `ApiKey`                          | Key that must be set in the `X-Api-Key` header to allow requests to protected endpoints.                                                                                                                                 | `""`                        |
| `gPAS__Version`                   | Version of gPAS to support. There were breaking changes to the FHIR API in 1.10.2 and 1.10.3, so explicitely set this value if you are using a later version than 1.10.1.                                                | `"1.10.1"`                  |
| `UseSystemTextJsonFhirSerializer` | Enable the new `System.Text.Json`-based FHIR serializer to significantly [improve throughput and latencies](#usesystemtextjsonfhirserializer). See <https://github.com/FirelyTeam/firely-net-sdk/releases/tag/v4.0.0-r4> | `false`                     |

See [appsettings.json](src/FhirPseudonymizer/appsettings.json) for additional options.

## Dynamic rule settings

Anonymization and pseudonymization rules in the `anonymization.yaml` config file can be overridden and/or extended on a per request basis.

Pseudonymization via gPAS supports a `domain-prefix` rule setting which can be used to dynamically configure the target gPAS domain by providing its value as part of the request body.

The following example shows how to use this feature to use a single service configuration in order to support multiple projects which have the same basic domain names, prefixed by a project name.

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

Providing the prefix (i.e. miracum- or test-) via the request, pseudonymization can be done with the same rules for different projects.

#### Request body

Rule settings can be provided by using the `Parameters` resource with a `settings` parameter and parts consisting of the settings key and value.
The `resource` parameter must contain the actual target resource.

The following request body and the (fixed) configuration settings above will result in the target domain `miracum-patient`.

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

Note: The domain name could also have been replaced completely by overriding the `domain` setting with the desired value. This works for all rule settings regardless of the `method` value.

## Development

### Build

```sh
dotnet restore src/FhirPseudonymizer
dotnet build src/FhirPseudonymizer
```

Or using Docker:

```sh
docker build -t fhir-pseudonymizer:local-build .
```

### Run

```sh
dotnet run --project src/FhirPseudonymizer
```

### Test

```sh
dotnet test src/FhirPseudonymizer.Tests/
```

### Install Pre-commit Hooks

```sh
pre-commit install
pre-commit install --hook-type commit-msg
```

## Benchmark

> **Note**
> Example runs were conducted on the following hardware:

```console
OS=Windows 11 (10.0.22000.978/21H2)
12th Gen Intel Core i9-12900K, 1 CPU, 24 logical and 16 physical cores
32GiB of DDR4 4800MHz RAM
Samsung SSD 980 Pro 1TiB
.NET SDK=7.0.100-rc.1.22431.12
```

Prerequisites: <https://github.com/codesenberg/bombardier>

```sh
dotnet run -c Release --project=src/FhirPseudonymizer
```

In a different terminal

```sh
cd benchmark/
$ ./bombardier.sh

Bombarding http://localhost:5000/fhir/$de-identify for 1m0s using 125 connection(s)
[====================================================================================================================] 1m0s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec     13107.78    1552.49   18917.77
  Latency        9.53ms   559.41us    53.88ms
  Latency Distribution
     50%     9.00ms
     75%    11.00ms
     90%    12.00ms
     95%    13.00ms
     99%    16.73ms
  HTTP codes:
    1xx - 0, 2xx - 786655, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    96.37MB/s
```

### UseSystemTextJsonFhirSerializer

You can improve throughput and P99 latencies by opting-in to using the System.Text.Json based FHIR resource serializer.
It can be enabled via `appsettings.json` or using the `UseSystemTextJsonFhirSerializer` environment variable:

```sh
UseSystemTextJsonFhirSerializer=true dotnet run -c Release --project=src/FhirPseudonymizer
```

```console
Bombarding http://localhost:5000/fhir/$de-identify for 1m0s using 125 connection(s)
[====================================================================================================================] 1m0s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec     21508.39    3751.90   38993.50
  Latency        5.80ms     1.63ms   442.82ms
  Latency Distribution
     50%     5.00ms
     75%     6.00ms
     90%     7.75ms
     95%     9.00ms
     99%    12.00ms
  HTTP codes:
    1xx - 0, 2xx - 1291159, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:   158.17MB/s
```

## Verify image integrity

All released container images are signed using [cosign](https://github.com/sigstore/cosign).
The public key hosted at <https://miracum.github.io/cosign.pub> (see [here](https://github.com/miracum/miracum.github.io) for the repository source) may be used to verify them:

```sh
cosign verify --key https://miracum.github.io/cosign.pub ghcr.io/miracum/fhir-pseudonymizer:latest
```

## Attribution

Icons made by [Freepik](https://www.freepik.com) from [Flaticon](https://www.flaticon.com/).
