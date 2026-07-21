using System.Globalization;
using System.Text.RegularExpressions;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Projects;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Prometheus;

namespace FhirPseudonymizer.Controllers
{
    /// <summary>
    ///     The main FHIR operation endpoint.
    /// </summary>
    /// <response code="500">An unexpected internal error occurred</response>
    /// <response code="400">Invalid or missing resource in POST body received</response>
    /// <response code="401">Invalid authorization credentials</response>
    [ApiController]
    [Route("[controller]")]
    [Produces("application/fhir+json")]
    [Consumes("application/fhir+json", "application/json")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public partial class FhirController : ControllerBase
    {
        /// <summary>
        ///     Names end up selecting a config file, so the charset is kept to what is safe in a
        ///     filename and cannot escape the mounted directory; the length bound keeps a caller
        ///     from naming absurdly long paths. Anchored with '\z' rather than '$', which also
        ///     matches before a trailing newline.
        /// </summary>
        [GeneratedRegex(@"^[A-Za-z0-9._-]{1,64}\z")]
        private static partial Regex ValidProjectName();

        private static readonly Histogram BundleSizeHistogram = Metrics.CreateHistogram(
            "fhirpseudonymizer_received_bundle_size",
            "Histogram of received bundle sizes.",
            new HistogramConfiguration
            {
                // we divide measurements in 10 buckets of 5 each, up to 50.
                Buckets = Histogram.LinearBuckets(start: 1, width: 5, count: 20),
                LabelNames = new[] { "operation" },
            }
        );

        private readonly AnonymizationConfig config;
        private readonly ServerEngines serverEngines;
        private readonly IProvenancePublisher provenancePublisher;
        private readonly ILogger<FhirController> logger;
        private readonly IProjectConfigProvider projectConfigProvider;

        public FhirController(
            AnonymizationConfig config,
            ILogger<FhirController> logger,
            ServerEngines serverEngines,
            IProvenancePublisher provenancePublisher,
            IProjectConfigProvider projectConfigProvider
        )
        {
            this.config = config;
            this.logger = logger;
            this.serverEngines = serverEngines;
            this.provenancePublisher = provenancePublisher;
            this.projectConfigProvider = projectConfigProvider;

            BadRequestOutcome = OperationOutcomes.BadRequest(
                "Received malformed or missing resource"
            );
        }

        private OperationOutcome BadRequestOutcome { get; }

        /// <summary>
        ///     Apply de-identification rules to the given FHIR resource. The rules can be configured using the anonymization.yaml
        ///     config file.
        /// </summary>
        /// <param name="resource">
        ///     The FHIR resource to be de-identified. If the resource is of type 'Parameters' then the input is
        ///     fetched from the parameter named 'resource', and a config is selected by the (optional) 'project' parameter.
        /// </param>
        /// <returns>The de-identified resource.</returns>
        /// <response code="200">Returns the de-identified resource</response>
        [HttpPost("$de-identify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Resource), 200)]
        [ProducesResponseType(typeof(OperationOutcome), 400)]
        [ProducesResponseType(typeof(OperationOutcome), 500)]
        public async Task<ObjectResult> DeIdentify([FromBody] Resource resource)
        {
            if (resource == null)
            {
                logger.LogWarning("Bad Request: received request body is empty.");
                return BadRequest(BadRequestOutcome);
            }

            logger.LogDebug(
                "De-Identifying resource {resourceType}/{resourceId}",
                resource.TypeName,
                resource.Id
            );

            if (!TryResolveEngines(ProjectNameOf(resource), out var engines, out var failure))
            {
                return failure;
            }

            var settings = new AnonymizerSettings()
            {
                ShouldAddSecurityTag = config.ShouldAddSecurityTag,
            };

            if (resource is Parameters param)
            {
                // parse dynamic rule settings
                var dynamicSettings = param.GetSingle("settings")?.Part;
                if (dynamicSettings?.Any() == true)
                {
                    settings.DynamicRuleSettings = dynamicSettings.ToDictionary(
                        p => p.Name,
                        p => p.Value as object
                    );
                }

                var innerResource = param.GetSingle("resource")?.Resource;
                if (innerResource is null)
                {
                    logger.LogWarning(
                        "Bad Request: received Parameters carry no 'resource' parameter."
                    );
                    return BadRequest(BadRequestOutcome);
                }

                return await Anonymize(innerResource, settings, engines);
            }

            return await Anonymize(resource, settings, engines);
        }

        /// <summary>
        ///     The Project named by a request, or null when it names none: the (optional) 'project'
        ///     parameter of a Parameters body. A bare resource carries no name, so it is served with
        ///     the server's own config.
        /// </summary>
        private static string ProjectNameOf(Resource resource)
        {
            return (resource as Parameters)?.GetSingle("project")?.Value is FhirString name
                ? name.Value
                : null;
        }

        /// <summary>
        ///     Picks the Engines a request is served with: a named Project's when one is named,
        ///     otherwise the ones built from the startup config.
        /// </summary>
        /// <returns>
        ///     False when no Engines could be picked, with <c>failure</c> set to the response to
        ///     send instead.
        /// </returns>
        private bool TryResolveEngines(
            string projectName,
            out ProjectEngines engines,
            out ObjectResult failure
        )
        {
            engines = null;
            failure = null;

            if (!string.IsNullOrEmpty(projectName))
            {
                if (!ValidProjectName().IsMatch(projectName))
                {
                    failure = InvalidProjectName(projectName);
                    return false;
                }

                try
                {
                    if (projectConfigProvider.TryGetEngines(projectName, out engines))
                    {
                        return true;
                    }
                }
                catch (InvalidProjectConfigException exc)
                {
                    failure = InvalidProjectConfig(projectName, exc);
                    return false;
                }

                failure = UnknownProject(projectName);
                return false;
            }

            engines = serverEngines.Engines;
            if (engines is null)
            {
                failure = ProjectNameRequired();
                return false;
            }

            return true;
        }

        private ObjectResult InvalidProjectName(string projectName)
        {
            logger.LogWarning("Rejected the unusable project name {projectName}", projectName);

            return BadRequest(
                OperationOutcomes.BadRequest(
                    $"Invalid project name '{projectName}'. A name may hold up to 64 characters, "
                        + "each of them a letter, a digit, '.', '_' or '-'."
                )
            );
        }

        private ObjectResult InvalidProjectConfig(
            string projectName,
            InvalidProjectConfigException exc
        )
        {
            logger.LogWarning(
                exc,
                "The config file for project {projectName} cannot build engines",
                projectName
            );

            return BadRequest(OperationOutcomes.BadRequest(exc.Message));
        }

        /// <summary>
        ///     Refuses a request naming a Project this deployment does not define. The config comes
        ///     from a mounted file, so a miss is a deployment error the caller cannot fix by
        ///     retrying — hence a 400, not the transient-looking 404.
        /// </summary>
        private ObjectResult UnknownProject(string projectName)
        {
            logger.LogInformation(
                "Received a request for the unknown project {projectName}",
                projectName
            );

            return BadRequest(
                OperationOutcomes.BadRequest(
                    $"Unknown project '{projectName}'. This deployment defines no config file for it."
                )
            );
        }

        /// <summary>
        ///     Refuses a request that names no Project on a deployment started with no Config of
        ///     its own. The caller has to name one, so this is a 400: no retry of the request as
        ///     sent can succeed.
        /// </summary>
        private ObjectResult ProjectNameRequired()
        {
            logger.LogInformation(
                "Received a request naming no project, which this server cannot serve without a config of its own"
            );

            return BadRequest(
                OperationOutcomes.Required(
                    "This server holds no anonymization config of its own, so every request must "
                        + "name a registered project via a 'project' parameter in a Parameters body."
                )
            );
        }

        private async Task<ObjectResult> Anonymize(
            Resource resource,
            AnonymizerSettings anonymizerSettings,
            ProjectEngines engines
        )
        {
            using var activity = Program.ActivitySource.StartActivity(nameof(Anonymize));
            activity?.AddTag("resource.type", resource.TypeName);
            activity?.AddTag("resource.id", resource.Id);

            if (resource is Bundle bundle)
            {
                activity?.AddTag("bundle.size", bundle.Entry.Count);
                BundleSizeHistogram.WithLabels(nameof(DeIdentify)).Observe(bundle.Entry.Count);
            }

            try
            {
                var anonymized = await engines.Anonymizer.AnonymizeResourceAsync(
                    resource,
                    anonymizerSettings
                );

                // Provenance is published for Project-scoped requests too: it is derived purely by
                // diffing the resource before and after, so it stays accurate whichever Project's
                // engine produced it, and it carries no Project name that could leak into the
                // record. Exempting them would blind the audit trail exactly where the applied
                // rules are least predictable.
                provenancePublisher.Publish(resource, anonymized);
                return Ok(anonymized);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Anonymize failed");
                return StatusCode(500, OperationOutcomes.InternalError(exc));
            }
        }

        /// <summary>
        ///     Revert any reversible de-identification methods previously applied to the given FHIR resource.
        ///     Always served with the server's own config: a projects-only deployment has no key of its own,
        ///     so it cannot de-pseudonymize.
        /// </summary>
        /// <param name="resource">The FHIR resource containing pseudonymized fields that are to be de-pseudonymized.</param>
        /// <returns>The modified FHIR resource with the pseudonymized fields replaced with the original value.</returns>
        /// <response code="200">Returns the de-pseudonymized resource</response>
        [HttpPost("$de-pseudonymize")]
        [Authorize]
        [ProducesResponseType(typeof(Resource), 200)]
        [ProducesResponseType(typeof(OperationOutcome), 400)]
        [ProducesResponseType(typeof(OperationOutcome), 500)]
        public async Task<ObjectResult> DePseudonymize([FromBody] Resource resource)
        {
            if (resource == null)
            {
                logger.LogWarning("Bad Request: received request body is empty.");
                return BadRequest(BadRequestOutcome);
            }

            logger.LogDebug(
                "De-Pseudonymizing resource {resourceType}/{resourceId}",
                resource.TypeName,
                resource.Id
            );

            var engines = serverEngines.Engines;
            if (engines is null)
            {
                logger.LogInformation(
                    "Received a $de-pseudonymize request on a server with no config of its own"
                );

                return BadRequest(
                    OperationOutcomes.Required(
                        "This server holds no anonymization config of its own; $de-pseudonymize is "
                            + "unavailable on a projects-only deployment."
                    )
                );
            }

            if (resource is Bundle bundle)
            {
                BundleSizeHistogram.WithLabels(nameof(DePseudonymize)).Observe(bundle.Entry.Count);
            }

            try
            {
                return Ok(await engines.DePseudonymizer.DePseudonymizeResourceAsync(resource));
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "DePseudonymize failed");
                return StatusCode(500, OperationOutcomes.InternalError(exc));
            }
        }

        /// <summary>
        ///     Returns the server's FHIR CapabilityStatement.
        ///     Note that this CapabilityStatement is not valid at this point as it does not include the custom operations.
        /// </summary>
        /// <returns>The server's FHIR CapabilityStatement.</returns>
        [HttpGet("metadata")]
        public CapabilityStatement GetMetadata()
        {
            return new()
            {
                Status = PublicationStatus.Active,
                Date = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture),
                Kind = CapabilityStatementKind.Instance,
                Software = new CapabilityStatement.SoftwareComponent
                {
                    Name = "FHIR Pseudonymizer",
                },
                FhirVersion = FHIRVersion.N4_0_1,
                Format = new[] { "application/fhir+json" },
                Rest = new List<CapabilityStatement.RestComponent>
                {
                    new() { Mode = CapabilityStatement.RestfulCapabilityMode.Server },
                },
            };
        }
    }
}
