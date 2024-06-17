using System;
using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using UnityEngine;
using UnityEngine.Jobs;

namespace DungeonArchitect.MarkerGenerator.Nodes.Actions
{
    public class MarkerGenRuleNodeAddMarker: MarkerGenRuleGraphNodeActionBase
    {
        public string markerName = "";
        
        [Tooltip(@"Copy the rotation from one of the markers found in this list")]
        public string[] copyRotationFromMarkers = Array.Empty<string>();
	
        [Tooltip(@"Copy the height from one of the markers found in this list")]
        public string[] copyHeightFromMarkers = Array.Empty<string>();
        
        public override string Title => "Add Marker: " + (string.IsNullOrEmpty(markerName) ? "<NONE>" : markerName);
        
        public override MarkerGenRuleActionInfo CreateActionInfo()
        {
            var actionInfo = CreateInstance<MarkerGenRuleActionInfoAddMarker>();
            actionInfo.markerName = markerName;
            actionInfo.copyRotationFromMarkers = new List<string>(copyRotationFromMarkers).ToArray();
            actionInfo.copyHeightFromMarkers = new List<string>(copyHeightFromMarkers).ToArray();
            return actionInfo;
        }
    }
}
