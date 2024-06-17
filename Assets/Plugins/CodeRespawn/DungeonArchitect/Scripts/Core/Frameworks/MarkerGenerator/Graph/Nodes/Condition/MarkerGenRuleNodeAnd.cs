using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Pins;

namespace DungeonArchitect.MarkerGenerator.Nodes.Condition
{
    public class MarkerGenRuleNodeAnd : MarkerGenRuleGraphNodeConditionBase
    {
        public override string Title => "AND";

        protected override void CreateDefaultPins()
        {   
            CreateInputPin("A");
            CreateInputPin("B");
            
            CreateOutputPin("");
        }
    }
}