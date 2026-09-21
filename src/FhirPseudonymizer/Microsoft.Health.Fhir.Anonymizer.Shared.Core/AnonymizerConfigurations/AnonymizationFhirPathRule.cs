using System.Text.RegularExpressions;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    public class AnonymizationFhirPathRule : AnonymizerRule
    {
        private static readonly Regex s_pathRegex = new Regex(
            @"^(?<resourceType>[A-Z][a-zA-Z]*)?(\.)?(?<expression>.*?)$"
        );

        public AnonymizationFhirPathRule(
            string path,
            string expression,
            string resourceType,
            string method,
            AnonymizerRuleType type,
            string source,
            Dictionary<string, object> settings = null
        )
            : base(path, method, type, source)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("expression");
            }

            Expression = expression;
            ResourceType = resourceType;
            RuleSettings = settings;

            // Method/Path are never mutated after construction (both setters exist only because
            // the base class exposes them); precomputing these here avoids redoing an
            // upper-casing allocation and a string comparison for every resource this rule is
            // evaluated against in AnonymizationVisitor's hot loop.
            MethodUpper = method.ToUpperInvariant();
            IsResourceTypeRule = path.Equals(resourceType);
        }

        public string Expression { get; set; }

        public string ResourceType { get; }

        public string MethodUpper { get; }

        public bool IsResourceTypeRule { get; }

        public static AnonymizationFhirPathRule CreateAnonymizationFhirPathRule(
            Dictionary<string, object> config
        )
        {
            ArgumentNullException.ThrowIfNull(config);

            if (!config.TryGetValue(Constants.PathKey, out var pathValue))
            {
                throw new ArgumentException("Missing path in rule config");
            }

            if (!config.TryGetValue(Constants.MethodKey, out var methodValue))
            {
                throw new ArgumentException("Missing method in rule config");
            }

            var path = pathValue.ToString();
            var method = methodValue.ToString();

            // Parse expression and resource type from path
            string resourceType = null;
            string expression = null;
            var match = s_pathRegex.Match(path);
            if (match.Success)
            {
                resourceType = match.Groups["resourceType"].Value;
                expression = match.Groups["expression"].Value;
            }

            if (string.IsNullOrEmpty(expression))
            {
                // For case: Path == "Resource"
                expression = path;
            }

            return new AnonymizationFhirPathRule(
                path,
                expression,
                resourceType,
                method,
                AnonymizerRuleType.FhirPathRule,
                path,
                config
            );
        }

        public AnonymizationFhirPathRule ShallowCopy()
        {
            return (AnonymizationFhirPathRule)this.MemberwiseClone();
        }
    }
}
