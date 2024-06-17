using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Pins;
using DungeonArchitect.MarkerGenerator.Rule;

namespace DungeonArchitect.MarkerGenerator.Nodes.Actions.Info
{
    public static class ActionGraphCompiler
    {
        public static void Compile(MarkerGenRuleGraph ruleGraph, MarkerGenRuleActionList actions)
        {
            if (ruleGraph == null || actions == null)
            {
                return;
            }
            
            actions.Clear();
            var actionsInfoList = new List<MarkerGenRuleActionInfo>();

            var visited = new HashSet<MarkerGenRuleGraphNodeActionBase>();
            var outgoingPins = BuildOutgoingPinMap(ruleGraph);
            var node = GetOutgoingNode(ruleGraph.passNode, outgoingPins);
            while (node != null && !visited.Contains(node))
            {
                var actionInfo = node.CreateActionInfo();
                if (actionInfo != null)
                {
                    actionsInfoList.Add(actionInfo);
                }
                visited.Add(node);
                node = GetOutgoingNode(node, outgoingPins);
            }

            actions.actionList = actionsInfoList.ToArray();
            
            foreach (var actionInfo in actions.actionList)
            {
                if (actionInfo is MarkerGenRuleActionInfoAddMarker)
                {
                    actions.hints.emitsMarker = true;
                }
            }
        }

        private static MarkerGenRuleGraphNodeActionBase GetOutgoingNode(MarkerGenRuleGraphNodeActionBase node, Dictionary<GraphPin, GraphPin> outgoingPins)
        {
            var outputPin = node.OutputPin;
            if (!outgoingPins.ContainsKey(outputPin))
            {
                return null;
            }

            var linkedToPin = outgoingPins[outputPin];
            return linkedToPin.Node as MarkerGenRuleGraphNodeActionBase;
        }
        
        
        private static Dictionary<GraphPin, GraphPin> BuildOutgoingPinMap(Graph graph)
        {
            var outgoingPins = new Dictionary<GraphPin, GraphPin>();
            foreach (var link in graph.Links)
            {
                var input = link.Input as MarkerGenRuleGraphPin;
                var output = link.Output as MarkerGenRuleGraphPin;
                if (input == null || output == null) continue;
                
                outgoingPins[output] = input;
            }

            return outgoingPins;
        }
    }
}