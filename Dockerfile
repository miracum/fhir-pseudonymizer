FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0.8-jammy-chiseled@sha256:5258a1139db036d151e49934406c50fc41604b9519441294e017d75347f932e6 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim-amd64@sha256:8539410b792e01480351d4161fa6b29211f560014f10cb9f8a1ea3f7d08d812d AS build
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
    -l "console;verbosity=detailed"

FROM runtime
COPY --from=build /build/publish/*anonymization.yaml /etc
COPY --from=build /build/publish .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
