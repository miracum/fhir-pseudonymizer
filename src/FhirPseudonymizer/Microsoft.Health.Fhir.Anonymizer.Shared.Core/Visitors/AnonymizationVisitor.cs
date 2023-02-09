using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Visitors
{
    public class AnonymizationVisitor : AbstractElementNodeVisitor
    {
        private readonly Stack<Tuple<ElementNode, ProcessResult>> _contextStack =
            new Stack<Tuple<ElementNode, ProcessResult>>();

        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizationVisitor>();
        private readonly Dictionary<string, IAnonymizerProcessor> _processors;
        private readonly AnonymizerSettings _settings;
        private readonly AnonymizationFhirPathRule[] _rules;
        private readonly HashSet<ElementNode> _visitedNodes = new HashSet<ElementNode>();

        public AnonymizationVisitor(
            AnonymizationFhirPathRule[] rules,
            Dictionary<string, IAnonymizerProcessor> processors,
            AnonymizerSettings settings = null
        )
        {
            _rules = rules;
            _processors = processors;
            _settings = settings;
        }

        public bool AddSecurityTag { get; set; } = true;

        public override bool Visit(ElementNode node)
        {
            if (node.IsFhirResource())
            {
                var result = ProcessResourceNode(node);
                _contextStack.Push(new Tuple<ElementNode, ProcessResult>(node, result));
            }

            return true;
        }

        public override void EndVisit(ElementNode node)
        {
            if (node.IsFhirResource())
            {
                var context = _contextStack.Pop();
                var result = context.Item2;

                if (context.Item1 != node)
                {
                    // Should never throw exception here. In case any bug happen, we can get clear message for this exception.
                    throw new ConstraintException("Internal error: access wrong context.");
                }

                if (_contextStack.Count() > 0)
                {
                    _contextStack.Peek().Item2.Update(result);
                }

                if (AddSecurityTag && !node.IsContainedNode())
                {
                    node.AddSecurityTag(result);
                }
            }
        }

        private ProcessResult ProcessResourceNode(ElementNode node)
        {
            var result = new ProcessResult();
            var typeString = node.InstanceType;
            var resourceSpecificAndGeneralRules = GetRulesByType(typeString);

            foreach (var rule in resourceSpecificAndGeneralRules)
            {
                var context = new ProcessContext { VisitedNodes = _visitedNodes };

                var resultOnRule = new ProcessResult();
                var method = rule.Method.ToUpperInvariant();
                if (!_processors.ContainsKey(method))
                {
                    continue;
                }

                IEnumerable<ElementNode> matchNodes;
                if (rule.IsResourceTypeRule)
                {
                    /*
                     * Special case handling:
                     * Senario: FHIR path only contains resourceType: Patient, Resource.
                     * Sample AnonymizationFhirPathRule: { "path": "Patient", "method": "keep" }
                     *
                     * Current FHIR path lib do not support navigate such ResourceType FHIR path from resource in bundle.
                     * Example: navigate with FHIR path "Patient" from "Bundle.entry[0].resource[0]" is not support
                     */
                    matchNodes = new List<ElementNode> { node };
                }
                else
                {
                    matchNodes = node.Select(rule.Expression).CastElementNodes();
                }

                foreach (var matchNode in matchNodes)
                {
                    resultOnRule.Update(
                        ProcessNodeRecursive(
                            matchNode,
                            _processors[method],
                            context,
                            MergeSettings(rule.RuleSettings)
                        )
                    );
                }

                LogProcessResult(node, rule, resultOnRule);

                result.Update(resultOnRule);
            }

            return result;
        }

        private Dictionary<string, object> MergeSettings(Dictionary<string, object> ruleSettings)
        {
            if (_settings?.DynamicRuleSettings?.Any() != true)
            {
                return ruleSettings;
            }

            // overwrites existing settings
            return ImmutableArray
                .Create(ruleSettings, _settings.DynamicRuleSettings)
                .SelectMany(dict => dict)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.Last());
        }

        private void LogProcessResult(
            ElementNode node,
            AnonymizationFhirPathRule rule,
            ProcessResult resultOnRule
        )
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var resourceId = node.GetNodeId();
                foreach (var processRecord in resultOnRule.ProcessRecords)
                {
                    foreach (var matchNode in processRecord.Value)
                    {
                        _logger.LogDebug(
                            $"[{resourceId}]: Rule '{rule.Path}' matches '{matchNode.Location}' and perform operation '{processRecord.Key}'"
                        );
                    }
                }
            }
        }

        private IEnumerable<AnonymizationFhirPathRule> GetRulesByType(string typeString)
        {
            return _rules.Where(
                r =>
                    r.ResourceType.Equals(typeString)
                    || string.IsNullOrEmpty(r.ResourceType)
                    || string.Equals(Constants.GeneralResourceType, r.ResourceType)
                    || string.Equals(Constants.GeneralDomainResourceType, r.ResourceType)
            );
        }

        public ProcessResult ProcessNodeRecursive(
            ElementNode node,
            IAnonymizerProcessor processor,
            ProcessContext context,
            Dictionary<string, object> settings
        )
        {
            var result = new ProcessResult();
            if (_visitedNodes.Contains(node))
            {
                return result;
            }

            result = processor.Process(node, context, settings);
            _visitedNodes.Add(node);

            foreach (var child in node.Children().CastElementNodes())
            {
                if (child.IsFhirResource())
                {
                    continue;
                }

                result.Update(ProcessNodeRecursive(child, processor, context, settings));
            }

            return result;
        }
    }
}
