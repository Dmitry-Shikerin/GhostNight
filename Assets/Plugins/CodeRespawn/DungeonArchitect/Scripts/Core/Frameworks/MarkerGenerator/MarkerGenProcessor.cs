using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Processor
{
    
    public interface IMarkerGenProcessor
    {
        bool Process(MarkerGenPattern pattern, PropSocket[] markers, System.Random random, out PropSocket[] newMarkers);
        void Release();
    }
    
}