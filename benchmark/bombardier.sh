#!/bin/bash

RESOURCE_PATH=${RESOURCE_PATH:-bundle.json}

bombardier -f "${RESOURCE_PATH}" \
    -H "Content-Type:application/fhir+json" \
    -m POST \
    -d 30s \
    "http://localhost:5000/fhir/\$de-identify"
