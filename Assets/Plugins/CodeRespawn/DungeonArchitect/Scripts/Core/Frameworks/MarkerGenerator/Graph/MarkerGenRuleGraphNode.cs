using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator
{
    public abstract class MarkerGenRuleGraphNode : GraphNode
    {
        public virtual Color BodyColor => new Color(0.15f, 0.15f, 0.15f);
        public virtual Color TitleColor => new Color(0.3f, 0.3f, 0.3f);
        
        public abstract string Title { get; }
        
        public override void Initialize(string id, Graph graph)
        {
            base.Initialize(id, graph);
            CreateDefaultPins();
        }
        
        protected virtual void CreateDefaultPins()
        {
        }
    }
}