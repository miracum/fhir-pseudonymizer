using EnsureThat;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Health.Fhir.Anonymizer.Core.Extensions;
using Microsoft.Health.Fhir.Anonymizer.Core.Models;
using Microsoft.Health.Fhir.Anonymizer.Core.Processors.Settings;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Health.Fhir.Anonymizer.Core.Processors
{
    public class SubstituteProcessor : IAnonymizerProcessor
    {
        private readonly FhirJsonDeserializer _parser = new FhirJsonDeserializer();

        public Task<ProcessResult> ProcessAsync(
            PocoNode node,
            ProcessContext context = null,
            Dictionary<string, object> settings = null
        )
        {
            EnsureArg.IsNotNull(node);
            EnsureArg.IsNotNull(context?.VisitedNodes);
            EnsureArg.IsNotNull(settings);

            var substituteSetting = SubstituteSetting.CreateFromRuleSettings(settings);
            PocoNode replacementNode;
            // Get replacementNode for substitution
            if (ModelInfo.IsPrimitive(node.GetInstanceType()))
            {
                // Handle replaceWith value of string
                replacementNode = GetPrimitiveNode(substituteSetting.ReplaceWith);
            }
            else
            {
                // Handle replaceWith value of json object
                var replacementNodeType = ModelInfo.GetTypeForFhirType(node.GetInstanceType());
                if (replacementNodeType == null)
                {
                    // Shall never throws here
                    throw new Exception($"Node type is invalid at path {node.GetFhirPath()}.");
                }

                // Convert null object to empty object
                var replaceWith = substituteSetting.ReplaceWith ?? "{}";
                var replacementPoco = (Base)
                    _parser.DeserializeObject(replacementNodeType, replaceWith);
                replacementNode = PocoNodeExtension.CreateRootNode(replacementPoco);
            }

            var keepNodes = new HashSet<PocoNode>(PocoNodeIdentityComparer.Instance);
            // Retrieve all nodes that have been processed before to keep
            _ = GenerateKeepNodeSetForSubstitution(node, context.VisitedNodes, keepNodes);
            var processResult = SubstituteNode(
                node,
                replacementNode,
                context.VisitedNodes,
                keepNodes
            );
            MarkSubstitutedFragmentAsVisited(node, context.VisitedNodes);

            return System.Threading.Tasks.Task.FromResult(processResult);
        }

        private ProcessResult SubstituteNode(
            PocoNode node,
            PocoNode replacementNode,
            HashSet<PocoNode> visitedNodes,
            HashSet<PocoNode> keepNodes
        )
        {
            var processResult = new ProcessResult();
            if (node == null || replacementNode == null || visitedNodes.Contains(node))
            {
                return processResult;
            }

            // children names to replace, multiple to multiple replacement
            var replaceChildrenNames = replacementNode
                .Children()
                .CastPocoNodes()
                .Select(element => element.Name)
                .ToHashSet();
            foreach (var name in replaceChildrenNames)
            {
                var children = node.ChildrenByName(name).ToList();
                var targetChildren = replacementNode.ChildrenByName(name).ToList();

                var i = 0;
                foreach (var child in children)
                {
                    if (visitedNodes.Contains(child))
                    {
                        // Skip replacement if child already processed before.
                        i++;
                    }
                    else if (i < targetChildren.Count)
                    {
                        // We still have target nodes, do replacement
                        SubstituteNode(child, targetChildren[i++], visitedNodes, keepNodes);
                    }
                    else if (keepNodes.Contains(child))
                    {
                        // Substitute with an empty node when no target node available but we need to keep this node
                        SubstituteNode(child, GetDummyNode(), visitedNodes, keepNodes);
                    }
                    else
                    {
                        // Remove source node when no target node available and we don't need to keep the source node
                        node.RemoveChild(child);
                    }
                }

                while (i < targetChildren.Count)
                {
                    // Add extra target nodes - borrow the POCO straight out of the (otherwise
                    // disconnected) replacement template and splice it into the live tree; the
                    // template isn't used for anything else afterward, so aliasing it here is safe.
                    node.AddChild(name, targetChildren[i++].Poco);
                }
            }

            // children nodes not presented in replacement value, we need either remove or keep a dummy copy
            var nonReplacementChildren = node.Children()
                .CastPocoNodes()
                .Where(element => !replaceChildrenNames.Contains(element.Name))
                .ToList();
            foreach (var child in nonReplacementChildren)
            {
                if (visitedNodes.Contains(child)) { }
                else if (keepNodes.Contains(child))
                {
                    SubstituteNode(child, GetDummyNode(), visitedNodes, keepNodes);
                }
                else
                {
                    node.RemoveChild(child);
                }
            }

            // Only primitive leaves carry a settable value; composite nodes' Poco isn't a
            // PrimitiveType, so this mirrors the old ElementNode.Value setter's no-op behaviour
            // for them instead of an invalid cast.
            if (node.Poco is PrimitiveType)
            {
                node.SetPrimitiveValue(replacementNode.GetValue());
            }

            processResult.AddProcessRecord(AnonymizationOperations.Substitute, node);
            return processResult;
        }

        // To keep consistent anonymization changes made by preceding rules, we should figure out whether a node can be removed during substitution
        private bool GenerateKeepNodeSetForSubstitution(
            PocoNode node,
            HashSet<PocoNode> visitedNodes,
            HashSet<PocoNode> keepNodes
        )
        {
            var shouldKeep = false;
            // If a child (no matter how deep) has been modified, this node should be kept
            foreach (var child in node.Children().CastPocoNodes())
            {
                shouldKeep |= GenerateKeepNodeSetForSubstitution(child, visitedNodes, keepNodes);
            }

            // If this node its self has been modified, it should be kept
            if (shouldKeep || visitedNodes.Contains(node))
            {
                keepNodes.Add(node);
                return true;
            }

            return shouldKeep;
        }

        // Post-process to mark all substituted children nodes as visited
        private void MarkSubstitutedFragmentAsVisited(PocoNode node, HashSet<PocoNode> visitedNodes)
        {
            visitedNodes.Add(node);
            foreach (var child in node.Children().CastPocoNodes())
            {
                MarkSubstitutedFragmentAsVisited(child, visitedNodes);
            }
        }

        private PocoNode GetPrimitiveNode(string value)
        {
            var node = PocoNode.ForAnyPrimitive(value ?? string.Empty);
            if (value == null)
            {
                // Set empty node value to null to ensure a correct serialization result
                node.SetPrimitiveValue(null);
            }

            return node;
        }

        private PocoNode GetDummyNode()
        {
            var dummy = GetPrimitiveNode(null);
            return dummy;
        }
    }
}
