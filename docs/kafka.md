# Reading from Kafka

In addition to the HTTP API, the application can consume FHIR resources/bundles directly from one
or more Kafka topics, pseudonymize them, and publish the result to an output topic.

This requires setting `Kafka__Enabled=true`, `Kafka__Client__BootstrapServers`, and at least one
topic in `Kafka__Topics`. See the [options](#options) below.

## Output topics and keys

By default, the output message's key is not the original message's key but its crypto-hash - the
lowercase hex HMAC-SHA256, keyed with `Kafka__CryptoHashMessageKeys__Key` - since keys are often
identifiers, e.g. a patient's id. The same key is always hashed to the same value (as long as
`Kafka__CryptoHashMessageKeys__Key` doesn't change), so messages that shared a key still share one
after pseudonymization, and with it their output partition. If `Kafka__CryptoHashMessageKeys__Key`
is the same as the anonymization config's `cryptoHashKey` (with the default `hmacSha256` algorithm
and no [key derivation](crypto-hash.md#key-derivation)), a message key is hashed to the same
value as a `cryptoHash` rule turns the same text into. Set
`Kafka__CryptoHashMessageKeys__Enabled=false` to keep the original keys instead.

The output topic for a given input topic is derived by applying a regular expression
match-and-replace (`Kafka__OutputTopicPattern` / `Kafka__OutputTopicReplacement`) to the input
topic's name. By default, this just prepends `pseudonymized.` to every topic, e.g. `fhir.test`
becomes `pseudonymized.fhir.test`. To instead insert it after a common prefix, e.g. turning
`fhir.test` into `fhir.pseudonymized.test`, set `Kafka__OutputTopicPattern=^fhir\.` and
`Kafka__OutputTopicReplacement=fhir.pseudonymized.`.

## Concurrency

Messages are anonymized concurrently by `Kafka__WorkerCount` consumers (defaulting to the number
of CPU cores), all members of the same consumer group.

Each consumer prefetches up to `Kafka__Consumer__QueuedMaxMessagesKbytes` worth of messages (16
MiB unless set, instead of librdkafka's default of 64 MiB, since there is one consumer per
worker). A consumer that is busy simply stops fetching more, without holding up the others.

## Failures and dead-lettering

Transient failures calling the configured pseudonymization service (gPAS, Vfps, entici, MII) are
retried with backoff at the HTTP/gRPC client level and, if they persist, the message is retried
indefinitely (with exponential backoff of up to one minute) instead of being dead-lettered, since
the message itself isn't at fault. Meanwhile, its partition is paused, while the consumer carries
on with its other partitions.

If processing a message fails for any other reason (e.g. malformed input, the pseudonymization
service rejecting a value - see the [422 status](de-identify.md#error-responses) - or an
unrecoverable error), or its pseudonymized version can't be delivered to the output topic (or its
provenance to `Kafka__ProvenanceTopic`, see [Provenance](provenance.md)), the original,
unmodified message is published to a dead letter topic named `error.<input-topic>.<group-id>`
(mirroring Spring Kafka's default dead letter topic naming), with headers describing the failure
(`x-error-type`, `x-error-message`, `x-source-topic`, `x-source-partition`, `x-source-offset`).

## Delivery guarantees

A message's offset is only committed once the broker has acknowledged the message it was turned
into - on the output or the dead letter topic - and those of all earlier messages of the same
partition, too. This way, a crash or restart never skips a message, though it may process some of
them twice: after a crash, the messages processed since the last commit (every
`Kafka__Consumer__AutoCommitIntervalMs`, 5 seconds by default) are processed again.

When a rebalance moves partitions to another replica instead, e.g. when scaling up or down, their
queued messages are dropped, but the ones already being processed are given up to 10 seconds to
finish, and their offsets committed, before the partitions are handed over, so this doesn't
produce duplicates. If even publishing a message to the dead letter topic fails, the application
stops instead of moving past it, so that it is reprocessed after a restart.

## Validation

Messages are parsed and serialized using the System.Text.Json-based FHIR serializer. It validates
resources while parsing them, so a message that isn't valid FHIR (e.g. a resource with an id
containing characters FHIR doesn't allow, or missing required elements) is published to the dead
letter topic.

## Options

| Environment Variable                              | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              | Default             |
| ------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------- |
| `Kafka__Enabled`                                  | Master switch for all Kafka integration - the producer/consumer, and therefore also Kafka-based provenance publishing (`Kafka__ProvenanceTopic`). Left `false`, no Kafka clients are created regardless of any other `Kafka__*` setting. Defaults to `true` in the Development environment.                                                                                                                                                                                                                                                                                                                                              | `false`             |
| `Kafka__Topics__0`, `__1`, ... or `Kafka__Topics` | The input topics to consume FHIR resources/bundles from. Each message's value is parsed as a FHIR resource, pseudonymized, and published to the corresponding output topic. Reading from Kafka additionally requires at least one topic to be set here. Set either as an indexed list (`Kafka__Topics__0=topic-a`, `Kafka__Topics__1=topic-b`) or as a single comma-separated string (`Kafka__Topics=topic-a,topic-b`).                                                                                                                                                                                                                  | `[]`                |
| `Kafka__OutputTopicPattern`                       | A regular expression matched against the input topic's name. Used together with `Kafka__OutputTopicReplacement` to derive the output topic name via match-and-replace, e.g. matching `^fhir\.` and replacing it with `fhir.pseudonymized.` turns `fhir.test` into `fhir.pseudonymized.test`.                                                                                                                                                                                                                                                                                                                                             | `"^"`               |
| `Kafka__OutputTopicReplacement`                   | The replacement string substituted for every match of `Kafka__OutputTopicPattern`.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       | `"pseudonymized."`  |
| `Kafka__CryptoHashMessageKeys__Enabled`           | Whether the keys of the messages produced to the output topics are crypto-hashed instead of copied from the input messages, since they often are identifiers, e.g. a patient's id: each becomes the lowercase hex HMAC-SHA256 of the input message's key, keyed with `Kafka__CryptoHashMessageKeys__Key`. Messages without a key stay without one, and dead-lettered messages keep their original key. Set to `false` to keep the input messages' keys.                                                                                                                                                                                  | `true`              |
| `Kafka__CryptoHashMessageKeys__Key`               | The key used to crypto-hash message keys, at least 32 bytes long (e.g. from `openssl rand -hex 32`). Required unless `Kafka__CryptoHashMessageKeys__Enabled=false` - the service refuses to start without it. As long as it doesn't change, the same message key is always hashed to the same output key, by every replica and after restarts.                                                                                                                                                                                                                                                                                           | `""`                |
| `Kafka__WorkerCount`                              | The number of consumers anonymizing messages concurrently, all in the same consumer group. Each processes the messages of the partitions assigned to it one after another, so messages from the same partition are always processed in order, while different partitions are anonymized in parallel. More consumers than assigned partitions sit idle.                                                                                                                                                                                                                                                                                   | number of CPU cores |
| `Kafka__ProvenanceTopic`                          | The topic to publish a Provenance audit trail Bundle to for every successfully pseudonymized message (one Provenance per pseudonymized resource, targeting its post-pseudonymization `<type>/<id>`). Left unset, no provenance records are produced.                                                                                                                                                                                                                                                                                                                                                                                     | unset               |
| `Kafka__Client__*`                                | Settings shared by the consumer and producer, e.g. `Kafka__Client__BootstrapServers`, `Kafka__Client__SecurityProtocol`, `Kafka__Client__SaslMechanism`, `Kafka__Client__SaslUsername`, `Kafka__Client__SaslPassword`. Any property of [`Confluent.Kafka.ClientConfig`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ClientConfig.html) can be set this way.                                                                                                                                                                                                                      | unset               |
| `Kafka__Consumer__*`                              | Consumer-only settings layered on top of `Kafka__Client__*`, e.g. `Kafka__Consumer__GroupId`, `Kafka__Consumer__SessionTimeoutMs`. Any property of [`Confluent.Kafka.ConsumerConfig`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html) can be set this way; `GroupId` defaults to `"fhir-pseudonymizer"` if unset. Note that `EnableAutoOffsetStore` is always forced to `false` regardless of this setting, since offsets are only stored after a message has been anonymized and produced. `ClientId` and `GroupInstanceId` get the consumer's index appended. | unset               |
| `Kafka__Producer__*`                              | Producer-only settings layered on top of `Kafka__Client__*`, e.g. `Kafka__Producer__LingerMs`, `Kafka__Producer__CompressionType`. Any property of [`Confluent.Kafka.ProducerConfig`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html) can be set this way.                                                                                                                                                                                                                                                                                                      | unset               |
