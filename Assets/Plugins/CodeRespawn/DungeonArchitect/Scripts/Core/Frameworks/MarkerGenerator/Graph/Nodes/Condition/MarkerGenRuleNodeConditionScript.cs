using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Nodes.Condition
{
    public class MarkerGenRuleNodeConditionScript : MarkerGenRuleGraphNodeConditionBase
    {
        public string scriptClassName;

        public override string Title
        {
            get
            {
                string displayName = GetDisplayName();
                if (string.IsNullOrEmpty(displayName))
                {
                    displayName = "<NONE>";
                }
                return "Script: " + displayName;
            }
        }
        protected override void CreateDefaultPins()
        {
            CreateOutputPin("");
        }

        private System.Type cachedType = null;
        private string cachedTypeText = null;
        
        private string GetDisplayName()
        {
            if (string.IsNullOrEmpty(scriptClassName))
            {
                return "";
            }

            if (cachedType == null || cachedTypeText != scriptClassName)
            {
                cachedTypeText = scriptClassName;
                cachedType = System.Type.GetType(scriptClassName);
            }

            return cachedType == null ? "" : cachedType.Name;
        }
    }
}