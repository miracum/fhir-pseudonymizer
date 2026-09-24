using System.Text.RegularExpressions;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;

namespace Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations
{
    public partial class AnonymizationFhirPathRule : AnonymizerRule
    {
        private static readonly Regex s_pathRegex = MyRegex();

        private string _expression;
        private Lazy<CompiledExpression> _compiledExpression;

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
        }

        public string Expression
        {
            get => _expression;
            set
            {
                _expression = value;

                // Compiled on first use rather than here, so that the anonymizer's FHIRPath
                // extension functions (e.g. nodesByType) are sure to be registered by then.
                // Invalid expressions are still rejected at startup, by the config validator.
                _compiledExpression = new Lazy<CompiledExpression>(() =>
                    new FhirPathCompiler(FhirPathCompiler.DefaultSymbolTable).Compile(value)
                );
            }
        }

        public string ResourceType { get; }

        public bool IsResourceTypeRule => Path.Equals(ResourceType);

        /// <summary>
        ///     Evaluates <see cref="Expression" /> against <paramref name="node" />, exactly like
        ///     <c>node.Select(Expression)</c>, but compiled only once per rule. Firely's string-based
        ///     Select instead looks the compiled expression up in a process-wide cache on every
        ///     call, which takes a lock each time - serializing all concurrent anonymizations (e.g.
        ///     the Kafka consumer's workers) on every rule applied to every resource.
        /// </summary>
        public IEnumerable<ITypedElement> Evaluate(ITypedElement node)
        {
            return _compiledExpression.Value(node.ToScopedNode(), new EvaluationContext());
        }

        public static AnonymizationFhirPathRule CreateAnonymizationFhirPathRule(
            Dictionary<string, object> config
        )
        {
            ArgumentNullException.ThrowIfNull(config);

            if (!config.ContainsKey(Constants.PathKey))
            {
                throw new ArgumentException("Missing path in rule config");
            }

            if (!config.ContainsKey(Constants.MethodKey))
            {
                throw new ArgumentException("Missing method in rule config");
            }

            var path = config[Constants.PathKey].ToString();
            var method = config[Constants.MethodKey].ToString();

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

        [GeneratedRegex(@"^(?<resourceType>[A-Z][a-zA-Z]*)?(\.)?(?<expression>.*?)$")]
        private static partial Regex MyRegex();
    }
}
