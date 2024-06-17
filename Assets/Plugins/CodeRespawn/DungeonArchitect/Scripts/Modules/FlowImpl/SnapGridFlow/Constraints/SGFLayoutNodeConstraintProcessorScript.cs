//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using DungeonArchitect.Flow.Domains.Layout;
using DungeonArchitect.Flow.Domains.Layout.Pathing;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Flow.Impl.SnapGridFlow.Constraints
{
    public class SGFLayoutNodeConstraintProcessorScript : IFlowLayoutNodeCreationConstraint
    {
        private readonly ISGFLayoutNodePositionConstraint scriptConstraint;
        private readonly Vector3Int gridSize;
        private readonly System.Random random;

        public SGFLayoutNodeConstraintProcessorScript(ISGFLayoutNodePositionConstraint scriptConstraint, Vector3Int gridSize, System.Random random)
        {
            this.scriptConstraint = scriptConstraint;
            this.gridSize = gridSize;
            this.random = random;
        }
        
        public bool CanCreateNodeAt(FlowLayoutGraphNode node, int totalPathLength, int currentPathPosition)
        {
            if (scriptConstraint == null || node == null)
            {
                // Ignore
                return true;
            }

            var nodeCoord = MathUtils.RoundToVector3Int(node.coord);
            var settings = new SGFLayoutNodePositionConstraintSettings
            {
                CurrentPathPosition = currentPathPosition,
                TotalPathLength = totalPathLength,
                NodeCoord = nodeCoord,
                GridSize = gridSize,
                Random = random
            };
            return scriptConstraint.CanCreateNodeAt(settings);
        }
    }
    
}