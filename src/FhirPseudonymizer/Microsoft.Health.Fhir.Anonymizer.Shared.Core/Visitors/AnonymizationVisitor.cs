using System.Data;
using Hl7.Fhir.ElementModel;
using Hl7.FhirPath;
using Microsoft.Health.Fhir.Anonymizer.Core.AnonymizerConfigurations;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Visitors
{
    public class AnonymizationVisitor : AbstractElementNodeVisitor
    {
        private readonly Stack<Tuple<ElementNode, ProcessResult>> _contextStack = new();

        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizationVisitor>();
        private readonly Dictionary<string, IAnonymizerProcessor> _processors;
        private readonly AnonymizerSettings _settings;
        private readonly AnonymizationFhirPathRule[] _rules;

        // Held for the whole visit rather than passed through VisitAsync/EndVisitAsync: a visitor
        // is built fresh for each AnonymizeAsync call and is scoped to exactly that one run, and
        // EndVisitAsync has no use for it at all.
        private readonly CancellationToken _cancellationToken;
        private readonly HashSet<ElementNode> _visitedNodes = [];

        // _rules never changes for the lifetime of the visitor (one visitor per AnonymizeAsync
        // call), so the resource-specific-and-general rule set for a given resource type is
        // memoized here instead of being re-filtered out of the full rule array on every single
        // resource node visited (every Bundle entry, every contained resource, ...).
        private readonly Dictionary<string, AnonymizationFhirPathRule[]> _rulesByTypeCache = [];

        public AnonymizationVisitor(
            AnonymizationFhirPathRule[] rules,
            Dictionary<string, IAnonymizerProcessor> processors,
            AnonymizerSettings settings = null,
            CancellationToken cancellationToken = default
        )
        {
            _rules = rules;
            _processors = processors;
            _settings = settings;
            _cancellationToken = cancellationToken;

            if (settings is not null)
            {
                AddSecurityTag = _settings.ShouldAddSecurityTag;
            }
        }

        public bool AddSecurityTag { get; set; } = true;

        public override async Task<bool> VisitAsync(ElementNode node)
        {
            if (node.IsFhirResource())
            {
                var result = await ProcessResourceNodeAsync(node);
                _contextStack.Push(new Tuple<ElementNode, ProcessResult>(node, result));
            }

            return true;
        }

        public override Task EndVisitAsync(ElementNode node)
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

                if (_contextStack.Count > 0)
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

        private async Task<ProcessResult> ProcessResourceNodeAsync(ElementNode node)
        {
            var result = new ProcessResult();
            var typeString = node.InstanceType;
            var resourceSpecificAndGeneralRules = GetRulesByType(typeString);

            foreach (var rule in resourceSpecificAndGeneralRules)
            {
                // Checked before the FHIRPath evaluation below, which is the most expensive step
                // per (resource, rule) pair and has no cancellation of its own.
                _cancellationToken.ThrowIfCancellationRequested();

                var context = new ProcessContext
                {
                    VisitedNodes = _visitedNodes,
                    CancellationToken = _cancellationToken,
                };

                var resultOnRule = new ProcessResult();
                if (!_processors.TryGetValue(rule.MethodUpper, out var processor))
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
                    matchNodes = [node];
                }
                else
                {
                    // Materialized eagerly: a Remove processor mutates the tree by detaching the
                    // matched node from its parent's child list, which - for a rule matching
                    // multiple siblings (e.g. Bundle.entry.where(...)) - is the same list the
                    // FHIRPath query below lazily enumerates. Without ToList(), removing one match
                    // while a later match is still being lazily computed throws
                    // "Collection was modified; enumeration operation may not execute."
                    matchNodes = node.Select(rule.Expression).CastElementNodes().ToList();
                }

                foreach (var matchNode in matchNodes)
                {
                    resultOnRule.Update(
                        await ProcessNodeRecursiveAsync(
                            matchNode,
                            processor,
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
            if (_settings?.DynamicRuleSettings is not { Count: > 0 } dynamicRuleSettings)
            {
                return ruleSettings;
            }

            // overwrites existing settings
            var merged = ruleSettings is null ? [] : new Dictionary<string, object>(ruleSettings);

            foreach (var (key, value) in dynamicRuleSettings)
            {
                merged[key] = value;
            }

            return merged;
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

        private AnonymizationFhirPathRule[] GetRulesByType(string typeString)
        {
            if (_rulesByTypeCache.TryGetValue(typeString, out var cached))
            {
                return cached;
            }

            var rulesForType = _rules
                .Where(r =>
                    r.ResourceType.Equals(typeString)
                    || string.IsNullOrEmpty(r.ResourceType)
                    || string.Equals(Constants.GeneralResourceType, r.ResourceType)
                    || string.Equals(Constants.GeneralDomainResourceType, r.ResourceType)
                )
                .ToArray();

            _rulesByTypeCache[typeString] = rulesForType;
            return rulesForType;
        }

        public async Task<ProcessResult> ProcessNodeRecursiveAsync(
            ElementNode node,
            IAnonymizerProcessor processor,
            ProcessContext context,
            Dictionary<string, object> settings
        )
        {
            context.CancellationToken.ThrowIfCancellationRequested();

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
            foreach (var child in node.Children().CastElementNodes().ToList())
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
