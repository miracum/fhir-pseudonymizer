using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class DateShiftProcessor : IAnonymizerProcessor
    {
        public DateShiftProcessor(
            string dateShiftKey,
            string dateShiftKeyPrefix,
            bool enablePartialDatesForRedact
        )
        {
            DateShiftKey = dateShiftKey;
            DateShiftKeyPrefix = dateShiftKeyPrefix;
            EnablePartialDatesForRedact = enablePartialDatesForRedact;
        }

        public string DateShiftKey { get; set; } = string.Empty;

        public string DateShiftKeyPrefix { get; set; } = string.Empty;

        public bool EnablePartialDatesForRedact { get; set; }

        public ProcessResult Process(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
            {
                return processResult;
            }

            var fixedOffsetInDays = ExtractFixedOffsetInDays(settings);

            if (node.IsDateNode())
            {
                return DateTimeUtility.ShiftDateNode(
                    node,
                    DateShiftKey,
                    DateShiftKeyPrefix,
                    EnablePartialDatesForRedact,
                    fixedOffsetInDays
                );
            }

            if (node.IsDateTimeNode() || node.IsInstantNode())
            {
                return DateTimeUtility.ShiftDateTimeAndInstantNode(
                    node,
                    DateShiftKey,
                    DateShiftKeyPrefix,
                    EnablePartialDatesForRedact,
                    fixedOffsetInDays
                );
            }

            return processResult;
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
                parameters.EnablePartialDatesForRedact
            );
        }
    }
}
