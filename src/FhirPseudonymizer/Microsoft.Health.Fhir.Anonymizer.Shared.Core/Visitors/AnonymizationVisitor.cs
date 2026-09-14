using System.Collections.Immutable;
using System.Data;
using Hl7.Fhir.Model;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Visitors
{
    public class AnonymizationVisitor : AbstractPocoNodeVisitor
    {
        private readonly Stack<Tuple<PocoNode, ProcessResult>> _contextStack =
            new Stack<Tuple<PocoNode, ProcessResult>>();

        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizationVisitor>();
        private readonly Dictionary<string, IAnonymizerProcessor> _processors;
        private readonly AnonymizerSettings _settings;
        private readonly AnonymizationFhirPathRule[] _rules;
        private readonly HashSet<PocoNode> _visitedNodes = new HashSet<PocoNode>(
            PocoNodeIdentityComparer.Instance
        );

        public AnonymizationVisitor(
            AnonymizationFhirPathRule[] rules,
            Dictionary<string, IAnonymizerProcessor> processors,
            AnonymizerSettings settings = null
        )
        {
            _rules = rules;
            _processors = processors;
            _settings = settings;

            if (settings is not null)
            {
                AddSecurityTag = _settings.ShouldAddSecurityTag;
            }
        }

        public bool AddSecurityTag { get; set; } = true;

        public override async Task<bool> VisitAsync(PocoNode node)
        {
            if (node.IsFhirResource())
            {
                var result = await ProcessResourceNodeAsync(node);
                _contextStack.Push(new Tuple<PocoNode, ProcessResult>(node, result));
            }

            return true;
        }

        public override Task EndVisitAsync(PocoNode node)
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

            return Task.CompletedTask;
        }

        private async Task<ProcessResult> ProcessResourceNodeAsync(PocoNode node)
        {
            var result = new ProcessResult();
            var typeString = node.GetInstanceType();
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

                IEnumerable<PocoNode> matchNodes;
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
                    matchNodes = new List<PocoNode> { node };
                }
                else
                {
                    // Materialized eagerly: a Remove processor mutates the tree by detaching the
                    // matched node from its parent's child list, which - for a rule matching
                    // multiple siblings (e.g. Bundle.entry.where(...)) - is the same list the
                    // FHIRPath query below lazily enumerates. Without ToList(), removing one match
                    // while a later match is still being lazily computed throws
                    // "Collection was modified; enumeration operation may not execute."
                    matchNodes = node.Select(rule.Expression).CastPocoNodes().ToList();
                }

                foreach (var matchNode in matchNodes)
                {
                    resultOnRule.Update(
                        await ProcessNodeRecursiveAsync(
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
            PocoNode node,
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
            return _rules.Where(r =>
                r.ResourceType.Equals(typeString)
                || string.IsNullOrEmpty(r.ResourceType)
                || string.Equals(Constants.GeneralResourceType, r.ResourceType)
                || string.Equals(Constants.GeneralDomainResourceType, r.ResourceType)
            );
        }

        public async Task<ProcessResult> ProcessNodeRecursiveAsync(
            PocoNode node,
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

            result = await processor.ProcessAsync(node, context, settings);
            _visitedNodes.Add(node);

            // Materialized eagerly for the same reason as the top-level match list in
            // ProcessResourceNodeAsync: if `node` has multiple non-resource children that all
            // match this rule (e.g. a removed Bundle.entry's "fullUrl" and "request"), a Remove
            // processor detaches each one from `node`'s own child list as it's visited, which
            // would otherwise invalidate this same enumeration mid-loop.
            foreach (var child in node.Children().CastPocoNodes().ToList())
            {
                if (child.IsFhirResource())
                {
                    continue;
                }

                result.Update(await ProcessNodeRecursiveAsync(child, processor, context, settings));
            }

            return result;
        }
    }
}
