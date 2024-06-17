//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using DungeonArchitect.UI.Widgets;
using UnityEngine;

namespace DungeonArchitect.Flow.Domains.Layout.Tooling.Graph3D
{
    public class FlowLayout3DRenderSettings
    {
        public FlowLayout3DRenderSettings(float nodeRadius)
        { 
            NodeRadius = nodeRadius;
            InactiveNodeRadius = NodeRadius * 0.2f;
            ItemRadius = NodeRadius * 0.4f;
            LinkThickness = NodeRadius * 0.2f;
            
        }
        public float NodeRadius { get; private set; } = 0.5f;
        public float InactiveNodeRadius { get; private set; } = 0.1f;
        public float ItemRadius { get; private set; }  = 0.2f;
        public float LinkThickness { get; private set; }  = 0.10f;
    }
    
    class FlowLayout3DConstants
    {
        public static readonly Color InactiveNodeColor = new Color(0, 0, 0, 0.05f);
        public static readonly Color LinkColor = new Color(0, 0, 0, 0.9f);
        public static readonly Color LinkOneWayColor = new Color(1, 0.2f, 0, 0.9f);
        public static readonly Color LinkItemRefColor = new Color(1, 0, 0, 0.9f);
        public static readonly float LinkHeadThicknessMultiplier = 4.0f;
        public static readonly float ItemNodeScaleMultiplier = 0.3f;   
    }

    public class FlowLayoutToolGraph3D : SxViewportWidget
    {
        private FlowLayout3DRenderSettings renderSettings = new FlowLayout3DRenderSettings(0.5f); 
        
        public void RecenterView()
        {   
            var activePoints = new List<Vector3>();
            var inactivePoints = new List<Vector3>();

            var nodeActors = World.GetActorsOfType<SxLayoutNodeActor>();
            foreach (var nodeActor in nodeActors)
            {
                if (nodeActor == null) continue;
                if (nodeActor.LayoutNode.active)
                {
                    activePoints.Add(nodeActor.Position);
                    foreach (var subNode in nodeActor.LayoutNode.MergedCompositeNodes)
                    {
                        activePoints.Add(subNode.position);
                    }
                }
                else
                {
                    inactivePoints.Add(nodeActor.Position);
                }
            }

            if (activePoints.Count > 0)
            {
                FocusCameraOnPoints(activePoints.ToArray(), renderSettings.NodeRadius);
            }
            else if (inactivePoints.Count > 0)
            {
                FocusCameraOnPoints(inactivePoints.ToArray(), renderSettings.NodeRadius);
            }
            else
            {
                ResetCamera(false);
            }
        }
        
        public void Build(FlowLayoutGraph graph)
        {
            SxLayout3DWorldBuilder.Build(World, graph);
            renderStateInvalidated = true;
        }
    }
}