using System;
using System.Collections.Generic;
using System.Globalization;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core;
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
        private static readonly Histogram BundleSizeHistogram = Metrics
            .CreateHistogram("fhirpseudonymizer_received_bundle_size", "Histogram of received bundle sizes.",
        new HistogramConfiguration
        {
            // we divide measurements in 10 buckets of 5 each, up to 50.
            Buckets = Histogram.LinearBuckets(start: 1, width: 5, count: 20),
            LabelNames = new[] { "operation" },
        });

        private readonly IAnonymizerEngine anonymizer;
        private readonly IConfiguration config;
        private readonly IDePseudonymizerEngine dePseudonymizer;
        private readonly ILogger<FhirController> logger;

        public FhirController(IConfiguration config, ILogger<FhirController> logger, IAnonymizerEngine anonymizer,
            IDePseudonymizerEngine dePseudonymizer)
        {
            this.config = config;
            this.logger = logger;
            this.anonymizer = anonymizer;
            this.dePseudonymizer = dePseudonymizer;
        }

        /// <summary>
        ///     Apply de-identification rules to the given FHIR resource. The rules can be configured using the anonymization.yaml
        ///     config file.
        /// </summary>
        /// <param name="resource">
        ///     The FHIR resource to be de-identified. If the resource is of type 'Parameters' then the input is
        ///     fetched from the parameter named 'resource'.
        /// </param>
        /// <returns>The de-identified resource.</returns>
        /// <response code="200">Returns the de-identified resource</response>
        [HttpPost("$de-identify")]
        [AllowAnonymous]
        public Resource DeIdentify([FromBody] Resource resource)
        {
            if (resource == null)
            {
                logger.LogWarning("Bad Request: received request body is empty.");
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequestOutcome;
            }

            logger.LogDebug("De-Identifying resource {resourceType}/{resourceId}",
                resource.TypeName, resource.Id);

            if (resource is Parameters param)
            {
                return Anonymize(param.GetSingle("resource").Resource);
            }

            return Anonymize(resource);
        }

        private Resource Anonymize(Resource resource)
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
                return anonymizer.AnonymizeResource(resource);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "Anonymize failed");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return GetInternalErrorOutcome(exc);
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
        public Resource DePseudonymize([FromBody] Resource resource)
        {
            if (resource == null)
            {
                logger.LogWarning("Bad Request: received request body is empty.");
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return BadRequestOutcome;
            }

            logger.LogDebug("De-Pseudonymizing resource {resourceType}/{resourceId}",
                resource.TypeName, resource.Id);

            if (resource is Bundle bundle)
            {
                BundleSizeHistogram.WithLabels(nameof(DePseudonymize)).Observe(bundle.Entry.Count);
            }

            try
            {
                return dePseudonymizer.DePseudonymizeResource(resource);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, "DePseudonymize failed");
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return GetInternalErrorOutcome(exc);
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
                    Version = config.GetValue<string>("App:Version")
                },
                FhirVersion = FHIRVersion.N4_0_1,
                Format = new[] { "application/fhir+json" },
                Rest = new List<CapabilityStatement.RestComponent>
                {
                    new () {Mode = CapabilityStatement.RestfulCapabilityMode.Server}
                }
            };
        }

        private static OperationOutcome BadRequestOutcome
        {
            get
            {
                var outcome = new OperationOutcome();
                outcome.Issue.Add(new OperationOutcome.IssueComponent
                {
                    Severity = OperationOutcome.IssueSeverity.Error,
                    Code = OperationOutcome.IssueType.Processing,
                    Diagnostics = "Received malformed or missing resource"
                });
                return outcome;
            }
        }

        private static OperationOutcome GetInternalErrorOutcome(Exception exc)
        {
            var outcome = new OperationOutcome();
            outcome.Issue.Add(new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Fatal,
                Code = OperationOutcome.IssueType.Processing,
                Diagnostics =
                    $"An internal error occurred when processing the request: {exc.Message}.\nAt: {exc.StackTrace}"
            });
            return outcome;
        }
    }
}
