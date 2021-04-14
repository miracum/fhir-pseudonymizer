# FHIR® Pseudonymizer

<p align="center"><img width="100" src="docs/img/logo.png" alt="FHIR® Pseudonymizer logo"></p>

> Send a FHIR® resource to `/fhir/$de-identify` get it back anonymized and/or pseudonymized.

Based on the brilliant [FHIR Tools for Anonymization](https://github.com/microsoft/FHIR-Tools-for-Anonymization/).

## Usage

```sh
docker run --rm -i -p 8080:8080 ghcr.io/miracum/fhir-pseudonymizer:latest
curl -X POST -H "Content-Type:application/fhir+json" "http://localhost:8080/fhir/\$de-identify" -d @benchmark/observation.json
```

All container images are pushed to [ghcr.io/miracum/fhir-pseudonymizer](https://github.com/orgs/miracum/packages/container/package/fhir-pseudonymizer) and [quay.io/miracum/fhir-pseudonymizer](https://quay.io/repository/miracum/fhir-pseudonymizer?tag=latest&tab=tags).

For deployment in Kubernetes see <https://github.com/miracum/charts/tree/master/charts/fhir-gateway> for a Helm Chart using the FHIR Pseudonymizer as one of its components.

### API Endpoints

An OpenAPI definition for the FHIR operation endpoints is available at `/swagger/`:

![Screenshot of the OpenAPI specification](docs/img/openapi.png)

#### `$de-identify`

The server provides a `/fhir/$de-identify` operation to de-identfiy received FHIR resources according to the configuration in the [anonymization.yaml](src/FhirPseudonymizer/anonymization.yaml) rules. See <https://github.com/microsoft/FHIR-Tools-for-Anonymization/> for more details on the anonymization rule configuration.

A new `pseudonymize` method was added to the default list of anonymization methods linked above. It uses [gPAS](https://www.ths-greifswald.de/en/researchers-general-public/gpas/) to create pseudonyms and replace the values in the resource with them.
For example, the following rule replaces all identifiers of type `http://terminology.hl7.org/CodeSystem/v2-0203|MR` with a pseudonym generated in the `PATIENT` domain.

```yaml
fhirPathRules:
  - path: nodesByType('Identifier').where(type.coding.system='http://terminology.hl7.org/CodeSystem/v2-0203' and type.coding.code='MR').value
    method: pseudonymize
    domain: PATIENT
```

Note that if the `domain` setting is omitted, and an id or reference is pseudonymized, then the resource name is used as the pseudonym domain. For example, pseudonymizing `"reference": "Patient/123"` will try to create a pseudonym for `123` in the `Patient` domain.

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

| Environment Variable            | Description                                                                                                                                              | Default                     |
| ------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| `GPAS__URL`                     | The gPAS TTP FHIR Gateway URL. Only required if any of the anonymization.yaml rules use the `pseudonymize` method.                                       | `""`                        |
| `GPAS__AUTH__BASIC__USERNAME`   | The HTTP basic auth username to connect to gPAS                                                                                                          | `""`                        |
| `GPAS__AUTH__BASIC__PASSWORD`   | The HTTP basic auth password to connect to gPAS                                                                                                          | `""`                        |
| `ANONYMIZATIONENGINECONFIGPATH` | Path to the `anonymization.yaml` that contains the rules to transform the resources.                                                                     | `"/etc/anonymization.yaml"` |
| `APIKEY`                        | Key that must be set in the `X-Api-Key` header to allow requests to protected endpoints.                                                                 | `""`                        |
| `GPAS__VERSION`                 | Version of gPAS to support. There were breaking changes to the FHIR API starting in 1.10.2, so explicitely set this value to 1.10.2 if you are using it. | `"1.10.1"`                  |

See [appsettings.json](src/FhirPseudonymizer/appsettings.json) for additional options.

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

Prerequisites: <https://github.com/codesenberg/bombardier>

```sh
dotnet run -c Release
```

In a different terminal

```sh
cd benchmark/
$ ./bombardier.sh

Bombarding http://localhost:5000/fhir/$de-identify for 30s using 125 connection(s)
[==========================================================================================================================] 30s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec      2170.62    5239.58  129714.63
  Latency       69.40ms     4.94ms   164.50ms
  HTTP codes:
    1xx - 0, 2xx - 54033, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    12.97MB/s
```

## Attribution

<div>Icons made by <a href="https://www.freepik.com" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></div>
