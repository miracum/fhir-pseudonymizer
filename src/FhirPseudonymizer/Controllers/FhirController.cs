using System.Globalization;
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
    public class FhirController : ControllerBase
    {
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
        private readonly ProjectEngines serverEngines;
        private readonly IProvenancePublisher provenancePublisher;
        private readonly ILogger<FhirController> logger;
        private readonly IProjectRegistry projectRegistry;

        public FhirController(
            AnonymizationConfig config,
            ILogger<FhirController> logger,
            ProjectEngines serverEngines,
            IProvenancePublisher provenancePublisher,
            IProjectRegistry projectRegistry
        )
        {
            this.config = config;
            this.logger = logger;
            this.serverEngines = serverEngines;
            this.provenancePublisher = provenancePublisher;
            this.projectRegistry = projectRegistry;

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
        ///     fetched from the parameter named 'resource', and a registered Project's config is selected by the
        ///     (optional) 'project' parameter.
        /// </param>
        /// <returns>The de-identified resource.</returns>
        /// <response code="200">Returns the de-identified resource</response>
        [HttpPost("$de-identify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Resource), 200)]
        [ProducesResponseType(typeof(OperationOutcome), 400)]
        [ProducesResponseType(typeof(OperationOutcome), 404)]
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

            if (!TryResolveEngines(resource, out var engines, out var failure))
            {
                return failure;
            }

            var settings = new AnonymizerSettings()
            {
                ShouldAddSecurityTag = config.ShouldAddSecurityTag,
            };

            if (resource is Parameters param)
            {
                // parse dynamic rule settings. Parts without a name are ignored (there is no
                // setting to apply them to) and duplicate names keep the last occurrence's value,
                // matching the "last one wins" precedence already used when merging these into a
                // rule's own settings (see AnonymizationVisitor.MergeSettings) - both are needed
                // since a caller fully controls this list and either would otherwise throw
                // (ToDictionary rejects null and duplicate keys alike).
                var dynamicSettings = param.GetSingle("settings")?.Part;
                if (dynamicSettings?.Any() == true)
                {
                    settings.DynamicRuleSettings = dynamicSettings
                        .Where(p => !string.IsNullOrEmpty(p.Name))
                        .GroupBy(p => p.Name)
                        .ToDictionary(g => g.Key, g => g.Last().Value as object);
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
        ///     Picks the Engines a request is served with: a registered Project's when the
        ///     (optional) 'project' parameter of a Parameters body names one, otherwise the ones
        ///     built from the startup config. A bare resource names no Project.
        /// </summary>
        /// <returns>
        ///     False when the request cannot be served, with <c>failure</c> set to the response to
        ///     send instead.
        /// </returns>
        private bool TryResolveEngines(
            Resource resource,
            out ProjectEngines engines,
            out ObjectResult failure
        )
        {
            failure = null;
            engines = serverEngines;

            // Not GetSingle, which throws on a duplicated name — a caller's mistake that has to
            // answer 400, not 500.
            var selectors = (resource as Parameters)?.Get("project").ToList();
            if (selectors is null or [])
            {
                return true;
            }

            if (selectors.Count > 1)
            {
                failure = AmbiguousProjectSelection(selectors.Count);
                return false;
            }

            var selector = selectors[0];

            // A 'project' parameter the server cannot read a name out of is refused rather than
            // served with the server's own config: the caller asked for a Project's rules, so
            // quietly applying different ones would hand back data they believe was de-identified
            // some other way. A name registration would reject gets the same answer instead of the
            // 404 below, whose re-register-and-retry advice could never resolve it.
            if (selector.Value is not FhirString name || !ProjectName.IsValid(name.Value))
            {
                failure = UnusableProjectName(selector.Value);
                return false;
            }

            if (projectRegistry.TryGet(name.Value, out engines))
            {
                return true;
            }

            failure = UnknownProject(name.Value);
            return false;
        }

        /// <summary>
        ///     Refuses a request carrying the 'project' parameter more than once: the server
        ///     cannot tell which of the named configs the caller expects to run, and picking one
        ///     would be as invisible as the fallback the single-parameter checks refuse.
        /// </summary>
        private ObjectResult AmbiguousProjectSelection(int count)
        {
            // Only the count is logged: the names are as unvetted as any other rejected selector.
            logger.LogWarning(
                "Rejected a request carrying {parameterCount} 'project' parameters",
                count
            );

            return BadRequest(
                OperationOutcomes.BadRequest(
                    "The 'project' parameter must not appear more than once."
                )
            );
        }

        /// <summary>
        ///     Refuses a 'project' parameter that names nothing that could ever be registered.
        /// </summary>
        private ObjectResult UnusableProjectName(DataType value)
        {
            // The value's FHIR type name is a fixed vocabulary, unlike the name it would carry,
            // so it can be logged as it is.
            logger.LogWarning(
                "Rejected a request whose 'project' parameter carries no usable name ({parameterType})",
                value?.TypeName ?? "no value"
            );

            return BadRequest(
                OperationOutcomes.BadRequest(
                    "The 'project' parameter must carry a valueString naming a registered project. "
                        + ProjectName.Rule
                )
            );
        }

        /// <summary>
        ///     Refuses a request naming a Project this server does not hold. A miss is expected —
        ///     a restart, scale-out or eviction all produce one — so the caller is told to
        ///     re-register and retry.
        /// </summary>
        private ObjectResult UnknownProject(string projectName)
        {
            logger.LogInformation(
                "Received a request for the unknown project {projectName}",
                projectName.ForLog()
            );

            // Safe to quote back: the name passed the same check registration applies, so it is
            // bounded and holds nothing a response or a log line has to be protected from.
            return NotFound(
                OperationOutcomes.NotFound(
                    $"Unknown project '{projectName}'. Register its config via PUT /projects/{projectName} and retry."
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

                // Provenance is published for Project-scoped requests too: it records only that a
                // de-identification happened and the before/after identity of the resource it
                // happened to, never which rules ran, so nothing in it is specific to the engine
                // that produced it and no Project name can enter the record. Exempting them would
                // blind the audit trail exactly where the applied rules are least predictable.
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
        ///     Always served with the server's own config, never a Project's: a Project carries its own
        ///     keys, so reversing its output through this API would cross the boundary that keeps one
        ///     Project from reading another's data.
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

            if (resource is Bundle bundle)
            {
                BundleSizeHistogram.WithLabels(nameof(DePseudonymize)).Observe(bundle.Entry.Count);
            }

            try
            {
                return Ok(
                    await serverEngines.DePseudonymizer.DePseudonymizeResourceAsync(resource)
                );
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
