import http from "k6/http";
import { check, sleep } from "k6";
import crypto from "k6/crypto";
import { Rate } from "k6/metrics";

// Sustained load while tests/chaos/chaos.yaml periodically kills a fhir-pseudonymizer/vfps pod
// (see tests/chaos/workflow.yaml). The invariant under test: pseudonymization must keep working
// correctly throughout - HTTP-level hiccups from an in-flight pod restart are tolerated up to the
// http_req_failed threshold below, but a wrong/missing pseudonym never is.

const BASE_URL = __ENV.BASE_URL || "http://localhost:8080";

// Set to true only when a 200 response's pseudonym doesn't match what Vfps's "stress" namespace
// (Sha256HexEncoded, see ../../tests/chaos/fhir-pseudonymizer-values.yaml) should have produced -
// as opposed to an outright HTTP failure, which http_req_failed already accounts for below.
const invariantViolations = new Rate("invariant_violations");

export const options = {
  stages: [
    { duration: "5m", target: 10 },
    { duration: "10m", target: 100 },
  ],
  thresholds: {
    http_req_failed: ["rate<0.001"],
    invariant_violations: ["rate<=0"],
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

export default function () {
  const originalRecordNumber = String(Math.random());
  const expectedPseudonym = `stress-${crypto.sha256(originalRecordNumber, "hex").toUpperCase()}`;

  const resource = {
    resourceType: "Patient",
    id: String(Math.random()),
    active: true,
    name: [{ family: "Doe", given: ["John"] }],
    identifier: [
      {
        system: "https://fhir.example.com/identifiers/mrn",
        value: originalRecordNumber,
        type: {
          coding: [
            {
              system: "http://terminology.hl7.org/CodeSystem/v2-0203",
              code: "MR",
            },
          ],
        },
      },
    ],
  };

  const parameters = {
    resourceType: "Parameters",
    parameter: [{ name: "resource", resource }],
  };

  const res = http.post(`${BASE_URL}/fhir/$de-identify`, JSON.stringify(parameters), {
    headers: { "Content-Type": "application/fhir+json" },
    timeout: "15s",
  });

  if (
    !check(res, {
      "status is 200": (r) => r.status === 200,
    })
  ) {
    // an HTTP-level failure, not a pseudonymization correctness bug - already reflected in
    // http_req_failed, so it doesn't count against invariant_violations.
    sleep(1);
    return;
  }

  const identifiers = res.json("identifier") || [];
  const pseudonymized =
    identifiers.length === 1 && identifiers[0].value === expectedPseudonym;

  check(res, { "exactly one correctly pseudonymized identifier": () => pseudonymized });
  invariantViolations.add(!pseudonymized);

  sleep(1);
}
