using DungeonArchitect.Editors.MarkerGenerator.Editors.Grid;
using DungeonArchitect.MarkerGenerator;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors
{
    public class MarkerGenEditorFactory
    {
        public static MarkerGenEditor Create(MarkerGeneratorAsset asset)
        {
            MarkerGenEditor editor = null;

            if (asset != null)
            {
                if (asset.layoutType == MarkerGeneratorLayoutType.Grid)
                {
                    editor = new GridMarkerGenEditor();
                }
                else
                {
                    Debug.Log("Unsupported pattern match editor implementation: " + asset.layoutType);
                }    
            }
            else
            {
                Debug.Log("Cannot create pattern matcher editor. Invalid asset reference");
            }

            if (editor != null)
            {
                editor.Asset = asset;
            }

            return editor;
        }
    }
}