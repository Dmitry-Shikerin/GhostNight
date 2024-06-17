using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.UI.Viewport
{
    public class PatternViewportWidget : SxViewportWidget
    {
        public PatternViewportWidget()
        {
            Renderer.SortRenderCommands = false;
        }

        public void RecenterView()
        {
            // TODO: Implement me
            FocusCameraOnPoints(new Vector3[] { Vector3.zero }, 3);
        }
    }
}
