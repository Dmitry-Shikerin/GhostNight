using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using DungeonArchitect.MarkerGenerator.Pins;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Nodes
{
    public abstract class MarkerGenRuleGraphNodeActionBase : MarkerGenRuleGraphNode
    {
        public override Color BodyColor => new Color(0.1f, 0.1f, 0.2f);
        public override Color TitleColor => new Color(0.1f, 0.2f, 0.3f);
        
        protected override void CreateDefaultPins()
        {
            CreateInputPin("");
            CreateOutputPin("");
        }

        protected void CreateInputPin(string pinName)
        {
            var pin = CreatePinOfType<MarkerGenRuleGraphPinExec>(GraphPinType.Input);
            pin.text = pinName;
        }
        
        protected void CreateOutputPin(string pinName)
        {
            var pin = CreatePinOfType<MarkerGenRuleGraphPinExec>(GraphPinType.Output);
            pin.text = pinName;
        }

        public virtual MarkerGenRuleActionInfo CreateActionInfo()
        {
            return null;
        }
    }
}