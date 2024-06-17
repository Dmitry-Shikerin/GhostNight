using DungeonArchitect.Graphs;

namespace DungeonArchitect.MarkerGenerator.Nodes.Condition
{
    public class MarkerGenRuleNodeNot : MarkerGenRuleGraphNodeConditionBase
    {
        public override string Title => "NOT";

        protected override void CreateDefaultPins()
        {
            CreateInputPin("");
            CreateOutputPin("");
        }
    }
}