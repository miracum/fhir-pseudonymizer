using FhirPseudonymizer.Projects;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FhirPseudonymizer.Controllers;

/// <summary>
///     Registers the anonymization Config a Project's requests are served with.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces("application/fhir+json")]
[Authorize(Policy = ApiKeyExtensions.ProjectRegistrationPolicy)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProjectsController(IProjectRegistry registry, ILogger<ProjectsController> logger)
    : ControllerBase
{
    /// <summary>
    ///     Register a Project's anonymization Config so later requests can select it by name.
    /// </summary>
    /// <param name="name">The caller-chosen Project name.</param>
    /// <returns>No content.</returns>
    /// <response code="200">An earlier config for this Project was replaced.</response>
    /// <response code="201">The Project was registered.</response>
    /// <response code="400">The config could not be parsed or is not a usable anonymization config.</response>
    /// <response code="503">The registry is full; retry shortly.</response>
    [HttpPut("{name}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof(OperationOutcome), 400)]
    [ProducesResponseType(typeof(OperationOutcome), 503)]
    public async Task<IActionResult> Register(string name)
    {
        var loggedName = name.ForLog();

        if (!ProjectName.IsValid(name))
        {
            logger.LogWarning("Rejected the unusable project name {projectName}", loggedName);

            return BadRequest(
                OperationOutcomes.BadRequest($"Invalid project name. {ProjectName.Rule}")
            );
        }

        // Read the YAML straight off the request body: the registered input formatters are
        // FHIR-JSON only, so a [FromBody] parameter would reject an application/yaml payload.
        using var reader = new StreamReader(Request.Body);
        var yamlConfig = await reader.ReadToEndAsync();

        ProjectRegistrationOutcome outcome;
        try
        {
            outcome = registry.Register(name, yamlConfig);
        }
        catch (InvalidProjectConfigException exc)
        {
            logger.LogWarning(
                exc,
                "Rejected the config offered for project {projectName}",
                loggedName
            );
            return BadRequest(OperationOutcomes.BadRequest(exc.Message));
        }

        if (outcome == ProjectRegistrationOutcome.NotStored)
        {
            logger.LogWarning(
                "Could not store project {projectName}: the registry is at its size limit",
                loggedName
            );

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                OperationOutcomes.TryLater(
                    "The project registry is full. Reaching the limit triggers a compaction, so retry shortly."
                )
            );
        }

        logger.LogInformation("Registered project {projectName} ({outcome})", loggedName, outcome);

        return outcome switch
        {
            ProjectRegistrationOutcome.Replaced => Ok(),
            _ => StatusCode(StatusCodes.Status201Created),
        };
    }

    /// <summary>
    ///     Release a Project, so requests naming it are answered with a 404 again.
    /// </summary>
    /// <param name="name">The Project name.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The Project is no longer registered here.</response>
    [HttpDelete("{name}")]
    [ProducesResponseType(204)]
    public IActionResult Unregister(string name)
    {
        // Idempotent: the registry is a cache, so "was never here" and "is no longer here" are
        // the same state to the caller.
        registry.Remove(name);

        return NoContent();
    }
}
