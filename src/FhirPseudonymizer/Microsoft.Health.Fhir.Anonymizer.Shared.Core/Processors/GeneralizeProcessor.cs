using System;
using System.Collections.Generic;
using EnsureThat;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class GeneralizeProcessor : IAnonymizerProcessor
    {
        public ProcessResult Process(
            ElementNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            EnsureArg.IsNotNull(node);
            EnsureArg.IsNotNull(context?.VisitedNodes);
            EnsureArg.IsNotNull(settings);

            var result = new ProcessResult();
            if (!ModelInfo.IsPrimitive(node.InstanceType) || node.Value == null)
            {
                return result;
            }

            var generalizeSetting = GeneralizeSetting.CreateFromRuleSettings(settings);
            foreach (var eachCase in generalizeSetting.Cases)
            {
                try
                {
                    if (node.Predicate(eachCase.Key))
                    {
                        node.Value = node.Scalar(eachCase.Value);
                        result.AddProcessRecord(AnonymizationOperations.Generalize, node);
                        return result;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new AnonymizerConfigurationErrorsException(
                        $"Invalid cases expression '{eachCase}': {ex.Message}",
                        ex
                    );
                }
            }

            if (generalizeSetting.OtherValues == GeneralizationOtherValuesOperation.Redact)
            {
                node.Value = null;
            }

            result.AddProcessRecord(AnonymizationOperations.Generalize, node);
            return result;
        }
    }
}
