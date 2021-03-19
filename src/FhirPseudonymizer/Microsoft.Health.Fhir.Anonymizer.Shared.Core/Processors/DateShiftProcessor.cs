using System.Collections.Generic;
using Hl7.Fhir.ElementModel;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Utility;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class DateShiftProcessor : IAnonymizerProcessor
    {
        public DateShiftProcessor(string dateShiftKey, string dateShiftKeyPrefix, bool enablePartialDatesForRedact)
        {
            DateShiftKey = dateShiftKey;
            DateShiftKeyPrefix = dateShiftKeyPrefix;
            EnablePartialDatesForRedact = enablePartialDatesForRedact;
        }

        public string DateShiftKey { get; set; } = string.Empty;

        public string DateShiftKeyPrefix { get; set; } = string.Empty;

        public bool EnablePartialDatesForRedact { get; set; }

        public ProcessResult Process(ElementNode node, ProcessContext context = null,
            Dictionary<string, object> settings = null)
        {
            var processResult = new ProcessResult();
            if (string.IsNullOrEmpty(node?.Value?.ToString()))
            {
                return processResult;
            }

            if (node.IsDateNode())
            {
                return DateTimeUtility.ShiftDateNode(node, DateShiftKey, DateShiftKeyPrefix,
                    EnablePartialDatesForRedact);
            }

            if (node.IsDateTimeNode() || node.IsInstantNode())
            {
                return DateTimeUtility.ShiftDateTimeAndInstantNode(node, DateShiftKey, DateShiftKeyPrefix,
                    EnablePartialDatesForRedact);
            }

            return processResult;
        }

        public static DateShiftProcessor Create(AnonymizerConfigurationManager configuratonManager)
        {
            var parameters = configuratonManager.GetParameterConfiguration();
            return new DateShiftProcessor(parameters.DateShiftKey, parameters.DateShiftKeyPrefix,
                parameters.EnablePartialDatesForRedact);
        }
    }
}
