using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using FhirPseudonymizer.Config;
using FhirPseudonymizer.Kafka;
using FhirPseudonymizer.Pseudonymization;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Health.Fhir.Anonymizer.Core;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Prometheus;

namespace FhirPseudonymizer.Controllers
{
    public static class AnonymizerConfigCacheKeys
    {
        public const string AnonymizerConfig = "AnonymizerConfigCache";
    }

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
                LabelNames = ["operation"],
            }
        );

        private readonly IAnonymizerEngine anonymizer;
        private readonly AnonymizationConfig config;
        private readonly IDePseudonymizerEngine dePseudonymizer;
        private readonly IProvenancePublisher provenancePublisher;
        private readonly ILogger<FhirController> logger;
        private readonly IPseudonymServiceClient psnClient;
        private readonly FeatureManagement features;
        private readonly IMemoryCache anonymizerConfigCache;
        private readonly MemoryCacheEntryOptions anonymizerConfigCacheEntryOptions;

        public FhirController(
            AnonymizationConfig config,
            ILogger<FhirController> logger,
            IAnonymizerEngine anonymizer,
            IDePseudonymizerEngine dePseudonymizer,
            IProvenancePublisher provenancePublisher,
            IPseudonymServiceClient psnClient,
            FeatureManagement features,
            [FromKeyedServices(AnonymizerConfigCacheKeys.AnonymizerConfig)]
                IMemoryCache anonymizerConfigCache,
            [FromKeyedServices(AnonymizerConfigCacheKeys.AnonymizerConfig)]
                MemoryCacheEntryOptions anonymizerConfigCacheEntryOptions
        )
        {
            this.config = config;
            this.logger = logger;
            this.anonymizer = anonymizer;
            this.dePseudonymizer = dePseudonymizer;
            this.provenancePublisher = provenancePublisher;
            this.psnClient = psnClient;
            this.features = features;
            this.anonymizerConfigCache = anonymizerConfigCache;
            this.anonymizerConfigCacheEntryOptions = anonymizerConfigCacheEntryOptions;

            BadRequestOutcome = new();
            BadRequestOutcome.Issue.Add(
                new OperationOutcome.IssueComponent
                {
                    Severity = OperationOutcome.IssueSeverity.Error,
                    Code = OperationOutcome.IssueType.Processing,
                    Diagnostics = "Received malformed or missing resource",
                }
            );
        }

        private OperationOutcome BadRequestOutcome { get; }

        /// <summary>
        ///     Apply de-identification rules to the given FHIR resource. The rules can be configured using the anonymization.yaml
        ///     config file, or supplied per-request (see below).
        /// </summary>
        /// <param name="resource">
        ///     The FHIR resource to be de-identified. If the resource is of type 'Parameters' then the input is
        ///     fetched from the parameter named 'resource'. The 'Parameters' resource may also carry a 'config'
        ///     parameter (an Attachment whose data is a base64-encoded YAML anonymization config, e.g. the contents
        ///     of hipaa-anonymization.yaml) to replace the server's statically configured rules for this request
        ///     only, and/or a 'settings' parameter to override individual rule settings (see the "Dynamic rule
        ///     settings" docs).
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
                if (dynamicSettings?.Count > 0)
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

                IAnonymizerEngine engine = anonymizer;
                if (param.GetSingle("config")?.Value is Attachment configAttachment)
                {
                    try
                    {
                        engine = await GetOrCreateDynamicEngine(configAttachment);
                    }
                    catch (Exception exc)
                    {
                        logger.LogWarning(
                            exc,
                            "Bad Request: failed to parse the received config attachment."
                        );
                        return BadRequest(
                            CreateBadRequestOutcome(
                                $"Failed to parse the received config attachment: {exc.Message}"
                            )
                        );
                    }
                }

                return await Anonymize(innerResource, settings, engine);
            }

            return await Anonymize(resource, settings, anonymizer);
        }

        /// <summary>
        ///     Builds (or retrieves from cache) an <see cref="IAnonymizerEngine" /> configured from the given
        ///     YAML config attachment, keyed by the SHA-256 hash of its raw bytes so repeated requests using the
        ///     same config reuse the same parsed engine instead of re-parsing the YAML every time.
        /// </summary>
        private async Task<IAnonymizerEngine> GetOrCreateDynamicEngine(Attachment configAttachment)
        {
            var configBytes = configAttachment.Data ?? [];
            var yamlConfig = Encoding.UTF8.GetString(configBytes);
            var configCacheKey = Convert.ToHexString(SHA256.HashData(configBytes));

            return await anonymizerConfigCache.GetOrCreateAsync(
                configCacheKey,
                entry =>
                {
                    var configurationManager =
                        AnonymizerConfigurationManager.CreateFromYamlConfigString(
                            yamlConfig,
                            config
                        );
                    var engine = new AnonymizerEngine(configurationManager);
                    engine.AddProcessor(
                        "pseudonymize",
                        new PseudonymizationProcessor(psnClient, features)
                    );
                    return System.Threading.Tasks.Task.FromResult<IAnonymizerEngine>(engine);
                },
                anonymizerConfigCacheEntryOptions
            );
        }

        private async Task<ObjectResult> Anonymize(
            Resource resource,
            AnonymizerSettings anonymizerSettings,
            IAnonymizerEngine engine
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
                var anonymized = await engine.AnonymizeResourceAsync(resource, anonymizerSettings);
                provenancePublisher.Publish(resource, anonymized);
                return Ok(anonymized);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Anonymize failed");
                return StatusCode(500, GetInternalErrorOutcome(exc));
            }
        }

        /// <summary>
        ///     Revert any reversible de-identification methods previously applied to the given FHIR resource.
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
                return Ok(await dePseudonymizer.DePseudonymizeResourceAsync(resource));
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "DePseudonymize failed");
                return StatusCode(500, GetInternalErrorOutcome(exc));
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
                Format = ["application/fhir+json"],
                Rest = [new() { Mode = CapabilityStatement.RestfulCapabilityMode.Server }],
            };
        }

        private static OperationOutcome CreateBadRequestOutcome(string diagnostics)
        {
            var outcome = new OperationOutcome();
            outcome.Issue.Add(
                new OperationOutcome.IssueComponent
                {
                    Severity = OperationOutcome.IssueSeverity.Error,
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
}
