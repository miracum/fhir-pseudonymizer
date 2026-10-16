# Image verification and supply chain

All released container images are signed using [cosign](https://github.com/sigstore/cosign) and
SLSA Level 3 provenance is available for verification.

Prerequisites:

- [cosign](https://github.com/sigstore/cosign/releases)
- [GitHub CLI](https://cli.github.com/), logged in to any GitHub account (`gh auth login`) - it
  fetches the provenance from GitHub's attestations API
- [crane](https://github.com/google/go-containerregistry/releases)

<!-- x-release-please-start-version -->

```sh
IMAGE=ghcr.io/miracum/fhir-pseudonymizer:v3.1.0
DIGEST=$(crane digest "${IMAGE}")
IMAGE_DIGEST_PINNED="ghcr.io/miracum/fhir-pseudonymizer@${DIGEST}"
IMAGE_TAG="${IMAGE#*:}"

cosign verify \
   --certificate-oidc-issuer=https://token.actions.githubusercontent.com \
   --certificate-identity-regexp="https://github.com/miracum/.github/.github/workflows/standard-build.yaml@.*" \
   --certificate-github-workflow-name="ci" \
   --certificate-github-workflow-repository="miracum/fhir-pseudonymizer" \
   --certificate-github-workflow-trigger="release" \
   --certificate-github-workflow-ref="refs/tags/${IMAGE_TAG}" \
   "${IMAGE_DIGEST_PINNED}"

gh attestation verify "oci://${IMAGE_DIGEST_PINNED}" \
   --repo miracum/fhir-pseudonymizer \
   --signer-workflow miracum/.github/.github/workflows/standard-build.yaml \
   --source-ref "refs/tags/${IMAGE_TAG}"
```

<!-- x-release-please-end-version -->

The images are built by a reusable workflow in [miracum/.github](https://github.com/miracum/.github),
which is why `--signer-workflow` names that workflow rather than one in this repository.

The provenance is also pushed to the registry next to the image, so it can be enforced at deploy
time - see GitHub's guide to
[enforcing artifact attestations with a Kubernetes admission controller](https://docs.github.com/en/actions/how-tos/secure-your-work/use-artifact-attestations/enforce-artifact-attestations).

## Supply chain

### Selecting dependencies

New dependencies are chosen based on their license (must be compatible with this project's [MIT license](https://github.com/miracum/fhir-pseudonymizer/blob/master/LICENSE)), maintenance activity, and security track record. Every pull request that adds or changes a dependency is automatically checked by GitHub's [dependency-review-action](https://github.com/actions/dependency-review-action) for known vulnerabilities and disallowed licenses before it can be merged.

### Obtaining dependencies

- **.NET/NuGet packages** are declared as `PackageReference`s in the `.csproj` files and restored from [nuget.org](https://www.nuget.org/). Restores are deterministic: `RestorePackagesWithLockFile` is enabled, and the resulting `packages.lock.json` pins every (transitive) package to an exact version with integrity hashes, so `dotnet restore` always resolves the same dependency graph.
- **Container base images** are pinned by digest, in addition to tag, in the [Dockerfile](https://github.com/miracum/fhir-pseudonymizer/blob/master/Dockerfile), e.g. `mcr.microsoft.com/dotnet/aspnet:10.0.10-resolute-chiseled@sha256:...`.
- **GitHub Actions** used in CI are pinned to a specific commit SHA rather than a mutable tag or branch.

### Tracking and updating dependencies

- [Renovate](https://docs.renovatebot.com/) ([`.renovaterc.json`](https://github.com/miracum/fhir-pseudonymizer/blob/master/.renovaterc.json), extending [miracum's shared configuration](https://github.com/miracum/.github/blob/master/renovate/default.json)) continuously monitors and opens pull requests to update NuGet packages, container base images, GitHub Actions, and Kubernetes manifests.
- Every pull request runs [CodeQL](https://codeql.github.com/) static analysis and the dependency-review check described above.
- Released container images are scanned for known vulnerabilities with [Trivy](https://github.com/aquasecurity/trivy); results are published as a signed [cosign](https://github.com/sigstore/cosign) attestation, and a [daily scheduled workflow](https://github.com/miracum/fhir-pseudonymizer/blob/master/.github/workflows/schedule.yaml) re-scans the latest published images so newly disclosed CVEs are caught even between releases.
- Every build generates a Software Bill of Materials (SBOM) in CycloneDX and SPDX format, which is attached as a downloadable asset to each [GitHub release](https://github.com/miracum/fhir-pseudonymizer/releases).
- [OpenSSF Scorecard](https://github.com/ossf/scorecard) analysis runs on a schedule and on every push to `master`, continuously assessing the repository's supply-chain security posture, including dependency-related checks. See the badge on the [README](https://github.com/miracum/fhir-pseudonymizer#readme) for the current score.
