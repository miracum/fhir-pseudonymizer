using System.Net;
using System.Net.Http.Headers;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Tests;

public class ProjectsTests(CustomWebApplicationFactory<Startup> factory)
    : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    /// <summary>
    ///     Redacts what the startup config encrypts, so applying it is observable in the response.
    /// </summary>
    private const string RedactMedicalRecordNumberConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('Identifier').value
            method: redact
        """;

    /// <summary>
    ///     The startup config encrypts this identifier's value; a redacting Project blanks it.
    /// </summary>
    private const string PatientWithMedicalRecordNumber = """
        {
            "resourceType": "Patient",
            "id": "glossy",
            "identifier": [
                {
                    "use": "usual",
                    "type": {
                        "coding": [
                            {
                                "system": "http://terminology.hl7.org/CodeSystem/v2-0203",
                                "code": "MR"
                            }
                        ]
                    },
                    "system": "http://www.goodhealth.org/identifiers/mrn",
                    "value": "123456"
                }
            ]
        }
        """;

    /// <summary>
    ///     Observably different from <see cref="RedactMedicalRecordNumberConfig" />: it leaves the
    ///     identifier value untouched instead of blanking it.
    /// </summary>
    private const string KeepMedicalRecordNumberConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('Identifier').value
            method: keep
        """;

    private readonly HttpClient client = factory.CreateClient();

    [Fact]
    public async Task PutProject_WithValidConfig_ShouldReturnCreated()
    {
        var response = await RegisterProject("slice2-demo", RedactMedicalRecordNumberConfig);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostDeIdentify_WithRegisteredProjectName_ShouldApplyThatProjectsRules()
    {
        await RegisterProject("slice3-redact", RedactMedicalRecordNumberConfig);

        var response = await DeIdentify(PatientWithMedicalRecordNumber, "slice3-redact");

        response.EnsureSuccessStatusCode();
        var patient = await ParsePatient(response);
        patient.Identifier[0].Value.Should().BeNull();
    }

    [Fact]
    public async Task PostDeIdentify_WithUnregisteredProjectName_ShouldReturnNotFoundOutcome()
    {
        var response = await DeIdentify(PatientWithMedicalRecordNumber, "slice4-never-registered");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Diagnostics.Should()
            .Contain("slice4-never-registered");
    }

    [Theory]
    [InlineData("", "empty")]
    [InlineData("fhirPathRules: [unclosed", "malformed-yaml")]
    [InlineData("fhirVersion: R4", "no-rules")]
    [InlineData(
        "fhirVersion: R4\nfhirPathRules:\n  - path: Patient.id\n    method: nonsense",
        "unknown-method"
    )]
    [InlineData("fhirVersion: R4\nfhirPathRules: [null]", "null-rule")]
    [InlineData(
        "fhirVersion: R4\nfhirPathRules:\n  - path: Patient.id\n    method: redact\n  -",
        "trailing-bare-dash"
    )]
    public async Task PutProject_WithConfigThatCannotBuildEngines_ShouldReturnBadRequestOutcome(
        string yamlConfig,
        string caseName
    )
    {
        var response = await RegisterProject($"slice6-{caseName}", yamlConfig);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Severity.Should()
            .Be(OperationOutcome.IssueSeverity.Error);
    }

    [Theory]
    [InlineData("encrypt", "encryptKey")]
    [InlineData("cryptoHash", "cryptoHashKey")]
    [InlineData("dateShift", "dateShiftKey")]
    public async Task PutProject_WithRuleWhoseKeyTheConfigDoesNotCarry_ShouldReturnBadRequestOutcome(
        string method,
        string missingParameter
    )
    {
        var yamlConfig = $"""
            fhirVersion: R4
            fhirPathRules:
              - path: nodesByType('Identifier').value
                method: {method}
            """;

        var response = await RegisterProject($"slice7-{method}", yamlConfig);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome.Issue.Should().ContainSingle().Which.Diagnostics.Should().Contain(missingParameter);
    }

    /// <summary>
    ///     Serving any of these with the server's own config would answer 200 to a caller who
    ///     asked for a Project's rules, who would never learn that different ones ran.
    /// </summary>
    [Theory]
    [InlineData("code", "a valueCode is not the valueString the parameter is read as")]
    [InlineData("uri", "a valueUri is not the valueString the parameter is read as")]
    [InlineData("empty-string", "an empty name selects nothing")]
    [InlineData("unregistrable", "a name registration would reject can never resolve")]
    public async Task PostDeIdentify_WithAProjectParameterCarryingNoUsableName_ShouldReturnBadRequestOutcome(
        string shape,
        string because
    )
    {
        DataType projectValue = shape switch
        {
            "code" => new Code("slice13-code"),
            "uri" => new FhirUri("slice13-uri"),
            "empty-string" => new FhirString(""),
            "unregistrable" => new FhirString("slice13 has a space"),
            _ => null,
        };

        var response = await DeIdentify(PatientWithMedicalRecordNumber, projectValue);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Severity.Should()
            .Be(OperationOutcome.IssueSeverity.Error);
    }

    [Fact]
    public async Task PostDeIdentify_WithAValuelessProjectParameter_ShouldReturnBadRequestOutcome()
    {
        // Sent as raw JSON on purpose: Parameters.Add drops a null value instead of adding the
        // component, so only the wire form can carry a 'project' that names nothing.
        using var content = new StringContent(
            """
            {
                "resourceType": "Parameters",
                "parameter": [
                    { "name": "project" },
                    { "name": "resource", "resource": { "resourceType": "Patient" } }
                ]
            }
            """
        );
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostDeIdentify_WithMoreThanOneProjectParameter_ShouldReturnBadRequestOutcome()
    {
        // Not the 404's re-register-and-retry: no registration could tell the server which of
        // the two names the caller meant, so only a corrected request can resolve this.
        var resource = await new FhirJsonParser().ParseAsync<Resource>(
            PatientWithMedicalRecordNumber
        );
        var parameters = new Parameters()
            .Add("project", new FhirString("slice14-first"))
            .Add("project", new FhirString("slice14-second"))
            .Add("resource", resource);

        using var content = new StringContent(await parameters.ToJsonAsync());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Severity.Should()
            .Be(OperationOutcome.IssueSeverity.Error);
    }

    [Fact]
    public async Task PostDeIdentify_WithParametersNamingNoProject_ShouldApplyTheServersOwnConfig()
    {
        // The promise that Projects are purely additive: the selector is optional, and a body
        // that leaves it out has to come back exactly as it did before Projects existed.
        var parameters = new Parameters().Add(
            "resource",
            await new FhirJsonParser().ParseAsync<Resource>(PatientWithMedicalRecordNumber)
        );

        using var content = new StringContent(await parameters.ToJsonAsync());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        var response = await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );

        response.EnsureSuccessStatusCode();
        var value = (await ParsePatient(response)).Identifier[0].Value;
        value
            .Should()
            .NotBeNull("the server's config encrypts the identifier rather than redacting it")
            .And.NotBe("123456", "and it does not keep it either");
    }

    [Fact]
    public async Task DeleteProject_ShouldMakeTheProjectUnknownAgain()
    {
        await RegisterProject("slice8-delete", RedactMedicalRecordNumberConfig);

        var deleteResponse = await DeleteProject("slice8-delete");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deIdentifyResponse = await DeIdentify(PatientWithMedicalRecordNumber, "slice8-delete");
        deIdentifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PutProject_OverAnExistingProject_ShouldReturnOkAndServeTheNewConfig()
    {
        var firstResponse = await RegisterProject(
            "slice9-overwrite",
            RedactMedicalRecordNumberConfig
        );
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await RegisterProject(
            "slice9-overwrite",
            KeepMedicalRecordNumberConfig
        );

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var patient = await ParsePatient(
            await DeIdentify(PatientWithMedicalRecordNumber, "slice9-overwrite")
        );
        patient.Identifier[0].Value.Should().Be("123456");
    }

    [Theory]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    public async Task ProjectRegistration_WithApiKeyConfiguredButNotSupplied_ShouldReturnUnauthorized(
        string method
    )
    {
        using var request = new HttpRequestMessage(
            new HttpMethod(method),
            "/projects/slice11-unauthorized"
        )
        {
            Content = new StringContent(RedactMedicalRecordNumberConfig),
        };
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/yaml");

        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PutProject_WithBlankApiKey_ShouldBeAllowedWithoutCredentials()
    {
        using var openFactory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["ApiKey"] = "",
                ["EnableMetrics"] = "false",
            },
        };

        using var content = new StringContent(RedactMedicalRecordNumberConfig);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/yaml");

        var response = await openFactory
            .CreateClient()
            .PutAsync("/projects/slice11-open", content, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PutProject_WhenTheRegistryIsFull_ShouldReturnServiceUnavailableOutcome()
    {
        using var boundedFactory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["ProjectCache:SizeLimit"] = "1",
                ["EnableMetrics"] = "false",
            },
        };
        var boundedClient = boundedFactory.CreateClient();

        var first = await RegisterProject(
            "slice12-first",
            RedactMedicalRecordNumberConfig,
            boundedClient
        );
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await RegisterProject(
            "slice12-second",
            RedactMedicalRecordNumberConfig,
            boundedClient
        );

        second.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);

        var outcome = await ParseResource<OperationOutcome>(second);
        outcome.Issue.Should().ContainSingle().Which.Diagnostics.Should().Contain("retry");
    }

    [Fact]
    public void Startup_WithAZeroSizedProjectRegistry_ShouldFailFastNamingTheSetting()
    {
        using var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["ProjectCache:SizeLimit"] = "0",
                ["EnableMetrics"] = "false",
            },
        };

        var start = () => factory.CreateClient();

        start
            .Should()
            .Throw<InvalidOperationException>(
                "a zero-sized registry answers every registration with a 503 promising a retry "
                    + "that can never succeed, and it fails $de-identify too, so an operator has "
                    + "to learn about it at boot rather than from the first request"
            )
            .WithMessage("*ProjectCache:SizeLimit*");
    }

    /// <summary>
    ///     65 'a's - one past the limit.
    /// </summary>
    private const string TooLongProjectName =
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Theory]
    [InlineData("forges%23sha256-deadbeef", "an encoded '#' is not in the allowed set")]
    [InlineData("has%20a%20space", "spaces")]
    [InlineData("pröject", "non-ascii")]
    [InlineData(TooLongProjectName, "65 characters")]
    [InlineData(
        "trailing-newline%0A",
        "a '$' anchor matches before a trailing newline, which '\\z' rejects"
    )]
    public async Task PutProject_WithANameOutsideTheAllowedCharacterSet_ShouldReturnBadRequestOutcome(
        string name,
        string because
    )
    {
        var response = await RegisterProject(name, RedactMedicalRecordNumberConfig);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, because);

        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Severity.Should()
            .Be(OperationOutcome.IssueSeverity.Error);
    }

    private async Task<HttpResponseMessage> DeleteProject(string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, $"/projects/{name}");
        request.Headers.Add("x-api-key", "dev");

        return await client.SendAsync(request, TestContext.Current.CancellationToken);
    }

    private async Task<T> ParseResource<T>(HttpResponseMessage response)
        where T : Resource
    {
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return await new FhirJsonParser().ParseAsync<T>(json);
    }

    private async Task<Patient> ParsePatient(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return await new FhirJsonParser().ParseAsync<Patient>(json);
    }

    /// <summary>
    ///     Selects a Project by wrapping the resource in a Parameters body carrying a 'project'
    ///     parameter — the request-body successor to the old <c>X-Project-Name</c> header.
    /// </summary>
    private async Task<HttpResponseMessage> DeIdentify(
        string resourceJson,
        string projectName,
        HttpClient httpClient = null
    )
    {
        return await DeIdentify(resourceJson, new FhirString(projectName), httpClient);
    }

    private async Task<HttpResponseMessage> DeIdentify(
        string resourceJson,
        DataType projectValue,
        HttpClient httpClient = null
    )
    {
        var resource = await new FhirJsonParser().ParseAsync<Resource>(resourceJson);
        var parameters = new Parameters().Add("project", projectValue).Add("resource", resource);

        using var content = new StringContent(await parameters.ToJsonAsync());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        using var request = new HttpRequestMessage(HttpMethod.Post, "/fhir/$de-identify")
        {
            Content = content,
        };

        return await (httpClient ?? client).SendAsync(
            request,
            TestContext.Current.CancellationToken
        );
    }

    private async Task<HttpResponseMessage> RegisterProject(
        string name,
        string yamlConfig,
        HttpClient httpClient = null
    )
    {
        using var content = new StringContent(yamlConfig);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/yaml");
        content.Headers.Add("x-api-key", "dev");

        return await (httpClient ?? client).PutAsync(
            $"/projects/{name}",
            content,
            TestContext.Current.CancellationToken
        );
    }
}
