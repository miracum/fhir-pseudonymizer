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
[Try it in your browser :lucide-external-link:](https://miracum.github.io/fhir-pseudonymizer/playground/){ .md-button .fp-button-playground }

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

- :lucide-rocket:{ .lg .middle } **Production ready**

    ---

    Release images are signed with cosign and ship with SLSA Level 3 build provenance. They
    are built on a minimal, distroless base image and are executable following [NSA/CISA runtime hardening practices](https://media.defense.gov/2022/Aug/29/2003066362/1/1/0/CTR_KUBERNETES_HARDENING_GUIDANCE_1.2_20220829.PDF).
    Prometheus
    metrics and OpenTelemetry tracing are built in.

    [:octicons-arrow-right-24: Image verification](image-verification.md)

    [:octicons-arrow-right-24: Observability](observability.md)

- :lucide-gauge:{ .lg .middle } **Fast**

    ---

    Tens of thousands of requests per second on a single instance, with full horizontal scalability. Low cold-start latencies allow for automatic scale to 0.

    [:octicons-arrow-right-24: Performance](performance.md)

</div>

<!-- prettier-ignore-end -->

<p align="center" markdown><small>Logo contains elements from [twitter/twemoji](https://github.com/twitter/twemoji) licensed under [CC-BY 4.0](https://creativecommons.org/licenses/by/4.0/).</small></p>
