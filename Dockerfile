# kics false positive "Missing User Instruction": <https://docs.kics.io/latest/queries/dockerfile-queries/fd54f200-402c-4333-a5a4-36ef6709af2f/>
# kics-scan ignore-line
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0.14-resolute-chiseled@sha256:9fd8f06517e807d3cf404f107ccca7a0d46a2d288aa088f41a7e532948ab43ad AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.403-resolute@sha256:d32b07e1e85ea9737338e49451b2804f861e18e8507feaf3ef7b20a1568569c2 AS build
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
    --results-directory=./coverage \
    --coverage \
    --coverage-output-format cobertura \
    --coverage-output coverage.cobertura.xml \
    --coverage-settings codecoverage.config

FROM scratch AS test
WORKDIR /build/src/FhirPseudonymizer.Tests/coverage
COPY --from=build-test /build/src/FhirPseudonymizer.Tests/coverage .
ENTRYPOINT [ "true" ]

FROM runtime
COPY LICENSE .
COPY --from=build /build/publish .
COPY --from=build /build/packages.lock.json .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
