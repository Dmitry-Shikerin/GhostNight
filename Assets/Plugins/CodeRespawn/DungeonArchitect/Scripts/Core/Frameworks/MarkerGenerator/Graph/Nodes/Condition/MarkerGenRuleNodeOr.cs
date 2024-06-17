using DungeonArchitect.Graphs;

namespace DungeonArchitect.MarkerGenerator.Nodes.Condition
{
    public class MarkerGenRuleNodeOr : MarkerGenRuleGraphNodeConditionBase
    {
        public override string Title => "OR";

        protected override void CreateDefaultPins()
        {
            CreateInputPin("A");
            CreateInputPin("B");
            
            CreateOutputPin("");
        }
    }
}