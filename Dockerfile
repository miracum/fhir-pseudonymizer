FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0.1-jammy-chiseled@sha256:05dd233adae9aaa218df1ac188fa6be415482eb0ee0f826c4c840ed64d473efd AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM mcr.microsoft.com/dotnet/sdk:7.0.101-bullseye-slim-amd64@sha256:da34b595be986af8c183209c405d8c5d1ebecb11ae1aceef6a6297896ab71ba3 AS build
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
