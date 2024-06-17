using DungeonArchitect.MarkerGenerator.Rule.Grid;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors.Grid
{
    public class GridMarkerGenEditorUtils
    {
        public static void DeprojectGround(Vector3 InWorldIntersection, float InTileSize, float InEdgeSize, out Vector2Int OutCoord, out GridMarkerGenRuleType OutItemType)
        {
            float offset = InTileSize + InEdgeSize;
            var loc = new Vector2(InWorldIntersection.x, InWorldIntersection.z);
            {
                var locOffset = InTileSize * 0.5f + InEdgeSize;
                loc += new Vector2(locOffset, locOffset);
            }

            var key = new Vector2Int();
            key.x = Mathf.FloorToInt(loc.x / offset);
            key.y = Mathf.FloorToInt(loc.y / offset);

            float fx = loc.x - key.x * offset;
            float fz = loc.y - key.y * offset;
            if (fx < InEdgeSize && fz < InEdgeSize) {
                OutItemType = GridMarkerGenRuleType.Corner;
            }
            else if (fx < InEdgeSize) {
                OutItemType = GridMarkerGenRuleType.EdgeZ;
            }
            else if (fz < InEdgeSize) {
                OutItemType = GridMarkerGenRuleType.EdgeX;
            }
            else {
                OutItemType = GridMarkerGenRuleType.Ground;
            }
            OutCoord = key;
        }

        public static void GetItemLocationScale(Vector2Int coord, float tileSize, float edgeSize, GridMarkerGenRuleType itemType, bool bVisuallyDominant,
            out Vector3 outLocation, out Vector3 outScale)
        {
            float offset = tileSize + edgeSize;
            float wallHeight = tileSize;
            if (!bVisuallyDominant) {
                wallHeight *= 0.25f;
            }
	
            if (itemType == GridMarkerGenRuleType.Corner) {
                outLocation = new Vector3(coord.x - 0.5f, 0, coord.y - 0.5f) * offset;
                outScale = new Vector3(edgeSize, edgeSize, wallHeight);
            }
            else if (itemType == GridMarkerGenRuleType.EdgeX) {
                outLocation = new Vector3(coord.x, 0, coord.y - 0.5f) * offset;
                outScale = new Vector3(tileSize, edgeSize, wallHeight);
            }
            else if (itemType == GridMarkerGenRuleType.EdgeZ) {
                outLocation = new Vector3(coord.x - 0.5f, 0, coord.y) * offset;
                outScale = new Vector3(edgeSize, tileSize, wallHeight);
            }
            else {	// Ground
                outLocation = new Vector3(coord.x, 0, coord.y) * offset;
                outScale = new Vector3(tileSize, tileSize, tileSize * 0.1f);
            }
        }


        public static Color CreateHoverColor(Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            s *= 0.75f;
            return Color.HSVToRGB(h, s, v);

        }

        public static Color CreatePaleColor(Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            s *= 0.5f;
            return Color.HSVToRGB(h, s, v);
        }
    }
}