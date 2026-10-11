# Observability

## Metrics

The application exposes metrics (via OpenTelemetry's Prometheus exporter) in the Prometheus format
at the `/metrics` endpoint on its own dedicated port, `8081` by default (`Metrics__Port`). That
endpoint only answers on this port - it isn't reachable on the main app port, and the metrics port
doesn't serve anything else. Set `Metrics__Enabled=false` to turn it off.

!!! note

    Metrics are excluded from the project's [semantic versioning guarantees](about.md#versioning).

## Grafana dashboard

A ready-to-import Grafana dashboard covering these metrics is available at
[`grafana/fhir-pseudonymizer-dashboard.json`](https://github.com/miracum/fhir-pseudonymizer/blob/master/grafana/fhir-pseudonymizer-dashboard.json).
Besides throughput and failures, it shows how long anonymizing takes per resource - comparable
across bundle sizes - how much of that is spent waiting for the pseudonymization service, and the
.NET runtime's CPU, memory and garbage collection. Its Kafka consumer lag panels require a
[Kafka exporter](https://github.com/danielqsj/kafka_exporter) scraped by the same Prometheus.

## Tracing

Tracing is off by default. Enable it by setting `Tracing__Enabled=true`; the development
[compose file](development.md#development-fixtures) starts Jaeger to try it out.
