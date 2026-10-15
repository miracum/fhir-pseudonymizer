# Development

See also [DEVELOPMENT.md](https://github.com/miracum/fhir-pseudonymizer/blob/master/DEVELOPMENT.md) and
[CONTRIBUTING.md](https://github.com/miracum/fhir-pseudonymizer/blob/master/CONTRIBUTING.md) in the repository for commit message
conventions, code style and the release process.

## Development fixtures

To test Vfps and tracing via Jaeger, run

```sh
docker compose -f compose.dev.yaml up
```

to also start a Keycloak instance with a pre-configured fhir-pseudonymizer client, set
`--profile=keycloak`:

```sh
docker compose -f compose.dev.yaml --profile=keycloak up
```

The `Development` environment's `appsettings.Development.json` uses it as the issuer of the
tokens required for [`$re-identify`](de-identify.md#re-identify), so a token for the
pre-configured client can be used to call it:

```sh
TOKEN=$(curl -s \
  -d grant_type=client_credentials \
  -d client_id=fhir-pseudonymizer \
  -d client_secret=3X3XlDpW7qw76sVUNP6Q8RliRJJi2tBr \
  http://localhost:8083/realms/fhir-pseudonymizer-test/protocol/openid-connect/token | jq -r .access_token)
```

Request it via `localhost`, not `127.0.0.1`: Keycloak sets the token's issuer to the address it
was requested from, and the issuer has to match the configured `Auth__Jwt__Authority`.

To also start a local Kafka broker for testing the [Kafka](kafka.md) consumer, set
`--profile=kafka`:

```sh
docker compose -f compose.dev.yaml --profile=kafka up
```

## Build

```sh
dotnet restore
dotnet build
```

Or using Docker:

```sh
docker build -t ghcr.io/miracum/fhir-pseudonymizer:local-build .
```

## Run

```sh
dotnet run --project src/FhirPseudonymizer
```

## Test

```sh
dotnet test src/FhirPseudonymizer.Tests/
```

## Install pre-commit hooks

```sh
pre-commit install
pre-commit install --hook-type commit-msg
```

## Run iter8 SLO experiments locally

```sh
ITER8_CLI_URL="https://github.com/iter8-tools/iter8/releases/download/v0.13.18/iter8-linux-amd64.tar.gz"
curl -LSs "${ITER8_CLI_URL}" | tar xz
mv linux-amd64/iter8 /usr/local/bin/iter8
chmod +x /usr/local/bin/iter8
iter8 version

kind create cluster

export IMAGE_TAG="iter8-test"

docker build -t ghcr.io/miracum/fhir-pseudonymizer:${IMAGE_TAG} .

kind load docker-image ghcr.io/miracum/fhir-pseudonymizer:${IMAGE_TAG}

# vfps is the pseudonymization backend and is deployed separately
# (argo workflows is only needed by the chaos test)
helm dependency update tests/prerequisites
helm upgrade --install \
  --set="argo-workflows.enabled=false" \
  --wait \
  --timeout=10m \
  prerequisites tests/prerequisites

helm dependency update charts/fhir-pseudonymizer
helm upgrade --install \
  --set="image.tag=${IMAGE_TAG}" \
  -f tests/iter8/values.yaml \
  --wait \
  --timeout=10m \
  fhir-pseudonymizer charts/fhir-pseudonymizer

kubectl apply -f tests/iter8/experiment.yaml

iter8 k assert -c completed --timeout 15m
iter8 k assert -c nofailure,slos
iter8 k report

# to restart:
kubectl delete job default-1-job
kubectl apply -f tests/iter8/experiment.yaml
```

## Preview this documentation

The documentation is built with [Zensical](https://zensical.org/) from the Markdown files in
the [docs](https://github.com/miracum/fhir-pseudonymizer/tree/master/docs) folder. To preview it locally with [uv](https://docs.astral.sh/uv/):

```sh
uv run zensical serve
```

and open <http://localhost:8000/>. CI builds the site with `--strict` on every pull request, so
broken links between pages fail the build.

The site is published to GitHub Pages at `/docs/`, next to the
[playground](https://miracum.github.io/fhir-pseudonymizer/playground/) and the
[benchmark dashboard](https://miracum.github.io/fhir-pseudonymizer/benchmarks/), by the
`deploy-pages` job in
[ci.yaml](https://github.com/miracum/fhir-pseudonymizer/blob/master/.github/workflows/ci.yaml).
