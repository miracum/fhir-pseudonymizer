# Getting started

## Run it

<!-- x-release-please-start-version -->

```sh
docker run --rm -i -p 8080:8080 \
  ghcr.io/miracum/fhir-pseudonymizer:v2.38.0
```

<!-- x-release-please-end-version -->

In a second terminal, send it a sample Observation (from a checkout of the
[repository](https://github.com/miracum/fhir-pseudonymizer)):

```sh
curl -X POST -H "Content-Type:application/fhir+json" \
  "http://localhost:8080/fhir/\$de-identify" \
  -d @benchmark/observation.json
```

This uses the
[default anonymization config](https://github.com/miracum/fhir-pseudonymizer/blob/master/src/FhirPseudonymizer/anonymization.yaml),
which redacts names and encrypts patient and visit identifiers. Replace it with your own rules
by mounting a file at `/etc/anonymization.yaml` - see [De-identification](de-identify.md).

The OpenAPI definition of the FHIR operation endpoints is served at
<http://localhost:8080/swagger/>:

![Screenshot of the OpenAPI specification](img/openapi.png)

## Docker Compose

The [compose folder](https://github.com/miracum/fhir-pseudonymizer/tree/master/compose) contains a
stack that uses an example anonymization config based on the
[HIPAA Safe Harbor rules](https://github.com/miracum/fhir-pseudonymizer/blob/master/compose/anonymization-hipaa.yaml):

```sh
docker compose up
# or
nerdctl compose up
# or
podman-compose up
```

Open <http://localhost:8080/swagger>, or POST any FHIR resource to
`http://localhost:8080/fhir/$de-identify`.

## Kubernetes

The recommended deployment is on Kubernetes. Use the
[Helm chart](https://github.com/miracum/charts/tree/master/charts/fhir-pseudonymizer).

## Next steps

- Write rules for your data: [De-identification](de-identify.md).
- Create reversible, consistent pseudonyms: pick a [pseudonymization service](pseudonymization/vfps.md).
- Set keys and other options: [Configuration reference](configuration.md).
- Process messages from [Kafka](kafka.md) instead of HTTP requests.
