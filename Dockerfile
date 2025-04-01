# kics false positive "Missing User Instruction": <https://docs.kics.io/latest/queries/dockerfile-queries/fd54f200-402c-4333-a5a4-36ef6709af2f/>
# kics-scan ignore-line
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0.3-noble-chiseled@sha256:dba2cfa6a751eac6ecc5371c2f7f08cad711edbf465afe8180c60be2853e7a5b AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0.202-noble@sha256:dcc11e3a07f72d463077b401275d09fbad72b68cdc7a2029c2b51f21812bd8f6 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /build
COPY src/Directory.Build.props .
COPY src/FhirPseudonymizer/FhirPseudonymizer.csproj .
COPY src/FhirPseudonymizer/packages.lock.json .
RUN dotnet restore --locked-mode
COPY . .

ARG VERSION=2.22.8
RUN dotnet publish \
    -c Release \
    -p:Version=${VERSION} \
    -o /build/publish \
    -a "$TARGETARCH" \
    src/FhirPseudonymizer/FhirPseudonymizer.csproj

FROM build AS build-test
WORKDIR /build/src/FhirPseudonymizer.Tests
RUN dotnet test \
    --configuration=Release \
    --collect:"XPlat Code Coverage" \
    --results-directory=./coverage \
    -l "console;verbosity=detailed" \
    --settings=runsettings.xml

FROM scratch AS test
WORKDIR /build/src/FhirPseudonymizer.Tests/coverage
COPY --from=build-test /build/src/FhirPseudonymizer.Tests/coverage .
ENTRYPOINT [ "true" ]

FROM build AS build-stress-test
WORKDIR /build/src/FhirPseudonymizer.StressTests
RUN <<EOF
dotnet build \
    --configuration=Release

dotnet publish \
    --no-restore \
    --no-build \
    --configuration=Release \
    -a "$TARGETARCH" \
    -o /build/publish
EOF

FROM build AS stress-test
WORKDIR /opt/fhir-pseudonymizer-stress

# https://github.com/hadolint/hadolint/pull/815 isn't yet in mega-linter
# hadolint ignore=DL3022
COPY --from=docker.io/bitnami/kubectl:1.32.3@sha256:3f21647d4c3f0fa0a906d6aacf7919beac2356bf6a2fb595414bea045ba75270 /opt/bitnami/kubectl/bin/kubectl /usr/bin/kubectl

COPY tests/chaos/chaos.yaml /tmp/
COPY --from=build-stress-test /build/publish .
# currently running into <https://github.com/dotnet/runtime/issues/80619>
# when running as non-root.
# hadolint ignore=DL3002
USER 0:0
ENTRYPOINT ["dotnet"]
CMD ["test", "/opt/fhir-pseudonymizer-stress/FhirPseudonymizer.StressTests.dll", "-l", "console;verbosity=detailed"]

FROM runtime
COPY LICENSE .
COPY --from=build /build/publish/*anonymization.yaml /etc/
COPY --from=build /build/publish .
COPY --from=build /build/packages.lock.json .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
