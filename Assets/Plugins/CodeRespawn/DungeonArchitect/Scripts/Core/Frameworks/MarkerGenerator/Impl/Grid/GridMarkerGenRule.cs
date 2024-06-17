using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Rule.Grid
{
    public enum GridMarkerGenRuleType
    {
        Ground,
        EdgeX,
        EdgeZ,
        Corner
    }
    
    public class GridMarkerGenRule : MarkerGenRule
    {
        [HideInInspector]
        public Vector2Int coord;
        
        [HideInInspector]
        public GridMarkerGenRuleType ruleType;
        
        [Tooltip(@"Tell the system that you'll be inserting an art asset at this location.  By default, if the rule graph emits a marker (EmitMarker action node), it would know that this position would be occupied by an art asset and you can ignore this flag.    In cases where you'd insert a larger asset, e.g. a 2x2 tile, you'd use the EmitMarker node in one of the 2x2 position and the system needs to know that the nearby 3 tiles would also be occupied. Go to each one and set this hint so your final result does not have overlaps")]
        public bool hintWillInsertAssetHere = false;

        public override bool IsAssetInsertedHere()
        {
            return hintWillInsertAssetHere || base.IsAssetInsertedHere();
        }
    }
}