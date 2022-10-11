#!/bin/bash

RESOURCE_PATH=${RESOURCE_PATH:-bundle.json}

bombardier -f "${RESOURCE_PATH}" \
  --timeout=10s \
  -H "Content-Type:application/fhir+json" \
  -m POST \
  -d 60s \
  -l \
  "http://localhost:5000/fhir/\$de-identify"
