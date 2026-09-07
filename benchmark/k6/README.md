# `$de-identify` - static vs. dynamic config benchmark

A [k6](https://k6.io/) script that compares latency and throughput between:

- `/fhir/$de-identify` with a plain resource body - applies the server's statically configured
  anonymization rules (`AnonymizationEngineConfigPath`/`AnonymizationEngineConfigInline`).
- `/fhir/$de-identify` with a `Parameters` body carrying a `config` part - applies rules sent
  per-request as a base64-encoded YAML `Attachment` alongside the resource, both wrapped in a
  `Parameters` resource (see [Dynamic config](../../README.md#dynamic-config) in the main
  README).

Both scenarios de-identify the same [`../observation.json`](../observation.json) resource using
the same rules (the server's own [`anonymization.yaml`](../../src/FhirPseudonymizer/anonymization.yaml)),
so the comparison isolates the cost of building/caching a per-request rule set rather than
differences in the rules applied.

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

The two scenarios run one after the other (`dynamic_config` starts only once `static_config` has
fully ramped down), so they never compete for the server's resources at the same time. The
summary at the end of the run reports `static_config_de_identify_duration`/
`dynamic_config_de_identify_duration` (latency) and `static_config_de_identify_requests`/
`dynamic_config_de_identify_requests` (throughput, reqs/s) side by side.
