using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DungeonArchitect.MarkerGenerator
{
    [Serializable]
    public enum MarkerGeneratorLayoutType
    {
        Grid,
    }
    
    [System.Serializable]
    public class MarkerGeneratorAsset : ScriptableObject
    {
        [FormerlySerializedAs("layers")]
        public MarkerGenPattern[] patterns = Array.Empty<MarkerGenPattern>();

        public MarkerGeneratorLayoutType layoutType = MarkerGeneratorLayoutType.Grid;
    }
}