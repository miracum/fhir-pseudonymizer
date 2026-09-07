import http from "k6/http";
import { check } from "k6";
import { Trend, Rate, Counter } from "k6/metrics";
import encoding from "k6/encoding";

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";
const VUS = Number(__ENV.VUS || 20);
const DURATION_SECONDS = Number(__ENV.DURATION_SECONDS || 30);

const RAMP_UP_SECONDS = 5;
const RAMP_DOWN_SECONDS = 2;
const GAP_SECONDS = 3;
const STATIC_CONFIG_TOTAL_SECONDS =
  RAMP_UP_SECONDS + DURATION_SECONDS + RAMP_DOWN_SECONDS;
const DYNAMIC_CONFIG_START_SECONDS = STATIC_CONFIG_TOTAL_SECONDS + GAP_SECONDS;

// Same fixture used by the bombardier-based benchmark in ../bombardier.sh, so both
// benchmarks exercise the same input.
const observationJson = open("../observation.json");
const observationResource = JSON.parse(observationJson);

// The default rules the server itself is expected to be running with (see the "Run" section
// in ../../README.md) - reused here as the inline config attachment for the dynamic-config
// scenario so both scenarios apply the exact same de-identification rules. That isolates the
// comparison to the cost of building/caching a per-request rule set rather than differences in
// the rules applied.
const anonymizationYaml = open(
  "../../src/FhirPseudonymizer/anonymization.yaml",
);
const configBase64 = encoding.b64encode(anonymizationYaml);

const staticConfigBody = observationJson;
const dynamicConfigBody = JSON.stringify({
  resourceType: "Parameters",
  parameter: [
    {
      name: "config",
      valueAttachment: {
        contentType: "application/yaml",
        data: configBase64,
      },
    },
    { name: "resource", resource: observationResource },
  ],
});

const fhirJsonHeaders = { "Content-Type": "application/fhir+json" };

const staticConfigDuration = new Trend(
  "static_config_de_identify_duration",
  true,
);
const staticConfigRequests = new Counter("static_config_de_identify_requests");
const staticConfigErrors = new Rate("static_config_de_identify_errors");

const dynamicConfigDuration = new Trend(
  "dynamic_config_de_identify_duration",
  true,
);
const dynamicConfigRequests = new Counter(
  "dynamic_config_de_identify_requests",
);
const dynamicConfigErrors = new Rate("dynamic_config_de_identify_errors");

export const options = {
  scenarios: {
    // /fhir/$de-identify with a plain resource body: the server's statically configured
    // anonymization rules (AnonymizationEngineConfigPath/Inline).
    static_config_de_identify: {
      executor: "ramping-vus",
      exec: "staticConfigDeIdentify",
      startVUs: 0,
      stages: [
        { duration: `${RAMP_UP_SECONDS}s`, target: VUS },
        { duration: `${DURATION_SECONDS}s`, target: VUS },
        { duration: `${RAMP_DOWN_SECONDS}s`, target: 0 },
      ],
      startTime: "0s",
      tags: { endpoint: "static_config" },
    },
    // /fhir/$de-identify with a Parameters body carrying a config part: rules are sent
    // per-request as a base64-encoded YAML Attachment alongside the resource.
    dynamic_config_de_identify: {
      executor: "ramping-vus",
      exec: "dynamicConfigDeIdentify",
      startVUs: 0,
      stages: [
        { duration: `${RAMP_UP_SECONDS}s`, target: VUS },
        { duration: `${DURATION_SECONDS}s`, target: VUS },
        { duration: `${RAMP_DOWN_SECONDS}s`, target: 0 },
      ],
      // Starts only once the static-config scenario has fully ramped down, so the two
      // scenarios never compete for the server's resources at the same time.
      startTime: `${DYNAMIC_CONFIG_START_SECONDS}s`,
      tags: { endpoint: "dynamic_config" },
    },
  },
  thresholds: {
    static_config_de_identify_errors: ["rate<0.01"],
    dynamic_config_de_identify_errors: ["rate<0.01"],
  },
};

export function setup() {
  const res = http.get(`${BASE_URL}/fhir/metadata`);
  if (res.status !== 200) {
    throw new Error(
      `Server at ${BASE_URL} is not ready (GET /fhir/metadata returned ${res.status}). Is it running?`,
    );
  }
}

export function staticConfigDeIdentify() {
  const res = http.post(`${BASE_URL}/fhir/$de-identify`, staticConfigBody, {
    headers: fhirJsonHeaders,
    tags: { endpoint: "static_config" },
  });

  staticConfigDuration.add(res.timings.duration);
  staticConfigRequests.add(1);
  staticConfigErrors.add(res.status !== 200);

  check(res, { "static config: status is 200": (r) => r.status === 200 });
}

export function dynamicConfigDeIdentify() {
  const res = http.post(`${BASE_URL}/fhir/$de-identify`, dynamicConfigBody, {
    headers: fhirJsonHeaders,
    tags: { endpoint: "dynamic_config" },
  });

  dynamicConfigDuration.add(res.timings.duration);
  dynamicConfigRequests.add(1);
  dynamicConfigErrors.add(res.status !== 200);

  check(res, { "dynamic config: status is 200": (r) => r.status === 200 });
}
