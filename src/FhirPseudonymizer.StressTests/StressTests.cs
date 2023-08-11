using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using NBomber.Contracts.Stats;
using Xunit.Abstractions;
using Polly;
using Polly.Retry;
using Microsoft.FSharp.Core;

namespace FhirPseudonymizer.StressTests;

public class StressTests
{
    private readonly string reportFolder;
    private readonly FhirClient fhirClient;
    private readonly ITestOutputHelper output;
    private readonly AsyncRetryPolicy retryPolicy;

    public StressTests(ITestOutputHelper output)
    {
        this.output = output;

        var pseudonymizerBaseAddress = new Uri(
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
                    var stepContext = context["stepContext"] as IStepContext<Unit, Unit>;
                    stepContext.Logger.Warning(
                        $"Request failed within retry context: {exception.GetType()}: {exception.Message}. Attempt {retryAttempt}."
                    );
                }
            );

        fhirClient = new FhirClient(
            pseudonymizerBaseAddress,
            settings: new() { PreferredFormat = ResourceFormat.Json, Timeout = 15_000 }
        );

        _ = bool.TryParse(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            out bool isRunningInContainer
        );
        reportFolder = isRunningInContainer
            ? Path.Combine(Path.GetTempPath(), "reports")
            : "./nbomber-reports";
    }

    private IStep PseudonymizeResourceStep()
    {
        return Step.Create(
            "pseudonymize_resource",
            execute: async context =>
            {
                try
                {
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
                            }
                        },
                        Identifier = new()
                        {
                            new(
                                "https://fhir.example.com/identifiers/mrn",
                                Guid.NewGuid().ToString()
                            )
                            {
                                Type = new("http://terminology.hl7.org/CodeSystem/v2-0203", "MR"),
                            }
                        },
                    };

                    var parameters = new Parameters().Add("resource", resource);

                    var response = await retryPolicy.ExecuteAsync(
                        (ctx) =>
                            fhirClient.WholeSystemOperationAsync(
                                "de-identify",
                                parameters,
                                ct: context.CancellationToken
                            ),
                        new Dictionary<string, object> { ["stepContext"] = context }
                    );

                    return Response.Ok(statusCode: 200);
                }
                catch (Exception exc)
                {
                    context.Logger.Error(exc, "Pseudonymization of resource failed");
                    return Response.Fail();
                }
            },
            timeout: TimeSpan.FromSeconds(60)
        );
    }

    [Theory]
    [InlineData(0.1)]
    public void StressTest_FailurePercentage_ShouldBeLessThanThreshold(
        double failPercentageThreshold
    )
    {
        var scenario = ScenarioBuilder
            .CreateScenario("de-identify", PseudonymizeResourceStep())
            .WithInit(async context =>
            {
                await fhirClient.CapabilityStatementAsync(ct: context.CancellationToken);
                context.Logger.Information("Completed scenario init.");
            })
            .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            .WithLoadSimulations(
                Simulation.RampConstant(copies: 10, during: TimeSpan.FromMinutes(5)),
                Simulation.KeepConstant(copies: 100, during: TimeSpan.FromMinutes(5)),
                Simulation.InjectPerSecRandom(
                    minRate: 10,
                    maxRate: 50,
                    during: TimeSpan.FromMinutes(5)
                )
            );

        var stats = NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFolder(reportFolder)
            .WithReportFormats(
                ReportFormat.Txt,
                ReportFormat.Csv,
                ReportFormat.Html,
                ReportFormat.Md
            )
            .Run();

        var failPercentage = stats.FailCount / (double)stats.RequestCount * 100.0;

        output.WriteLine(
            $"Actual fail percentage: {failPercentage} %. Threshold: {failPercentageThreshold} %"
        );

        failPercentage.Should().BeLessThan(failPercentageThreshold);
    }
}
