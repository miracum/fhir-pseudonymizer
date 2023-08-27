# Deploy the FHIR Pseudonymizer using Compose

This uses an example anonymization config based on the [HIPAA Safe Harbor rules](anonymization-hipaa.yaml):

```sh
docker compose up
# or
nerdctl compose up
# or
podman-compose up
```

Open your browser at <http://localhost:8080/swagger>. Or simply POST any FHIR resource to <http://localhost:8080/fhir/$de-identify>.
