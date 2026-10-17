# fhir-pseudonymizer

[FHIR Pseudonymizer](https://github.com/miracum/fhir-pseudonymizer) - A Helm chart for deploying the MIRACUM FHIR Pseudonymizer

## TL;DR

```console
helm install fhir-pseudonymizer oci://ghcr.io/miracum/fhir-pseudonymizer/charts/fhir-pseudonymizer --create-namespace -n fhir-pseudonymizer
```

## Introduction

This chart deploys the MIRACUM FHIR Pseudonymizer on a [Kubernetes](http://kubernetes.io) cluster using the [Helm](https://helm.sh) package manager.

## Prerequisites

- Kubernetes v1.19+
- Helm v3

## Installing the Chart

To install the chart with the release name `fhir-pseudonymizer`:

```console
helm install fhir-pseudonymizer oci://ghcr.io/miracum/fhir-pseudonymizer/charts/fhir-pseudonymizer --create-namespace -n fhir-pseudonymizer
```

The command deploys the FHIR Pseudonymizer on the Kubernetes cluster in the default configuration. The [configuration](#configuration) section lists the parameters that can be configured during installation.

> **Tip**: List all releases using `helm list`

## Uninstalling the Chart

To uninstall/delete the `fhir-pseudonymizer` release:

```console
helm delete fhir-pseudonymizer -n fhir-pseudonymizer
```

The command removes all the Kubernetes components associated with the chart and deletes the release.

## Configuration

Specify each parameter using the `--set key=value[,key=value]` argument to `helm install`. For example:

```console
helm install fhir-pseudonymizer oci://ghcr.io/miracum/fhir-pseudonymizer/charts/fhir-pseudonymizer -n fhir-pseudonymizer --set replicaCount=2
```

Alternatively, a YAML file that specifies the values for the parameters can be provided while
installing the chart. For example:

```console
helm install fhir-pseudonymizer oci://ghcr.io/miracum/fhir-pseudonymizer/charts/fhir-pseudonymizer -n fhir-pseudonymizer --values values.yaml
```

### Environment variables

The chart only exposes a few dedicated values for things that need Kubernetes resources, such as `anonymizationConfig`,
`kafka.*`, or `ingress.*`. Everything else, including the pseudonymization service and its authentication, is configured
using environment variables. See the [configuration reference](https://miracum.github.io/fhir-pseudonymizer/docs/configuration/)
for the available ones.

Set them as a plain map using `extraEnvDict`:

```yaml
extraEnvDict:
  Anonymization__ShouldAddSecurityTag: "false"
  Metrics__Enabled: "true"
```

Variables that need `valueFrom`, e.g. to read a credential from a secret, are set using `extraEnv` in Kubernetes' regular
`name`/`valueFrom` syntax. If a variable is set in both, the one from `extraEnv` wins.

```yaml
extraEnv:
  - name: Anonymization__EncryptKey
    valueFrom:
      secretKeyRef:
        name: fhir-pseudonymizer-keys
        key: encrypt-key
```

### Authentication

Protected endpoints such as `$re-identify` require OAuth 2.0 access tokens in JWT format. Configure the token issuer using the
`Auth__Jwt__*` environment variables:

```yaml
extraEnvDict:
  Auth__Jwt__Authority: https://keycloak.example.com/realms/example
  Auth__Jwt__Audience: fhir-pseudonymizer
```

## Anonymization and pseudonymization

Anonymization rules are configured directly in the `values.yaml` using `anonymizationConfig`. There is no default, the
service fails to start without one. For example, the following configuration redacts all names and encrypts the medical
record and visit numbers:

```yaml
anonymizationConfig: |
  ---
  fhirVersion: R4
  fhirPathRules:
    - path: nodesByType('HumanName')
      method: redact
    - path: nodesByType('Identifier').where(type.coding.where(system='http://terminology.hl7.org/CodeSystem/v2-0203' and code='VN').exists()).value
      method: encrypt
    - path: nodesByType('Identifier').where(type.coding.where(system='http://terminology.hl7.org/CodeSystem/v2-0203' and code='MR').exists()).value
      method: encrypt
  parameters:
    dateShiftKey: ""
    dateShiftScope: resource
    enablePartialAgesForRedact: true
    enablePartialDatesForRedact: true
    enablePartialZipCodesForRedact: true
    restrictedZipCodeTabulationAreas: []
```

`anonymizationConfig` also accepts a regular YAML object instead of a string, which is useful when the rules are
composed from several values files or set with `--set`. The chart renders it to the same `anonymization.yaml`:

```yaml
anonymizationConfig:
  fhirVersion: R4
  fhirPathRules:
    - path: nodesByType('HumanName')
      method: redact
  parameters:
    dateShiftKey: ""
    dateShiftScope: resource
```

To use a ConfigMap that already exists in the namespace, e.g. one managed by another tool, set `existingAnonymizationConfigMap`
instead of `anonymizationConfig`. The chart then creates no ConfigMap of its own and mounts the existing one. The config is
read from the `anonymization.yaml` key unless `existingAnonymizationConfigMapKey` says otherwise:

```yaml
existingAnonymizationConfigMap: my-anonymization-rules
existingAnonymizationConfigMapKey: anonymization.yaml
```

Setting both `existingAnonymizationConfigMap` and `anonymizationConfig` fails the installation. Since the chart can't see the
ConfigMap's content, changing it does not restart the pods; do that yourself, e.g. with `kubectl rollout restart`.

Keys are best kept out of the config. The `Anonymization__CryptoHashKey` and `Anonymization__EncryptKey` environment
variables must be at least 32 bytes long and can be read from a secret as shown [above](#environment-variables).

### Pseudonymization service

To use the `pseudonymize` method, select a pseudonymization service and configure it using environment variables. The chart
does not deploy one. For example, to use an existing Vfps instance:

```yaml
extraEnvDict:
  PseudonymizationService: Vfps
  Vfps__Address: dns:///vfps-headless.vfps.svc.cluster.local:8081
```

Or gPAS, with the password read from a secret:

```yaml
extraEnvDict:
  PseudonymizationService: gPAS
  gPAS__Url: http://gpas:8080/ttp-fhir/fhir/gpas/
  gPAS__Auth__Basic__Username: fhir-pseudonymizer
extraEnv:
  - name: gPAS__Auth__Basic__Password
    valueFrom:
      secretKeyRef:
        name: gpas-credentials
        key: password
```

The available services and their settings are listed in the
[configuration reference](https://miracum.github.io/fhir-pseudonymizer/docs/configuration/#service-specific-settings).

## Reading from Kafka

In addition to the HTTP API, the FHIR Pseudonymizer can consume FHIR resources/bundles directly from one or more Kafka topics,
pseudonymize them, and produce the result to a per-topic output topic. Set `kafka.enabled=true`, point `kafka.bootstrapServers`
at your brokers, and list the input topics as an array:

```yaml
kafka:
  enabled: true
  bootstrapServers: "kafka:9092"
  topics:
    - fhir.patient
    - fhir.observation
```

By default, the keys of the produced messages are crypto-hashed, and the service refuses to start without a key to do that with. Set
`Kafka__CryptoHashMessageKeys__Key` to a randomly generated key of at least 32 bytes, e.g. from `openssl rand -hex 32`, preferably
read from a secret using `extraEnv`. Use the same key for every replica and keep it stable, so a message key is always hashed to
the same value. Alternatively, set `Kafka__CryptoHashMessageKeys__Enabled` to `"false"` to keep the input messages' keys.

```yaml
extraEnv:
  - name: Kafka__CryptoHashMessageKeys__Key
    valueFrom:
      secretKeyRef:
        name: fhir-pseudonymizer-kafka-keys
        key: message-key-hash-key
```

The output topic is derived from the input topic by a regular expression match-and-replace. The default prepends
`pseudonymized.` to every input topic, so `fhir.patient` becomes `pseudonymized.fhir.patient`. To instead insert it after a
common prefix, turning `fhir.patient` into `fhir.pseudonymized.patient`:

```yaml
kafka:
  outputTopicPattern: '^fhir\.'
  outputTopicReplacement: "fhir.pseudonymized."
```

Every entry of `kafka.topics`, as well as `kafka.provenanceTopic` and `kafka.consumer.groupId`, is evaluated as a template.
The consumer group id defaults to the release's fullname; note that it is also part of the name of the dead letter topic that
messages which could not be pseudonymized are sent to: `error.<input-topic>.<group-id>`.

Settings not exposed as a dedicated value can be set through the `kafka.client`, `kafka.consumer.config`, and
`kafka.producer.config` maps. Their keys are the property names of `Confluent.Kafka.ClientConfig`, `ConsumerConfig`, and
`ProducerConfig` respectively, and they are passed to the container as `Kafka__Client__<key>`, `Kafka__Consumer__<key>`, and
`Kafka__Producer__<key>` environment variables:

```yaml
kafka:
  client:
    securityProtocol: SaslSsl
    saslMechanism: ScramSha512
    saslUsername: fhir-pseudonymizer
  consumer:
    config:
      sessionTimeoutMs: "45000"
```

Since these end up in the pod spec in plaintext, set credentials via `extraEnv` with a `secretKeyRef` instead:

```yaml
extraEnv:
  - name: Kafka__Client__SaslPassword
    valueFrom:
      secretKeyRef:
        name: fhir-pseudonymizer-kafka-user
        key: password
```

If the brokers use TLS with a private CA, mount the CA certificate using `extraVolumes`/`extraVolumeMounts` and point
`kafka.client.sslCaLocation` at it.

Note that the application exposes no Kafka health check: a pod that cannot reach its brokers still reports itself as ready,
and only logs connection errors. Use the `fhirpseudonymizer_kafka_messages_total` metric and the consumer group's lag to
monitor whether messages are actually being processed.

## Values

| Key                                         | Type             | Default                                                                                                                         | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| ------------------------------------------- | ---------------- | ------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| affinity                                    | object           | `{}`                                                                                                                            | affinity for pods assignment see: <https://kubernetes.io/docs/concepts/configuration/assign-pod-node/#affinity-and-anti-affinity>                                                                                                                                                                                                                                                                                                                                                                                                                   |
| anonymizationConfig                         | string or object | `nil`                                                                                                                           | configure the anonymization rules, see <https://github.com/miracum/fhir-pseudonymizer/blob/HEAD/src/FhirPseudonymizer/anonymization.yaml> for an example. Either a YAML string (a block scalar, `                                                                                                                                                                                                                                                                                                                                                   | `) or a regular YAML object, which is rendered to `anonymization.yaml`as-is. Strings are not evaluated as templates. A`files/anonymization.yaml`inside the chart takes precedence over both. Can't be combined with`existingAnonymizationConfigMap`. Also see [README.md](README.md#anonymization-and-pseudonymization) for configuring it within this values.yaml |
| autoscaling.enabled                         | bool             | `false`                                                                                                                         | enable horizontal pod autoscaling                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| autoscaling.maxReplicas                     | int              | `5`                                                                                                                             | upper limit for the number of pods that can be set by the autoscaler; cannot be smaller than `minReplicas`.                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| autoscaling.minReplicas                     | int              | `1`                                                                                                                             | minReplicas is the lower limit for the number of replicas to which the autoscaler can scale down. It defaults to 1 pod. minReplicas is allowed to be 0 if the alpha feature gate HPAScaleToZero is enabled and at least one Object or External metric is configured. Scaling is active as long as at least one metric value is available.                                                                                                                                                                                                           |
| autoscaling.targetCPUUtilizationPercentage  | int              | `80`                                                                                                                            | target average CPU utilization (represented as a percentage of requested CPU) over all the pods; if not specified the default autoscaling policy will be used.                                                                                                                                                                                                                                                                                                                                                                                      |
| deploymentAnnotations                       | object           | `{}`                                                                                                                            | metadata.annotations to apply to the deployment                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| existingAnonymizationConfigMap              | string           | `""`                                                                                                                            | name of an existing ConfigMap containing the anonymization config, instead of having the chart create one from `anonymizationConfig`. Evaluated as a template. It is mounted into the container, so changes to it are picked up after a pod restart, which the chart does not trigger.                                                                                                                                                                                                                                                              |
| existingAnonymizationConfigMapKey           | string           | `"anonymization.yaml"`                                                                                                          | name of the key inside `existingAnonymizationConfigMap` holding the anonymization config                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| extraEnv                                    | list             | `[]`                                                                                                                            | extra environment variables to apply to the container, in Kubernetes' `name`/`value`/`valueFrom` syntax.                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| extraEnvDict                                | object           | `{}`                                                                                                                            | extra environment variables to apply to the container, as a map of variable name to value. This is the simplest way to configure the application, including the pseudonymization service and its authentication; see the [configuration reference](https://miracum.github.io/fhir-pseudonymizer/docs/configuration/) for the available variables. Values are always passed as strings. Use `extraEnv` instead if a variable needs `valueFrom`, e.g. to read a credential from a secret. If a variable is set in both, the one from `extraEnv` wins. |
| extraVolumeMounts                           | list             | `[]`                                                                                                                            | extra volumeMounts for the main fhir-pseudonymizer container                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| extraVolumes                                | list             | `[]`                                                                                                                            | extra volumes                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| fullnameOverride                            | string           | `""`                                                                                                                            | String to fully override fullname template                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| image.pullPolicy                            | string           | `"IfNotPresent"`                                                                                                                | container image pull policy                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| image.registry                              | string           | `"ghcr.io"`                                                                                                                     | container image registry                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| image.repository                            | string           | `"miracum/fhir-pseudonymizer"`                                                                                                  | container image repository                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| image.tag                                   | string           | `"v3.1.0"`                                                                                                                      | container image tag. The chart's publish workflow pins it to the digest of the image built for the release.                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| imagePullSecrets                            | list             | `[]`                                                                                                                            | image pull secrets                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| ingress.annotations                         | object           | `{}`                                                                                                                            | extra annotations to apply to the Ingress resource                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| ingress.className                           | string           | `""`                                                                                                                            | ingressClassName to use                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| ingress.enabled                             | bool             | `false`                                                                                                                         | create an Ingress for the application                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| ingress.hosts                               | list             | `[{"host":"fhir-pseudonymizer.127.0.0.1.nip.io","paths":[{"path":"/","pathType":"ImplementationSpecific","portName":"http"}]}]` | list of ingress hosts                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| ingress.tls                                 | list             | `[]`                                                                                                                            | TLS configuration                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| kafka.bootstrapServers                      | string           | `""`                                                                                                                            | comma-separated list of `host:port` Kafka brokers to bootstrap from, e.g. `kafka:9092`                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| kafka.client                                | object           | `{}`                                                                                                                            | settings shared by the consumer and the producer, set as `Kafka__Client__<key>` environment variables. Any property of `Confluent.Kafka.ClientConfig` can be set here, e.g. `securityProtocol: SaslSsl`, `saslMechanism: ScramSha512`, or `saslUsername: fhir-pseudonymizer`. Set the brokers via `kafka.bootstrapServers` instead of here. Since these end up in the pod spec in plaintext, set credentials via `extraEnv` with a `secretKeyRef` instead.                                                                                          |
| kafka.consumer.config                       | object           | `{}`                                                                                                                            | consumer-only settings layered on top of `kafka.client`, set as `Kafka__Consumer__<key>` environment variables. Any property of `Confluent.Kafka.ConsumerConfig` can be set here, e.g. `sessionTimeoutMs: "45000"`.                                                                                                                                                                                                                                                                                                                                 |
| kafka.consumer.groupId                      | string           | `"{{ include \"fhir-pseudonymizer.fullname\" . }}"`                                                                             | the Kafka consumer group id. Evaluated as a template. Note that it is also part of the name of the dead letter topic failed messages are sent to: `error.<input-topic>.<group-id>`.                                                                                                                                                                                                                                                                                                                                                                 |
| kafka.enabled                               | bool             | `false`                                                                                                                         | master switch for all Kafka integration: consuming from `kafka.topics` and publishing provenance bundles to `kafka.provenanceTopic`. Requires `kafka.bootstrapServers` to be set, as well as the `Kafka__CryptoHashMessageKeys__Key` environment variable (or `Kafka__CryptoHashMessageKeys__Enabled=false`), see [README.md](README.md#reading-from-kafka).                                                                                                                                                                                        |
| kafka.outputTopicPattern                    | string           | `"^"`                                                                                                                           | regular expression matched against each input topic's name to derive the output topic the pseudonymized resource is written to. Together with `kafka.outputTopicReplacement`, the default prepends `pseudonymized.` to every input topic, turning `fhir.test` into `pseudonymized.fhir.test`.                                                                                                                                                                                                                                                       |
| kafka.outputTopicReplacement                | string           | `"pseudonymized."`                                                                                                              | the string every match of `kafka.outputTopicPattern` is replaced with                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| kafka.producer.config                       | object           | `{}`                                                                                                                            | producer-only settings layered on top of `kafka.client`, set as `Kafka__Producer__<key>` environment variables. Any property of `Confluent.Kafka.ProducerConfig` can be set here, e.g. `lingerMs: "100"`.                                                                                                                                                                                                                                                                                                                                           |
| kafka.provenanceTopic                       | string           | `""`                                                                                                                            | if set, a Bundle containing a FHIR Provenance resource documenting the pseudonymization is published to this topic for every successfully pseudonymized resource - including those pseudonymized via the REST API. Evaluated as a template.                                                                                                                                                                                                                                                                                                         |
| kafka.topics                                | list             | `[]`                                                                                                                            | list of input topics to consume FHIR resources/bundles from, e.g. `["fhir.patient", "fhir.observation"]`. Each entry is evaluated as a template. Left empty, nothing is consumed from Kafka and only provenance for resources pseudonymized via the REST API is published, if `kafka.provenanceTopic` is set.                                                                                                                                                                                                                                       |
| kafka.workerChannelCapacity                 | string           | `""`                                                                                                                            | the maximum number of messages buffered per worker before the consumer pauses fetching new ones (backpressure). Left empty, this defaults to 100.                                                                                                                                                                                                                                                                                                                                                                                                   |
| kafka.workerCount                           | string           | `""`                                                                                                                            | the number of workers anonymizing messages concurrently. Left empty, this defaults to the number of CPU cores visible to the container.                                                                                                                                                                                                                                                                                                                                                                                                             |
| metrics.serviceMonitor.additionalLabels     | object           | `{}`                                                                                                                            | additional labels for the ServiceMonitor resource, e.g. `release: prometheus`                                                                                                                                                                                                                                                                                                                                                                                                                                                                       |
| metrics.serviceMonitor.enabled              | bool             | `false`                                                                                                                         | if enabled, creates a ServiceMonitor instance for Prometheus Operator-based monitoring                                                                                                                                                                                                                                                                                                                                                                                                                                                              |
| nameOverride                                | string           | `""`                                                                                                                            | String to partially override fullname template (will maintain the release name)                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| nodeSelector                                | object           | `{}`                                                                                                                            | node labels for pods assignment see: <https://kubernetes.io/docs/concepts/scheduling-eviction/assign-pod-node/>                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| podAnnotations                              | object           | `{}`                                                                                                                            | pod annotations                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| podDisruptionBudget.enabled                 | bool             | `false`                                                                                                                         | create a PodDisruptionBudget resource                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| podDisruptionBudget.maxUnavailable          | string           | `""`                                                                                                                            | Maximum unavailable instances; ignored if there is no PodDisruptionBudget                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| podDisruptionBudget.minAvailable            | int              | `1`                                                                                                                             | Minimum available instances; ignored if there is no PodDisruptionBudget                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| replicaCount                                | int              | `1`                                                                                                                             | number of replicas. This components can also be easily scaled horizontally if necessary.                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| resources                                   | object           | `{}`                                                                                                                            | configure the resource requests and limits                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| resourcesPreset                             | string           | `"small"`                                                                                                                       | set container resources according to one common preset (allowed values: none, nano, micro, small, medium, large, xlarge, 2xlarge). This is ignored if `resources` is set (`resources` is recommended for production). More information: <https://github.com/bitnami/charts/blob/main/bitnami/common/templates/_resources.tpl#L15>                                                                                                                                                                                                                   |
| service.metricsPort                         | int              | `8081`                                                                                                                          | port to expose metrics at under the `/metrics` path                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| service.port                                | int              | `8080`                                                                                                                          | port to expose the HTTP API at                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| service.type                                | string           | `"ClusterIP"`                                                                                                                   | the type of the Service exposing the fhir-pseudonymizer                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| serviceAccount.annotations                  | object           | `{}`                                                                                                                            | Annotations to add to the service account                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           |
| serviceAccount.automountServiceAccountToken | bool             | `false`                                                                                                                         | whether to automount the SA token                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| serviceAccount.create                       | bool             | `false`                                                                                                                         | Specifies whether a service account should be created                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| serviceAccount.name                         | string           | `""`                                                                                                                            | The name of the service account to use. If not set and create is true, a name is generated using the fullname template                                                                                                                                                                                                                                                                                                                                                                                                                              |
| tests.automountServiceAccountToken          | bool             | `false`                                                                                                                         | whether the Helm test pods automount the service account token                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| tests.resources                             | object           | `{}`                                                                                                                            | configure the test pods resource requests and limits                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| tests.resourcesPreset                       | string           | `"nano"`                                                                                                                        | set container resources according to one common preset (allowed values: none, nano, micro, small, medium, large, xlarge, 2xlarge). This is ignored if `tests.resources` is set. More information: <https://github.com/bitnami/charts/blob/main/bitnami/common/templates/_resources.tpl#L15>                                                                                                                                                                                                                                                         |
| tolerations                                 | list             | `[]`                                                                                                                            | tolerations for pods assignment see: <https://kubernetes.io/docs/concepts/configuration/taint-and-toleration/>                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| topologySpreadConstraints                   | list             | `[]`                                                                                                                            | pod topology spread configuration see: <https://kubernetes.io/docs/concepts/workloads/pods/pod-topology-spread-constraints/#api>                                                                                                                                                                                                                                                                                                                                                                                                                    |
