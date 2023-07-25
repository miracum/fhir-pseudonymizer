FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0.9-jammy-chiseled@sha256:d612147bddd8753f0da14e073e6d3d567e0138e38e622250c60e565046632b77 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM mcr.microsoft.com/dotnet/sdk:7.0.306-bullseye-slim-amd64@sha256:3a024733ce4a9df932472c7c72915695976782d05bef7571cc5ee32bfa9d46ab AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /build
COPY src/FhirPseudonymizer/FhirPseudonymizer.csproj .
COPY src/FhirPseudonymizer/packages.lock.json .
RUN dotnet restore --locked-mode
COPY . .

ARG VERSION=0.0.0
RUN dotnet publish \
    -c Release \
    -p:Version=${VERSION} \
    -p:UseAppHost=false \
    -o /build/publish \
    src/FhirPseudonymizer/FhirPseudonymizer.csproj

FROM build AS test
WORKDIR /build/src/FhirPseudonymizer.Tests
RUN dotnet test \
    --configuration=Release \
    --collect:"XPlat Code Coverage" \
    --results-directory=./coverage \
    -l "console;verbosity=detailed" \
    --settings=runsettings.xml

FROM runtime
COPY --from=build /build/publish/*anonymization.yaml /etc/
COPY --from=build /build/publish .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
