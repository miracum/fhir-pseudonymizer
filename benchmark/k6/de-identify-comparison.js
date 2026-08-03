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
const LEGACY_TOTAL_SECONDS =
  RAMP_UP_SECONDS + DURATION_SECONDS + RAMP_DOWN_SECONDS;
const V3ALPHA1_START_SECONDS = LEGACY_TOTAL_SECONDS + GAP_SECONDS;

// Same fixture used by the bombardier-based benchmark in ../bombardier.sh, so both
// benchmarks exercise the same input.
const observationJson = open("../observation.json");
const observationResource = JSON.parse(observationJson);

// The default rules the server itself is expected to be running with (see the "Run" section
// in ../../README.md) - reused here as the inline config attachment for v3alpha1 so both
// endpoints apply the exact same de-identification rules. That isolates the comparison to the
// cost of the two endpoints' request handling rather than differences in the rules applied.
const anonymizationYaml = open(
  "../../src/FhirPseudonymizer/anonymization.yaml",
);
const configBase64 = encoding.b64encode(anonymizationYaml);

const legacyBody = observationJson;
const v3Alpha1Body = JSON.stringify({
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

const legacyDuration = new Trend("legacy_de_identify_duration", true);
const legacyRequests = new Counter("legacy_de_identify_requests");
const legacyErrors = new Rate("legacy_de_identify_errors");

const v3Alpha1Duration = new Trend("v3alpha1_de_identify_duration", true);
const v3Alpha1Requests = new Counter("v3alpha1_de_identify_requests");
const v3Alpha1Errors = new Rate("v3alpha1_de_identify_errors");

export const options = {
  scenarios: {
    // /fhir/$de-identify: the existing endpoint, using the server's statically configured
    // anonymization rules (AnonymizationEngineConfigPath/Inline).
    legacy_de_identify: {
      executor: "ramping-vus",
      exec: "legacyDeIdentify",
      startVUs: 0,
      stages: [
        { duration: `${RAMP_UP_SECONDS}s`, target: VUS },
        { duration: `${DURATION_SECONDS}s`, target: VUS },
        { duration: `${RAMP_DOWN_SECONDS}s`, target: 0 },
      ],
      startTime: "0s",
      tags: { endpoint: "legacy" },
    },
    // /v3alpha1/fhir/$de-identify: rules are sent per-request as a base64-encoded YAML
    // Attachment alongside the resource, both wrapped in a Parameters resource.
    v3alpha1_de_identify: {
      executor: "ramping-vus",
      exec: "v3Alpha1DeIdentify",
      startVUs: 0,
      stages: [
        { duration: `${RAMP_UP_SECONDS}s`, target: VUS },
        { duration: `${DURATION_SECONDS}s`, target: VUS },
        { duration: `${RAMP_DOWN_SECONDS}s`, target: 0 },
      ],
      // Starts only once the legacy scenario has fully ramped down, so the two scenarios
      // never compete for the server's resources at the same time.
      startTime: `${V3ALPHA1_START_SECONDS}s`,
      tags: { endpoint: "v3alpha1" },
    },
  },
  thresholds: {
    legacy_de_identify_errors: ["rate<0.01"],
    v3alpha1_de_identify_errors: ["rate<0.01"],
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

export function legacyDeIdentify() {
  const res = http.post(`${BASE_URL}/fhir/$de-identify`, legacyBody, {
    headers: fhirJsonHeaders,
    tags: { endpoint: "legacy" },
  });

  legacyDuration.add(res.timings.duration);
  legacyRequests.add(1);
  legacyErrors.add(res.status !== 200);

  check(res, { "legacy: status is 200": (r) => r.status === 200 });
}

export function v3Alpha1DeIdentify() {
  const res = http.post(`${BASE_URL}/v3alpha1/fhir/$de-identify`, v3Alpha1Body, {
    headers: fhirJsonHeaders,
    tags: { endpoint: "v3alpha1" },
  });

  v3Alpha1Duration.add(res.timings.duration);
  v3Alpha1Requests.add(1);
  v3Alpha1Errors.add(res.status !== 200);

  check(res, { "v3alpha1: status is 200": (r) => r.status === 200 });
}
