using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class RedactProcessor : IAnonymizerProcessor
    {
        public RedactProcessor(
            bool enablePartialDatesForRedact,
            bool enablePartialAgesForRedact,
            bool enablePartialZipCodesForRedact,
            List<string> restrictedZipCodeTabulationAreas
        )
        {
            EnablePartialDatesForRedact = enablePartialDatesForRedact;
            EnablePartialAgesForRedact = enablePartialAgesForRedact;
            EnablePartialZipCodesForRedact = enablePartialZipCodesForRedact;
            RestrictedZipCodeTabulationAreas = restrictedZipCodeTabulationAreas;
        }

        public bool EnablePartialDatesForRedact { get; set; }

        public bool EnablePartialAgesForRedact { get; set; }

        public bool EnablePartialZipCodesForRedact { get; set; }

        public List<string> RestrictedZipCodeTabulationAreas { get; set; }

        public Task<ProcessResult> ProcessAsync(
            PocoNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            if (string.IsNullOrEmpty(node?.GetValue()?.ToString()))
            {
                return Task.FromResult(new ProcessResult());
            }

            if (node.IsDateNode())
            {
                return Task.FromResult(
                    DateTimeUtility.RedactDateNode(node, EnablePartialDatesForRedact)
                );
            }

            if (node.IsDateTimeNode() || node.IsInstantNode())
            {
                return Task.FromResult(
                    DateTimeUtility.RedactDateTimeAndInstantNode(node, EnablePartialDatesForRedact)
                );
            }

            if (node.IsAgeDecimalNode())
            {
                return Task.FromResult(
                    DateTimeUtility.RedactAgeDecimalNode(node, EnablePartialAgesForRedact)
                );
            }

            if (node.IsPostalCodeNode())
            {
                return Task.FromResult(
                    PostalCodeUtility.RedactPostalCode(
                        node,
                        EnablePartialZipCodesForRedact,
                        RestrictedZipCodeTabulationAreas
                    )
                );
            }

            node.SetPrimitiveValue(null);
            var result = new ProcessResult();
            result.AddProcessRecord(AnonymizationOperations.Redact, node);
            return Task.FromResult(result);
        }

        public static RedactProcessor Create(AnonymizerConfigurationManager configuratonManager)
        {
            var parameters = configuratonManager.GetParameterConfiguration();
            return new RedactProcessor(
                parameters.EnablePartialDatesForRedact,
                parameters.EnablePartialAgesForRedact,
                parameters.EnablePartialZipCodesForRedact,
                parameters.RestrictedZipCodeTabulationAreas
            );
        }
    }
}
