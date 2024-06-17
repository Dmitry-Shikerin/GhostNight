using System;
using DungeonArchitect.MarkerGenerator.Rule;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonArchitect.MarkerGenerator
{
    public class MarkerGenPattern : ScriptableObject
    {
        public string patternName;
        
        [Tooltip(@"The probability of running the patter matcher on the scene. Set to 1.0 to run it everywhere (100% of the time), 0.5 to insert this randomly 50% of the time, 0.0 to disable it")]
        public float probability = 1.0f;
	
        [Tooltip(@"Rotate the pattern as many times as needed to fit in all directions (e.g. rotate the pattern in 0, 90, 180 and 270 degrees and try to fit there. You might want to disable this if your fitting needs to happen in a fixed direction, e.g. a top down diablo-like game where you'd want to consider the camera angle before placing the assets")]
        public bool rotateToFit = true;

        [Tooltip(@"If disabled, the system tries to match the pattern from left to right, top to bottom in the generated level, which would create a ordered fitting pattern (e.g. a 2x1 tile would all face the same way). Enable this to create a more organic randomized fitting order")]
        public bool randomizeFittingOrder = true;

        [Tooltip(@"As the pattern is matched on a layer, cell by cell, some cells will be skipped if the rule inserts a marker which might overlap with another marker that was inserted same layer previously (HintWillInsertAssetHere flag on the rule block). Enable this to allow overlaps")]
        public bool allowInsertionOverlaps = false;
        
        [HideInInspector]
        public MarkerGenRule[] rules = Array.Empty<MarkerGenRule>();
    }
}
