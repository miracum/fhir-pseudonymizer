FROM mcr.microsoft.com/dotnet/nightly/aspnet:7.0.5-jammy-chiseled@sha256:130398f80260b94f5376b78ecd191cfcd3c1718ad1205d1944c7e83eea6799e3 AS runtime
WORKDIR /opt/fhir-pseudonymizer
EXPOSE 8080/tcp 8081/tcp
USER 65532:65532
ENV ASPNETCORE_ENVIRONMENT="Production" \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    ASPNETCORE_URLS="http://*:8080"

FROM mcr.microsoft.com/dotnet/sdk:7.0.203-bullseye-slim-amd64@sha256:ac056155c01f3c3e16a46e7d8880860947b210c64d74a6d2c60fab3b439beeb9 AS build
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
COPY --from=build /build/publish/*anonymization.yaml /etc/
COPY --from=build /build/publish .

ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
