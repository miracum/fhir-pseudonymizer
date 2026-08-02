using System.Text;
using FhirParametersGenerator;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace FhirPseudonymizer.Controllers;

[ApiController]
[Route("v3alpha1/fhir")]
[Produces("application/fhir+json")]
[Consumes("application/fhir+json", "application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class FhirV3Controller(
    ILogger<FhirV3Controller> logger,
    AnonymizationConfig anonymizationConfig,
    IPseudonymServiceClient psnClient,
    FeatureManagement features,
    IProvenancePublisher provenancePublisher
) : ControllerBase
{
    private readonly ILogger<FhirV3Controller> logger = logger;

    /// <summary>
    ///     Apply de-identification rules supplied inline in the request body to the accompanying FHIR resource.
    /// </summary>
    /// <param name="parameters">
    ///     A FHIR Parameters resource carrying a "config" part (an Attachment whose data is the
    ///     base64-encoded YAML anonymization config, e.g. the contents of hipaa-anonymization.yaml)
    ///     and a "resource" part holding the FHIR resource to de-identify.
    /// </param>
    /// <returns>The de-identified resource.</returns>
    /// <response code="200">Returns the de-identified resource</response>
    [HttpPost("$de-identify")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Resource), 200)]
    public async Task<ActionResult> DeIdentify([FromBody] Parameters parameters)
    {
        if (parameters is null)
        {
            logger.LogWarning("Bad Request: received request body is empty.");
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    "Received malformed or missing Parameters resource"
                )
            );
        }

        DeIdentifyRequest request;
        try
        {
            request = DeIdentifyRequest.FromFhirParameters(parameters);
        }
        catch (Exception exc)
        {
            logger.LogWarning(
                exc,
                "Bad Request: failed to parse the received Parameters resource."
            );
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    $"Failed to parse the received Parameters resource: {exc.Message}"
                )
            );
        }

        if (request.Resource is null)
        {
            logger.LogWarning("Bad Request: received Parameters has no 'resource' parameter.");
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    "Received Parameters has no 'resource' parameter"
                )
            );
        }

        if (request.Config is null)
        {
            logger.LogWarning("Bad Request: received Parameters has no 'config' parameter.");
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    "Received Parameters has no 'config' parameter"
                )
            );
        }

        AnonymizerEngine engine;
        try
        {
            var yamlConfig = Encoding.UTF8.GetString(request.Config.Data ?? []);
            var configurationManager = AnonymizerConfigurationManager.CreateFromYamlConfigString(
                yamlConfig,
                anonymizationConfig
            );

            engine = new AnonymizerEngine(configurationManager);
            engine.AddProcessor("pseudonymize", new PseudonymizationProcessor(psnClient, features));
        }
        catch (Exception exc)
        {
            logger.LogWarning(exc, "Bad Request: failed to parse the received config attachment.");
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    $"Failed to parse the received config attachment: {exc.Message}"
                )
            );
        }

        using var activity = Program.ActivitySource.StartActivity(nameof(DeIdentify));
        activity?.AddTag("resource.type", request.Resource.TypeName);
        activity?.AddTag("resource.id", request.Resource.Id);

        var settings = new AnonymizerSettings
        {
            ShouldAddSecurityTag = anonymizationConfig.ShouldAddSecurityTag,
        };

        try
        {
            var anonymized = await engine.AnonymizeResourceAsync(request.Resource, settings);
            provenancePublisher.Publish(request.Resource, anonymized);
            return Ok(anonymized);
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "DeIdentify failed");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                GetInternalErrorOutcome(exc)
            );
        }
    }

    private static OperationOutcome CreateOutcome(
        OperationOutcome.IssueSeverity severity,
        string diagnostics
    )
    {
        var outcome = new OperationOutcome();
        outcome.Issue.Add(
            new OperationOutcome.IssueComponent
            {
                Severity = severity,
                Code = OperationOutcome.IssueType.Processing,
                Diagnostics = diagnostics,
            }
        );
        return outcome;
    }

    private static OperationOutcome GetInternalErrorOutcome(Exception exc)
    {
        var outcome = new OperationOutcome();
        outcome.Issue.Add(
            new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Code = OperationOutcome.IssueType.Processing,
                Diagnostics =
                    $"An internal error occurred when processing the request: {exc.Message}.\nAt: {exc.StackTrace}",
            }
        );
        return outcome;
    }
}

[GenerateFhirParameters]
public partial class DeIdentifyRequest
{
    public Attachment Config { get; set; }
    public Resource Resource { get; set; }
}
