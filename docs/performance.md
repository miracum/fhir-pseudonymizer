# Performance

!!! note

    Example runs were conducted on the following hardware:

    ```console
    OS=Windows 11 (10.0.26300)
    12th Gen Intel Core i9-12900K, 1 CPU, 24 logical and 16 physical cores
    32GiB of DDR5 4800MHz RAM
    Samsung SSD 980 Pro 1TiB
    .NET SDK=10.0.401
    ```

## Load test with bombardier

Prerequisites: [bombardier](https://github.com/codesenberg/bombardier)

The server is started in the `Development` environment with Kafka disabled and the default log
level, so that the debug logging and the Kafka consumer from `appsettings.Development.json`
don't skew the results:

```sh
ASPNETCORE_ENVIRONMENT=Development \
ASPNETCORE_URLS=http://localhost:5000 \
Kafka__Enabled=false \
Logging__LogLevel__Default=Information \
dotnet run -c Release --no-launch-profile --project=src/FhirPseudonymizer
```

!!! note

    On Windows, the Prometheus metrics listener binds to `http://+:<MetricsPort>/metrics/`, which
    requires an elevated shell (or a URL ACL reservation via `netsh http add urlacl`).

In a different terminal:

```sh
cd benchmark/
./bombardier.sh
```

which produces output like:

```console
Bombarding http://localhost:5000/fhir/$de-identify for 1m0s using 125 connection(s)
[====================================================================================================================] 1m0s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec     26416.66    5890.85   35211.27
  Latency        4.73ms   747.37us   148.68ms
  Latency Distribution
     50%     4.43ms
     75%     4.85ms
     90%     5.51ms
     95%     8.10ms
     99%    10.95ms
  HTTP codes:
    1xx - 0, 2xx - 1582351, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:   214.17MB/s
```

## Static vs. dynamic config

[benchmark/k6/](https://github.com/miracum/fhir-pseudonymizer/tree/master/benchmark/k6) contains a [k6](https://k6.io/) script that compares
latency and throughput between the `/fhir/$de-identify` endpoint using the server's statically
configured rules and the same endpoint using a per-request [dynamic config](dynamic-config.md)
(rules sent as a base64-encoded YAML `Attachment`). See its
[README](https://github.com/miracum/fhir-pseudonymizer/tree/master/benchmark/k6/README.md) for how to run it.

## Microbenchmarks

[src/FhirPseudonymizer.Benchmarks](https://github.com/miracum/fhir-pseudonymizer/tree/master/src/FhirPseudonymizer.Benchmarks) contains
[BenchmarkDotNet](https://benchmarkdotnet.org/) microbenchmarks for the anonymization engine
itself (no HTTP involved). Besides benchmarking the parsing of the shipped `anonymization.yaml`
and `hipaa-anonymization.yaml` configs, `AnonymizationBenchmarks` runs a full de-identify pass
(parse + anonymize + serialize) using a "complex" config
([complex-anonymization.yaml](https://github.com/miracum/fhir-pseudonymizer/blob/master/src/FhirPseudonymizer.Benchmarks/complex-anonymization.yaml),
combining `redact`, `cryptoHash`, `dateshift`, `perturb`, `encrypt` and `pseudonymize` rules)
against the large Synthea-generated bundle also used by the
[snapshot tests](https://github.com/miracum/fhir-pseudonymizer/tree/master/src/FhirPseudonymizer.Tests/Fixtures/Data/Resources). The `pseudonymize`
rule is handled by a trivial mocked `IPseudonymServiceClient` so the benchmark doesn't depend on a
running gPAS/Vfps/entici/MII instance.

Run all benchmarks with:

```sh
dotnet run -c Release --project src/FhirPseudonymizer.Benchmarks
```

Results are tracked over time using
[github-action-benchmark](https://github.com/benchmark-action/github-action-benchmark) via the
`microbenchmarks` job in [ci.yaml](https://github.com/miracum/fhir-pseudonymizer/blob/master/.github/workflows/ci.yaml). On every pull request, it
compares the new results against the stored history and comments on the PR if a benchmark
regresses beyond the configured threshold. On every push to `master`, it instead appends the new
results to that history and publishes the rendered dashboard as part of the same GitHub Pages
deployment as this site, at
<https://miracum.github.io/fhir-pseudonymizer/benchmarks/>.
