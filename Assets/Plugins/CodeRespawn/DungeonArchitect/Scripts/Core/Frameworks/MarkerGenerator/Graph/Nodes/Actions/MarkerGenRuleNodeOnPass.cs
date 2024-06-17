using DungeonArchitect.Graphs;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DungeonArchitect.MarkerGenerator.Nodes.Actions
{
    public class MarkerGenRuleNodeOnPass : MarkerGenRuleGraphNodeActionBase
    {
        public override Color BodyColor => new Color(0.1f, 0.1f, 0.3f);
        public override Color TitleColor => new Color(0.1f, 0.2f, 0.4f);
        
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);

            canBeDeleted = false;
        }

        public override string Title => "On Selected";

        protected override void CreateDefaultPins()
        {
            CreateOutputPin("Exec");
        }
    }
}