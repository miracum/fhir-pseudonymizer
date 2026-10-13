---
title: FHIR Pseudonymizer
description: A REST and Kafka service for de-identifying FHIR resources.
hide:
  - navigation
  - toc
---

<div class="fp-hero" markdown>

<div class="fp-brand">
  <img src="img/logo.svg" alt="" />
  <h1 class="fp-title"><span class="fp-title-accent">FHIR</span> <span>Pseudonymizer</span></h1>
</div>

<p class="fp-tagline">A REST and Kafka service for de-identifying FHIR® resources.</p>

[Get started](getting-started.md){ .md-button .md-button--primary }
[Try it in your browser](https://miracum.github.io/fhir-pseudonymizer/playground/){ .md-button }

</div>

<!-- prettier-ignore-start -->

<div class="grid cards" markdown>

- :lucide-shield-check:{ .lg .middle } **Rule-based de-identification**

    ---

    Describe what should happen to each element with FHIRPath rules: redact,
    generalize, shift dates, hash, encrypt, remove, or replace with a pseudonym.

    [:octicons-arrow-right-24: De-identification](de-identify.md)

- :lucide-key-round:{ .lg .middle } **Pseudonymization backends**

    ---

    Create and reverse pseudonyms with gPAS, Vfps, entici or any service implementing the MII
    pseudonymization interface.

    [:octicons-arrow-right-24: Pseudonymization services](pseudonymization/vfps.md)

- :lucide-sliders-horizontal:{ .lg .middle } **Per-request settings**

    ---

    Serve many projects from one deployment: override rule settings, or send an entire
    anonymization config along with the request.

    [:octicons-arrow-right-24: Dynamic rules and config](dynamic-config.md)

- :lucide-workflow:{ .lg .middle } **Streaming from Kafka**

    ---

    Consume FHIR resources straight from Kafka topics and publish the result, with dead-lettering
    and at-least-once delivery.

    [:octicons-arrow-right-24: Reading from Kafka](kafka.md)

- :lucide-scroll-text:{ .lg .middle } **Audit trail**

    ---

    Every pseudonymized resource can produce an idempotent FHIR Provenance record naming what it
    was derived from.

    [:octicons-arrow-right-24: Provenance](provenance.md)

- :lucide-gauge:{ .lg .middle } **Fast and observable**

    ---

    Tens of thousands of requests per second on a single instance, exposed Prometheus metrics,
    OpenTelemetry tracing support, and a
    ready-to-import Grafana dashboard.

    [:octicons-arrow-right-24: Performance](performance.md)

</div>

<!-- prettier-ignore-end -->

<p align="center" markdown><small>Logo contains elements from [twitter/twemoji](https://github.com/twitter/twemoji) licensed under [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/).</small></p>
