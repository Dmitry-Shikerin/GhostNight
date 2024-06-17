//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect
{
    public struct SGFLayoutNodePositionConstraintSettings
    {
        public int CurrentPathPosition;
        public int TotalPathLength;
        public Vector3Int NodeCoord;
        public Vector3Int GridSize;
        public System.Random Random;
    }
    public interface ISGFLayoutNodePositionConstraint
    {
        bool CanCreateNodeAt(SGFLayoutNodePositionConstraintSettings settings);
    }
}