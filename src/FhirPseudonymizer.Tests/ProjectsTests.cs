using System.Net;
using System.Net.Http.Headers;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;

namespace FhirPseudonymizer.Tests;

/// <summary>
///     Drives the Projects feature over HTTP: a <c>$de-identify</c> request selects one of the
///     configs mounted in <see cref="configsDir" /> by naming it in a 'project' parameter.
/// </summary>
public sealed class ProjectsTests : IDisposable
{
    /// <summary>Redacts the identifier's value but leaves the name — the opposite fingerprint
    /// from <see cref="ServerRedactsNameConfig" />, so which config ran is observable.</summary>
    private const string RedactIdentifierConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('Identifier').value
            method: redact
        """;

    /// <summary>The server's own rules for the fallback tests: redacts the name and no
    /// Project config touches names, so a surviving name proves a Project's rules replaced these.</summary>
    private const string ServerRedactsNameConfig = """
        fhirVersion: R4
        fhirPathRules:
          - path: nodesByType('HumanName')
            method: redact
        """;

    private const string InvalidSyntaxConfig = "fhirPathRules: [this bracket never closes";

    private readonly string configsDir;

    public ProjectsTests()
    {
        configsDir = Path.Join(
            Path.GetTempPath(),
            "fp-projects-it-" + Guid.NewGuid().ToString("N")
        );
        Directory.CreateDirectory(configsDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(configsDir))
        {
            Directory.Delete(configsDir, recursive: true);
        }
    }

    [Fact]
    public async Task PostDeIdentify_NamingAMountedProject_AppliesThatProjectsRules()
    {
        WriteProjectConfig("redactor", RedactIdentifierConfig);
        var client = Client(serverConfig: ServerRedactsNameConfig);

        var response = await DeIdentify(client, project: "redactor");

        response.EnsureSuccessStatusCode();
        var patient = await ParsePatient(response);
        patient.Identifier[0].Value.Should().BeNull("the project redacts identifier values");
        patient
            .Name.Should()
            .NotBeEmpty(
                "the project's rules replace the server's, which would have redacted the name"
            );
    }

    [Fact]
    public async Task PostDeIdentify_NamingAProjectWithNoMountedConfig_ReturnsBadRequest()
    {
        var client = Client(serverConfig: ServerRedactsNameConfig);

        var response = await DeIdentify(client, project: "never-mounted");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var outcome = await ParseResource<OperationOutcome>(response);
        outcome.Issue.Should().ContainSingle().Which.Diagnostics.Should().Contain("never-mounted");
    }

    [Fact]
    public async Task PostDeIdentify_NamingAProjectWhoseConfigIsInvalid_ReturnsBadRequest()
    {
        WriteProjectConfig("broken", InvalidSyntaxConfig);
        var client = Client(serverConfig: ServerRedactsNameConfig);

        var response = await DeIdentify(client, project: "broken");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostDeIdentify_NamingAnUnusableProjectName_ReturnsBadRequest()
    {
        var client = Client(serverConfig: ServerRedactsNameConfig);

        var response = await DeIdentify(client, project: "not a valid name!");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostDeIdentify_NamingNoProject_UsesTheServerConfig()
    {
        var client = Client(serverConfig: ServerRedactsNameConfig);

        // A bare resource carries no project part, so it is served with the server's own rules.
        var response = await DeIdentifyBare(client);

        response.EnsureSuccessStatusCode();
        var patient = await ParsePatient(response);
        patient.Name.Should().BeEmpty("the server config redacts the name");
    }

    [Fact]
    public async Task PostDeIdentify_NamingNoProject_OnAProjectsOnlyServer_ReturnsBadRequestRequired()
    {
        var client = Client(serverConfig: null);

        var response = await DeIdentifyBare(client);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var outcome = await ParseResource<OperationOutcome>(response);
        outcome
            .Issue.Should()
            .ContainSingle()
            .Which.Code.Should()
            .Be(OperationOutcome.IssueType.Required);
    }

    private void WriteProjectConfig(string name, string yaml)
    {
        File.WriteAllText(Path.Join(configsDir, $"{name}.yaml"), yaml);
    }

    private HttpClient Client(string serverConfig)
    {
        var factory = new CustomWebApplicationFactory<Startup>
        {
            CustomInMemorySettings = new Dictionary<string, string>
            {
                ["EnableMetrics"] = "false",
                ["PseudonymizationService"] = "None",
                ["AnonymizationEngineConfigPath"] = "",
                ["AnonymizationEngineConfigInline"] = serverConfig ?? "",
                ["ProjectConfigsDirectory"] = configsDir,
            },
        };
        return factory.CreateClient();
    }

    private static Patient SamplePatient()
    {
        return new Patient
        {
            Id = "glossy",
            Identifier = new List<Identifier> { new("http://example.org/mrn", "123456") },
            Name = new List<HumanName> { new() { Family = "Mustermann" } },
        };
    }

    private static async Task<HttpResponseMessage> DeIdentify(HttpClient client, string project)
    {
        var parameters = new Parameters();
        parameters.Add("project", new FhirString(project));
        parameters.Add("resource", SamplePatient());

        return await Post(
            client,
            await new FhirJsonSerializer().SerializeToStringAsync(parameters)
        );
    }

    private static async Task<HttpResponseMessage> DeIdentifyBare(HttpClient client)
    {
        return await Post(
            client,
            await new FhirJsonSerializer().SerializeToStringAsync(SamplePatient())
        );
    }

    private static async Task<HttpResponseMessage> Post(HttpClient client, string bodyJson)
    {
        using var content = new StringContent(bodyJson);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/fhir+json");

        return await client.PostAsync(
            "/fhir/$de-identify",
            content,
            TestContext.Current.CancellationToken
        );
    }

    private async Task<T> ParseResource<T>(HttpResponseMessage response)
        where T : Resource
    {
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return await new FhirJsonParser().ParseAsync<T>(json);
    }

    private Task<Patient> ParsePatient(HttpResponseMessage response)
    {
        return ParseResource<Patient>(response);
    }
}
