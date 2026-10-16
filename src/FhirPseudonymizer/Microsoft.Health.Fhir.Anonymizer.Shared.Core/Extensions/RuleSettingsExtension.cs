using System.Globalization;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Extensions;

internal static class RuleSettingsExtension
{
    internal static bool IsTransformWholeUriEnabled(
        this IReadOnlyDictionary<string, object> settings
    )
    {
        return settings?.TryGetValue(RuleKeys.TransformWholeUri, out var value) == true
            && Convert.ToBoolean(value, CultureInfo.InvariantCulture);
    }
}
