using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Nodes.Actions
{
    public class MarkerGenRuleNodeRemoveMarker: MarkerGenRuleGraphNodeActionBase
    {
        public string markerName = "";
        public override string Title => "Remove Marker: " + (string.IsNullOrEmpty(markerName) ? "<NONE>" : markerName);

        public override MarkerGenRuleActionInfo CreateActionInfo()
        {
            var actionInfo = CreateInstance<MarkerGenRuleActionInfoRemoveMarker>();
            actionInfo.markerName = markerName;
            return actionInfo;
        }
    }
}