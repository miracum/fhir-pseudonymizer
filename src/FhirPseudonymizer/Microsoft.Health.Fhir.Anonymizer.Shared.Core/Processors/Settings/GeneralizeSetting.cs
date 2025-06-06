using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings
{
    public class GeneralizeSetting
    {
        public Dictionary<string, string> Cases { get; set; }
        public GeneralizationOtherValuesOperation OtherValues { get; set; }

        public static GeneralizeSetting CreateFromRuleSettings(
            Dictionary<string, object> ruleSettings
        )
        {
            EnsureArg.IsNotNull(ruleSettings);

            Dictionary<object, object> cases;
            try
            {
                cases = (Dictionary<object, object>)ruleSettings.GetValueOrDefault(RuleKeys.Cases);
            }
            catch (Exception ex)
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"Invalid cases '{RuleKeys.Cases}': {ex.Message}",
                    ex
                );
            }

            if (
                !Enum.TryParse(
                    ruleSettings.GetValueOrDefault(RuleKeys.OtherValues)?.ToString(),
                    true,
                    out GeneralizationOtherValuesOperation otherValues
                )
            )
            {
                otherValues = GeneralizationOtherValuesOperation.Redact;
            }

            return new GeneralizeSetting
            {
                OtherValues = otherValues,
                Cases = cases.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString()),
            };
        }

        public static void ValidateRuleSettings(Dictionary<string, object> ruleSettings)
        {
            var compiler = new FhirPathCompiler();
            if (ruleSettings == null)
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Generalize rule should not be null."
                );
            }

            if (!ruleSettings.ContainsKey(Constants.PathKey))
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Missing path in FHIR path rule config."
                );
            }

            if (!ruleSettings.ContainsKey(Constants.MethodKey))
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Missing method in FHIR path rule config."
                );
            }

            if (!ruleSettings.ContainsKey(RuleKeys.Cases))
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Missing cases in FHIR path rule config."
                );
            }

            try
            {
                var cases =
                    (Dictionary<object, object>)ruleSettings.GetValueOrDefault(RuleKeys.Cases);
                foreach (var eachCase in cases)
                {
                    compiler.Compile(eachCase.Key.ToString());
                    compiler.Compile(eachCase.Value.ToString());
                }
            }
            catch (JsonReaderException ex)
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"Invalid Json format {ruleSettings.GetValueOrDefault(RuleKeys.Cases)}",
                    ex
                );
            }
            catch (Exception ex)
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"Invalid cases expression {ruleSettings.GetValueOrDefault(RuleKeys.Cases)}",
                    ex
                );
            }

            var supportedOtherValuesOperations = Enum.GetNames(
                    typeof(GeneralizationOtherValuesOperation)
                )
                .ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            if (
                ruleSettings.ContainsKey(RuleKeys.OtherValues)
                && !supportedOtherValuesOperations.Contains(
                    ruleSettings[RuleKeys.OtherValues].ToString()
                )
            )
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"OtherValues setting is invalid at {ruleSettings[RuleKeys.OtherValues]}."
                );
            }
        }
    }
}
