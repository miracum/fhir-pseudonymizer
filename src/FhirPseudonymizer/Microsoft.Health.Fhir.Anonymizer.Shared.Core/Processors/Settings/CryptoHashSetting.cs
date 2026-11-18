using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings
{
    public enum CryptoHashOutputFormat
    {
        /// <summary>The lowercase hex digest of the hash (the default).</summary>
        Hex,

        /// <summary>A version 8 (custom) UUID built from the first 16 bytes of the hash.</summary>
        Uuid,
    }

    public static class CryptoHashSetting
    {
        public static CryptoHashOutputFormat GetOutputFormat(
            IReadOnlyDictionary<string, object> ruleSettings
        )
        {
            if (ruleSettings?.TryGetValue(RuleKeys.OutputFormat, out var value) != true)
            {
                return CryptoHashOutputFormat.Hex;
            }

            return TryParseOutputFormat(value, out var format)
                ? format
                : throw new AnonymizerConfigurationErrorsException(
                    UnsupportedOutputFormatMessage(value, ruleSettings)
                );
        }

        public static void ValidateRuleSettings(Dictionary<string, object> ruleSettings)
        {
            if (ruleSettings?.TryGetValue(RuleKeys.OutputFormat, out var value) != true)
            {
                return;
            }

            if (!TryParseOutputFormat(value, out var format))
            {
                throw new AnonymizerConfigurationErrorsException(
                    UnsupportedOutputFormatMessage(value, ruleSettings)
                );
            }

            if (
                format == CryptoHashOutputFormat.Uuid
                && ruleSettings.ContainsKey(RuleKeys.TruncateToMaxLength)
            )
            {
                throw new AnonymizerConfigurationErrorsException(
                    $"{RuleKeys.TruncateToMaxLength} can't be combined with {RuleKeys.OutputFormat}: uuid "
                        + $"in cryptoHash rule at {ruleSettings.GetValueOrDefault(Constants.PathKey)}: "
                        + "a truncated UUID is no longer a valid UUID."
                );
            }
        }

        private static bool TryParseOutputFormat(object value, out CryptoHashOutputFormat format)
        {
            switch (value?.ToString()?.Trim().ToLowerInvariant())
            {
                case "hex":
                    format = CryptoHashOutputFormat.Hex;
                    return true;
                case "uuid":
                    format = CryptoHashOutputFormat.Uuid;
                    return true;
                default:
                    format = default;
                    return false;
            }
        }

        private static string UnsupportedOutputFormatMessage(
            object value,
            IReadOnlyDictionary<string, object> ruleSettings
        ) =>
            $"Unsupported {RuleKeys.OutputFormat} '{value}' in cryptoHash rule at "
            + $"{ruleSettings.GetValueOrDefault(Constants.PathKey)}. Supported values: hex, uuid.";
    }
}
