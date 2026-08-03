# `$de-identify` vs. `v3alpha1`'s `$de-identify` benchmark

A [k6](https://k6.io/) script that compares latency and throughput between:

- `/fhir/$de-identify` - the existing endpoint, which applies the server's statically configured
  anonymization rules (`AnonymizationEngineConfigPath`/`AnonymizationEngineConfigInline`).
- `/v3alpha1/fhir/$de-identify` - the newer endpoint, which receives the anonymization rules as a
  base64-encoded YAML `Attachment` alongside the resource, both wrapped in a `Parameters` resource.

Both scenarios de-identify the same [`../observation.json`](../observation.json) resource using
the same rules (the server's own [`anonymization.yaml`](../../src/FhirPseudonymizer/anonymization.yaml)),
so the comparison isolates the cost of the two endpoints' request handling rather than differences
in the rules applied.

## Prerequisites

- [k6](https://grafana.com/docs/k6/latest/set-up/install-k6/)
- A running instance of the server, started with the default configuration:

  ```sh
  dotnet run -c Release --project=src/FhirPseudonymizer
  ```

## Run

```sh
cd benchmark/k6
k6 run de-identify-comparison.js
```

By default this runs each scenario at 20 VUs for 30s (plus a short ramp-up/down). Override with:

```sh
BASE_URL=http://localhost:5000 VUS=50 DURATION_SECONDS=60 k6 run de-identify-comparison.js
```

The two scenarios run one after the other (`v3alpha1` starts only once `legacy` has fully ramped
down), so they never compete for the server's resources at the same time. The summary at the end
of the run reports `legacy_de_identify_duration`/`v3alpha1_de_identify_duration` (latency) and
`legacy_de_identify_requests`/`v3alpha1_de_identify_requests` (throughput, reqs/s) side by side.
