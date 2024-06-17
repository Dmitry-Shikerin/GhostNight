using System;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Rule
{
    public class MarkerGenRuleActionList : ScriptableObject
    {
        public MarkerGenRuleActionInfo[] actionList = Array.Empty<MarkerGenRuleActionInfo>();
        public MarkerGenRuleActionListHints hints = new MarkerGenRuleActionListHints();
        public void Clear()
        {
            actionList = Array.Empty<MarkerGenRuleActionInfo>();
            hints = new MarkerGenRuleActionListHints();
        }
    }

    [System.Serializable]
    public class MarkerGenRuleActionListHints
    {
        public bool emitsMarker = false;
    }
}