FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0.0-jammy-chiseled@sha256:c1350bc839492415f55f4dc6549533e4bad68ecdaf9976497a235eee18f89993 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM mcr.microsoft.com/dotnet/sdk:7.0.100-bullseye-slim-amd64@sha256:680f4c6544dde8796b7669aa8c080e739d995854e5dad2b6061f65e31a4c7fe8 AS build
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /build
COPY src/FhirPseudonymizer/FhirPseudonymizer.csproj .
RUN dotnet restore
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
COPY --from=build /build/publish/*anonymization.yaml /etc
COPY --from=build /build/publish .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
