using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Pins
{
    public abstract class MarkerGenRuleGraphPin : GraphPin
    {
        public string text = "";
        
        public virtual Color GetPinColor()
        {
            return Color.white;
        }

    }
}