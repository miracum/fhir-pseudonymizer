using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class DateShiftProcessor : IAnonymizerProcessor
    {
        public DateShiftProcessor(
            string dateShiftKey,
            string dateShiftKeyPrefix,
            bool enablePartialDatesForRedact,
            int? dateShiftFixedOffsetInDays = null
        )
        {
            DateShiftKey = dateShiftKey;
            DateShiftKeyPrefix = dateShiftKeyPrefix;
            EnablePartialDatesForRedact = enablePartialDatesForRedact;
            DateShiftFixedOffsetInDays = dateShiftFixedOffsetInDays;
        }

        public string DateShiftKey { get; set; } = string.Empty;

        public string DateShiftKeyPrefix { get; set; } = string.Empty;

        public bool EnablePartialDatesForRedact { get; set; }

        public int? DateShiftFixedOffsetInDays { get; set; }

        public Task<ProcessResult> ProcessAsync(
            PocoNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.GetValue()?.ToString()))
            {
                return System.Threading.Tasks.Task.FromResult(processResult);
            }

            var fixedOffsetInDays =
                ExtractFixedOffsetInDays(settings) ?? DateShiftFixedOffsetInDays;

            if (node.IsDateNode())
            {
                return System.Threading.Tasks.Task.FromResult(
                    DateTimeUtility.ShiftDateNode(
                        node,
                        DateShiftKey,
                        DateShiftKeyPrefix,
                        EnablePartialDatesForRedact,
                        fixedOffsetInDays
                    )
                );
            }

            if (node.IsDateTimeNode() || node.IsInstantNode())
            {
                return System.Threading.Tasks.Task.FromResult(
                    DateTimeUtility.ShiftDateTimeAndInstantNode(
                        node,
                        DateShiftKey,
                        DateShiftKeyPrefix,
                        EnablePartialDatesForRedact,
                        fixedOffsetInDays
                    )
                );
            }

            return System.Threading.Tasks.Task.FromResult(processResult);
        }

        private static int? ExtractFixedOffsetInDays(Dictionary<string, object> settings)
        {
            var fixedOffsetValue = settings?.GetValueOrDefault("dateShiftFixedOffsetInDays", null);

            if (fixedOffsetValue is Integer fhirInt)
            {
                return fhirInt.Value;
            }

            if (fixedOffsetValue is int intValue)
            {
                return intValue;
            }

            return null;
        }

        public static DateShiftProcessor Create(AnonymizerConfigurationManager configuratonManager)
        {
            var parameters = configuratonManager.GetParameterConfiguration();
            return new DateShiftProcessor(
                parameters.DateShiftKey,
                parameters.DateShiftKeyPrefix,
                parameters.EnablePartialDatesForRedact,
                parameters.DateShiftFixedOffsetInDays
            );
        }
    }
}
