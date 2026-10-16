# FHIR® Pseudonymizer

[![OpenSSF Scorecard](https://img.shields.io/ossf-scorecard/github.com/miracum/fhir-pseudonymizer?label=openssf%20scorecard&style=flat)](https://scorecard.dev/viewer/?uri=github.com/miracum/fhir-pseudonymizer)
[![SLSA 3](https://slsa.dev/images/gh-badge-level3.svg)](https://slsa.dev)
[![OpenSSF Baseline](https://www.bestpractices.dev/projects/14501/baseline)](https://www.bestpractices.dev/projects/14501)
[![OpenSSF Best Practices](https://www.bestpractices.dev/projects/14501/badge)](https://www.bestpractices.dev/projects/14501)

<p align="center"><img width="100" src="docs/img/logo.svg" alt="FHIR® Pseudonymizer Logo"></p>

> Send a FHIR® resource to `/fhir/$de-identify` get it back anonymized and/or pseudonymized.

Based on the brilliant [Tools for Health Data Anonymization](https://github.com/microsoft/Tools-for-Health-Data-Anonymization).

📖 **Documentation: <https://miracum.github.io/fhir-pseudonymizer/docs/>**

## Quick start

<!-- x-release-please-start-version -->

```sh
docker run --rm -i -p 8080:8080 \
  -e AnonymizationEngineConfigInline='
fhirVersion: R4
fhirPathRules:
  - path: Resource.id
    method: cryptoHash
  - path: Patient.birthDate
    method: redact
parameters:
  cryptoHashKey: "a-secret-key-with-at-least-32-characters"
' \
  ghcr.io/miracum/fhir-pseudonymizer:v3.1.0

curl -X POST -H "Content-Type:application/fhir+json" "http://localhost:8080/fhir/\$de-identify" \
  -d '{"resourceType": "Patient", "id": "example", "birthDate": "1985-10-14", "gender": "female"}'
```

There is no default anonymization config - the service fails to start without one. The example above crypto-hashes every `Resource.id` and redacts `Patient.birthDate`. See the [documentation](https://miracum.github.io/fhir-pseudonymizer/docs/) for how to write your own rules, pseudonymize with gPAS, Vfps, entici or an MII-compliant service, read from Kafka, and for the full configuration reference.

An example for deploying using (Docker) Compose can be found in the [compose folder](./compose/README.md).

The recommended deployment is on Kubernetes. See <https://github.com/miracum/fhir-pseudonymizer/tree/master/charts/fhir-pseudonymizer> for a Helm Chart.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) and [DEVELOPMENT.md](DEVELOPMENT.md). How to build, run and test the project is described in the [development guide](https://miracum.github.io/fhir-pseudonymizer/docs/development/).

## Security

All released container images are signed and ship with SLSA Level 3 provenance, see [how to verify them](https://miracum.github.io/fhir-pseudonymizer/docs/image-verification/). To report a vulnerability, see [SECURITY.md](SECURITY.md).

## Versioning

The project follows [SemVer](https://semver.org/), excluding metrics, traces, and the contents of the container image. See the [documentation](https://miracum.github.io/fhir-pseudonymizer/docs/about/#versioning).

## Attribution

Icons made by [Freepik](https://www.freepik.com) from [Flaticon](https://www.flaticon.com/).
