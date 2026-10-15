# kics false positive "Missing User Instruction": <https://docs.kics.io/latest/queries/dockerfile-queries/fd54f200-402c-4333-a5a4-36ef6709af2f/>
# kics-scan ignore-line
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0.12-resolute-chiseled@sha256:65121360bba4839ade70bea222d6ea6485eab090bbe6e8290cb04c0c7d913a39 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.401-resolute@sha256:d8051d88928fb40f74c9aca6896a57851cdced277f4102e1c5aa6d4983e27bad AS build
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
