using System.Security.Cryptography;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NBomber.Contracts.Stats;
using NBomber.Http;
using Polly;
using Polly.Retry;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FhirPseudonymizer.StressTests;

public class StressTests
{
    private readonly string reportFolder;
    private readonly ITestOutputHelper output;
    private readonly AsyncRetryPolicy retryPolicy;
    private readonly Uri pseudonymizerBaseAddress;

    public StressTests(ITestOutputHelper output)
    {
        this.output = output;

        pseudonymizerBaseAddress = new Uri(
            Environment.GetEnvironmentVariable("FHIR_PSEUDONYMIZER_BASE_URL")
                ?? "http://localhost:5000/fhir"
        );

        retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<FhirOperationException>()
            .Or<OperationCanceledException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryAttempt, context) =>
                {
                    var stepContext = context["stepContext"] as IScenarioContext;
                    stepContext?.Logger.Warning(
                        $"Request failed within retry context: {exception.GetType()}: {exception.Message}. Attempt {retryAttempt}."
                    );
                }
            );

        _ = bool.TryParse(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            out bool isRunningInContainer
        );
        reportFolder = isRunningInContainer
            ? Path.Combine(Path.GetTempPath(), "reports")
            : "./nbomber-reports";
    }

    private async Task<Response<object>> RunPseudonymizeResource(
        IScenarioContext scenarioContext,
        HttpClient httpClient
    )
    {
        return await Step.Run(
            "pseudonymize resource",
            scenarioContext,
            run: async () =>
            {
                // this is basically just a copy-paste of what Vfps does when configured to
                // use the `Sha256HexEncoded` pseudonymization method
                var originalRecordNumber = Guid.NewGuid().ToString();
                var inputAsBytes = Encoding.UTF8.GetBytes(originalRecordNumber);
                var sha256Bytes = SHA256.HashData(inputAsBytes);
                var expectedPseudonym = $"stress-{Convert.ToHexString(sha256Bytes)}";

                var resource = new Patient()
                {
                    Id = Guid.NewGuid().ToString(),
                    Active = true,
                    Name = new()
                    {
                        new()
                        {
                            Family = "Doe",
                            Given = new List<string> { "John" },
                        },
                    },
                    Identifier = new()
                    {
                        new("https://fhir.example.com/identifiers/mrn", originalRecordNumber)
                        {
                            Type = new("http://terminology.hl7.org/CodeSystem/v2-0203", "MR"),
                        },
                    },
                };

                var parameters = new Parameters().Add("resource", resource);

                using var fhirClient = new FhirClient(
                    pseudonymizerBaseAddress,
                    httpClient,
                    settings: new() { PreferredFormat = ResourceFormat.Json, Timeout = 15_000 }
                );

                try
                {
                    var response = await retryPolicy.ExecuteAsync(
                        (ctx) => fhirClient.WholeSystemOperationAsync("de-identify", parameters),
                        new Dictionary<string, object> { ["stepContext"] = scenarioContext }
                    );

                    var pseudonymizedPatient = response as Patient;

                    pseudonymizedPatient?.Should().NotBeNull();
                    pseudonymizedPatient!.Identifier.Should().HaveCount(1);
                    pseudonymizedPatient!.Identifier.First().Value.Should().Be(expectedPseudonym);

                    return Response.Ok(statusCode: "200");
                }
                catch (Exception exc)
                    when (exc is HttpRequestException
                        || exc is FhirOperationException
                        || exc is OperationCanceledException
                    )
                {
                    // catch the retry-able exceptions related to transient errors. Any exceptions thrown by
                    // the FluentAssertions (Should) will still create test-failing exceptions. Their invariants must
                    // always hold.
                    scenarioContext.Logger.Error(exc, "Pseudonymization of resource failed");
                    return Response.Fail();
                }
                catch (XunitException exc)
                {
                    scenarioContext.Logger.Error(exc, "Stopping test due to invariant violation.");
                    scenarioContext.StopCurrentTest(exc.Message);
                    return Response.Fail("400", exc.Message, sizeBytes: 0);
                }
            }
        );
    }

    [Theory]
    [InlineData(0.1)]
    public void StressTest_FailurePercentage_ShouldBeLessThanThreshold(
        double failPercentageThreshold
    )
    {
        using var httpClient = new HttpClient();
        var scenario = Scenario
            .Create(
                "de-identify",
                async context =>
                {
                    return await RunPseudonymizeResource(context, httpClient);
                }
            )
            .WithInit(async context =>
            {
                using var fhirClient = new FhirClient(
                    pseudonymizerBaseAddress,
                    httpClient,
                    settings: new() { PreferredFormat = ResourceFormat.Json }
                );
                await fhirClient.CapabilityStatementAsync();
                context.Logger.Information("Completed scenario init.");
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(10))
            .WithLoadSimulations(
                Simulation.RampingConstant(copies: 10, during: TimeSpan.FromMinutes(5)),
                Simulation.KeepConstant(copies: 100, during: TimeSpan.FromMinutes(10))
            );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder(reportFolder)
            .WithWorkerPlugins(new HttpMetricsPlugin(new[] { HttpVersion.Version1 }))
            .WithReportFormats(
                ReportFormat.Txt,
                ReportFormat.Csv,
                ReportFormat.Html,
                ReportFormat.Md
            )
            .Run();

        var deIdentifyStatusCodes = stats.ScenarioStats.Get("de-identify").Fail.StatusCodes;

        deIdentifyStatusCodes
            .Should()
            .NotContain(
                statusCodeStats => statusCodeStats.StatusCode == "400",
                because: "it means that pseudonym validation failed."
            );

        var failPercentage = stats.AllFailCount / (double)stats.AllRequestCount * 100.0;

        output.WriteLine(
            $"Actual fail percentage: {failPercentage} %. Threshold: {failPercentageThreshold} %"
        );

        failPercentage.Should().BeLessThan(failPercentageThreshold);
    }
}
