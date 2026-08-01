using FhirPseudonymizer.Pseudonymization.DeIdentification;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FhirPseudonymizer.Controllers;

/// <summary>
///     Experimental v2alpha1 FHIR operations. These endpoints are still under development and may
///     change or be removed without notice.
/// </summary>
/// <response code="500">An unexpected internal error occurred</response>
/// <response code="400">Invalid or missing Parameters resource in POST body received</response>
[ApiController]
[Route("v2alpha1/fhir")]
[Produces("application/fhir+json")]
[Consumes("application/fhir+json", "application/json")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public class FhirV2Alpha1Controller : ControllerBase
{
    private readonly ILogger<FhirV2Alpha1Controller> logger;

    public FhirV2Alpha1Controller(ILogger<FhirV2Alpha1Controller> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    ///     Apply de-identification rules supplied inline in the request body - the same rules and
    ///     parameters that would otherwise be configured via an anonymization.yaml config file, e.g.
    ///     hipaa-anonymization.yaml - to the accompanying FHIR resource.
    /// </summary>
    /// <param name="parameters">
    ///     A FHIR Parameters resource carrying the same "fhirVersion", "fhirPathRules" and
    ///     "parameters" parts as an anonymization.yaml config file, plus a "resource" part holding
    ///     the FHIR resource to de-identify.
    /// </param>
    /// <response code="501">
    ///     The request was parsed successfully, but applying inline de-identification rules is not
    ///     yet implemented.
    /// </response>
    [HttpPost("$de-identify")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult DeIdentify([FromBody] Parameters parameters)
    {
        if (parameters == null)
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

        if (request.Resource == null)
        {
            logger.LogWarning("Bad Request: received Parameters carry no 'resource' parameter.");
            return BadRequest(
                CreateOutcome(
                    OperationOutcome.IssueSeverity.Error,
                    "Received Parameters carry no 'resource' parameter"
                )
            );
        }

        logger.LogDebug(
            "Parsed $de-identify request for {resourceType} with {ruleCount} fhirPathRules",
            request.Resource.TypeName,
            request.FhirPathRules.Count
        );

        return StatusCode(
            StatusCodes.Status501NotImplemented,
            CreateOutcome(
                OperationOutcome.IssueSeverity.Information,
                "The request was parsed successfully, but applying inline de-identification rules is not yet implemented."
            )
        );
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
}
