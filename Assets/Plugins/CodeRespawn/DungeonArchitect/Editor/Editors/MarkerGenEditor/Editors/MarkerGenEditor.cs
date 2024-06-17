using DungeonArchitect.Editors.MarkerGenerator.UI.Viewport;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.UI;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors
{
    public abstract class MarkerGenEditor
    {
        public abstract System.Type PatternType { get; }
        public abstract System.Type RuleType { get; }
        
        public PatternViewportWidget PatternViewport { get; set; }
        public bool RequestRepaint { get; set; }
        public MarkerGeneratorAsset Asset { get; set; }

        public abstract void Update(double frameTime);
        public abstract void LoadScene(MarkerGenPattern pattern, UIPlatform platform);
        public abstract void HandleInput(Event widgetEvent, UISystem uiSystem);

        public virtual void OnRuleGraphChanged(MarkerGenRule rule) { }
        public virtual void HandleRulePropertyChange(MarkerGenRule rule) { }

        public delegate void OnRuleSelected(MarkerGenRule rule);
        public event OnRuleSelected RuleSelected;

        protected void NotifyRuleSelected(MarkerGenRule rule)
        {
            RuleSelected?.Invoke(rule);
        }

    }
}