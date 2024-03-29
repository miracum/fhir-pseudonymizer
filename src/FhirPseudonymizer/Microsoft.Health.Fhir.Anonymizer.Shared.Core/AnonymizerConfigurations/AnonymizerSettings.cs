using System.Collections.Generic;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    public class AnonymizerSettings
    {
        public bool IsPrettyOutput { get; set; }

        public bool ValidateInput { get; set; }

        public bool ValidateOutput { get; set; }

        public bool ShouldAddSecurityTag { get; set; } = true;

        public Dictionary<string, object> DynamicRuleSettings { get; set; }
    }
}
