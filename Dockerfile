# kics false positive "Missing User Instruction": <https://docs.kics.io/latest/queries/dockerfile-queries/fd54f200-402c-4333-a5a4-36ef6709af2f/>
# kics-scan ignore-line
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:9.0.7-noble-chiseled@sha256:519d28b1f35e7060f5ae53d0e43e9522b59522dab51ecef6465af60b94b3ad92 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0.303-noble@sha256:af75742bd2f6fd148504951b57a983bc410191729bf543eae467528d1bc175e5 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /build
COPY src/Directory.Build.props .
COPY src/FhirPseudonymizer/FhirPseudonymizer.csproj .
COPY src/FhirPseudonymizer/packages.lock.json .
RUN dotnet restore --locked-mode
COPY . .

RUN dotnet publish \
    -c Release \
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
WORKDIR /build

RUN <<EOF
dotnet publish \
    --configuration=Release \
    -a "$TARGETARCH" \
    -o /build/publish \
    src/FhirPseudonymizer.StressTests/FhirPseudonymizer.StressTests.csproj
EOF

FROM build AS stress-test
WORKDIR /opt/fhir-pseudonymizer-stress

# https://github.com/hadolint/hadolint/pull/815 isn't yet in mega-linter
# hadolint ignore=DL3022
COPY --from=docker.io/rancher/kubectl:v1.34.0@sha256:ff3cdadeac7eae628b59debe73302bb41337098dc3f15dfb3f3c5a59b046d23c /bin/kubectl /usr/bin/kubectl

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
