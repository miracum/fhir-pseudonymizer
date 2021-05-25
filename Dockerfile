FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /build
COPY src/FhirPseudonymizer/FhirPseudonymizer.csproj .
RUN dotnet restore
COPY . .

ARG VERSION=0.0.0
RUN dotnet publish \
    -c Release \
    -p:Version=${VERSION} \
    -o /build/publish \
    src/FhirPseudonymizer/FhirPseudonymizer.csproj

FROM build AS test
WORKDIR /build/src/FhirPseudonymizer.Tests
RUN dotnet test -p:CollectCoverage=true

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
ENV ASPNETCORE_ENVIRONMENT="Production" \
    ASPNETCORE_URLS="http://*:8080"
USER 65532
WORKDIR /opt/fhir-pseudonymizer

COPY --from=build /build/publish/*anonymization.yaml /etc
COPY --from=build /build/publish .
ENTRYPOINT ["dotnet", "/opt/fhir-pseudonymizer/FhirPseudonymizer.dll"]
