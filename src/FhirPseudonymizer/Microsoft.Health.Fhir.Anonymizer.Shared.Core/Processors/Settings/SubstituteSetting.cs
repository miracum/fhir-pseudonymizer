using System.Collections.Generic;
using EnsureThat;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings
{
    public class SubstituteSetting
    {
        public string ReplaceWith { get; set; }

        public static SubstituteSetting CreateFromRuleSettings(
            Dictionary<string, object> ruleSettings
        )
        {
            EnsureArg.IsNotNull(ruleSettings);

            var replaceWith = ruleSettings.GetValueOrDefault(RuleKeys.ReplaceWith)?.ToString();
            return new SubstituteSetting { ReplaceWith = replaceWith };
        }

        public static void ValidateRuleSettings(Dictionary<string, object> ruleSettings)
        {
            if (ruleSettings == null)
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Substitute rule should not be null."
                );
            }

            if (!ruleSettings.ContainsKey(Constants.PathKey))
            {
                throw new AnonymizerConfigurationErrorsException(
                    "Missing path in FHIR path rule config."
                );
            }

            if (!ruleSettings.ContainsKey(RuleKeys.ReplaceWith))
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"Missing replaceWith value in substitution rule at {ruleSettings[Constants.PathKey]}."
                );
            }
        }
    }
}
