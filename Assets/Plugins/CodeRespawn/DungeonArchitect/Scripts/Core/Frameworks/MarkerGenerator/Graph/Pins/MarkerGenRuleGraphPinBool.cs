using DungeonArchitect.Graphs;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Pins
{
    public class MarkerGenRuleGraphPinBool : MarkerGenRuleGraphPin
    {
        public bool defaultValue = true;

        public override Color GetPinColor()
        {
            if (ClickState == GraphPinMouseState.Hover)
            {
                return new Color(1.0f, 0.5f, 0.1f);
            }
            else if (ClickState == GraphPinMouseState.Clicked)
            {
                return new Color(0.9f, 0.1f, 0.1f);
            }
            
            return Color.red;
        }
    } 
}