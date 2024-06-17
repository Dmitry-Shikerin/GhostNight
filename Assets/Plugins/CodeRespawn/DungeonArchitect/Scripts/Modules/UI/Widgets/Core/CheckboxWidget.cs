using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class CheckboxWidget : WidgetBase
    {
        public bool Value = false;
        
        private string label;
        Color color = new Color(0.8f, 0.8f, 0.8f);

        public CheckboxWidget(bool defaultValue, string label)
        {
            this.Value = defaultValue;
            this.label = label;
        }

        public CheckboxWidget SetColor(Color color)
        {
            this.color = color;
            return this;
        }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer renderer)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.black;

            var state = new GUIState(renderer);
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);
            renderer.color = color;
            Value = renderer.Toggle(bounds, Value, label);
            state.Restore();
        }
    }
}