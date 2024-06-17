using DungeonArchitect.MarkerGenerator.Rule.Grid;
using UnityEngine;

namespace DungeonArchitect
{
    /// <summary>
    /// Inherit from this class to attach custom 
    /// </summary>
    public class GridMarkerGenRuleUserScript : ScriptableObject
    {
        public struct ValidateSettings
        {
            public Vector3 Position;
            public Vector2Int Coord;
            public GridMarkerGenRuleType CoordType;
            public Matrix4x4 DungeonTransform;
            public DungeonBuilder Builder;
            public DungeonModel Model;
            public DungeonConfig Config;
            public DungeonQuery Query;
        }

        public virtual bool Validate(ValidateSettings settings)
        {
            return true;
        }
    }
}