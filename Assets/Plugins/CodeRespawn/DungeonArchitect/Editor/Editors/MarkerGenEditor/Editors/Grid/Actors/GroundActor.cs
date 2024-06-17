using System;
using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.SxEngine;
using DungeonArchitect.SxEngine.Utils;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors.Grid.Actors
{
    public class SxGridGroundObjectMaterial : SxUnityResourceMaterial
    {
        public SxGridGroundObjectMaterial() : base("MarkerGen/materials/MatGroundQuad")
        {
            DepthBias = -2;
        }
    }
    public class SxGridGroundObjectSelectedMaterial : SxUnityResourceMaterial
    {
        public SxGridGroundObjectSelectedMaterial() : base("MarkerGen/materials/MatGroundQuadSelected")
        {
            DepthBias = -2;
        }
    }

    
    
    public class GridMarkerGenGroundActorSettings
    {
        public Vector2Int Start = Vector2Int.zero;
        public Vector2Int End = Vector2Int.zero;
        public float TileSize = 4;
        public float EdgeSize = 1;
    }

    public class SxGridMarkerGenGroundItemComponent : SxMeshComponent
    {
        public Vector2Int Coord;
        public GridMarkerGenRuleType ItemType;
        
        private static int idScale = Shader.PropertyToID("_Scale");
        private static Dictionary<Vector3, SxMaterial> materialRegistryNormal = new Dictionary<Vector3, SxMaterial>();
        private static Dictionary<Vector3, SxMaterial> materialRegistryHover = new Dictionary<Vector3, SxMaterial>();
        private bool hovered = false;

        public void Initialize(Vector3 location, Vector2 scale)
        {
            RequiresTick = false;
            Mesh = SxMeshRegistry.Get<SxMGGroundTileMesh>();
            
            var scale3D = new Vector3(scale.x, 1, scale.y);
            RelativeTransform.SetTRS(location, Quaternion.identity, scale3D);

            UpdateMaterialParameters();
        }
        

        public void SetHover(bool newHoveredState)
        {
            if (hovered != newHoveredState)
            {
                this.hovered = newHoveredState;
                UpdateMaterialParameters();
            }
        }

        private void UpdateMaterialParameters()
        {
            Material = GetCachedMaterial(RelativeTransform.lossyScale, hovered);
        }
        
        private static T CreateQuadMat<T>(Vector3 scale) where T : SxMaterial, new()
        {
            var mat = new T();
            mat.UnityMaterial.SetVector(idScale, scale);
            return mat;
        }
        
        private static SxMaterial GetCachedMaterial(Vector3 scale, bool selected)
        {
            if (!materialRegistryNormal.ContainsKey(scale))
            {
                materialRegistryNormal.Add(scale, CreateQuadMat<SxGridGroundObjectMaterial>(scale));
            }
            
            if (!materialRegistryHover.ContainsKey(scale))
            {
                materialRegistryHover.Add(scale, CreateQuadMat<SxGridGroundObjectSelectedMaterial>(scale));
            }

            return selected ? materialRegistryHover[scale] : materialRegistryNormal[scale];
        }

    }
    
    public class SxGridMarkerGenGroundActor : SxActor
    {
        readonly Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent> tiles = new Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent>();
        readonly Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent> edgesX = new Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent>();
        readonly Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent> edgesZ = new Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent>();
        readonly Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent> corners = new Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent>();

        public Rect WorldBounds { get; private set; } = Rect.zero;
        public GridMarkerGenGroundActorSettings Settings { get; private set; }
        
        public void Build(GridMarkerGenGroundActorSettings buildSettings)
        {
            Settings = buildSettings;

            var tileMeshNormal = new Color(0, 1, 0);    // The normal is expected in the color channel on the used shader
            var tileMesh = new SxMGGroundTileMesh(tileMeshNormal);
            
            var itemTileScale = new Vector2(Settings.TileSize, Settings.TileSize);
            var edgeScaleX = new Vector2(Settings.TileSize, Settings.EdgeSize);
            var edgeScaleZ = new Vector2(Settings.EdgeSize, Settings.TileSize);
            var cornerScale = new Vector2(Settings.EdgeSize, Settings.EdgeSize);
            
            float offset = Settings.TileSize + Settings.EdgeSize;

            // Build the tiles, edgesX, edgesY and corners separately so they are sorted correctly for rendering in fewer batches
            
            // Tiles
            for (int X = Settings.Start.x; X <= Settings.End.x; X++)
            {
                for (int Y = Settings.Start.y; Y <= Settings.End.y; Y++)
                {
                    var key = new Vector2Int(X, Y);
                    var tileLoc = new Vector3(X, 0, Y) * offset;
                    var tileItem = CreateItem(tileLoc, itemTileScale);
                    tileItem.Coord = key;
                    tileItem.ItemType = GridMarkerGenRuleType.Ground;
                    tiles.Add(key, tileItem);
                }
            }
            
            // Edges X
            for (int X = Settings.Start.x; X <= Settings.End.x; X++)
            {
                for (int Y = Settings.Start.y; Y <= Settings.End.y; Y++)
                {
                    var key = new Vector2Int(X, Y);
                    var edgeLocX = new Vector3(X, 0, Y - 0.5f) * offset;
                    var edgeItemX = CreateItem(edgeLocX, edgeScaleX);
                    edgeItemX.Coord = key;
                    edgeItemX.ItemType = GridMarkerGenRuleType.EdgeX;
                    edgesX.Add(key, edgeItemX);
                }
            }

            // Edges Z
            for (int X = Settings.Start.x; X <= Settings.End.x; X++)
            {
                for (int Y = Settings.Start.y; Y <= Settings.End.y; Y++)
                {
                    var key = new Vector2Int(X, Y);
                    var edgeLocZ = new Vector3(X - 0.5f, 0, Y) * offset;
                    var edgeItemZ = CreateItem(edgeLocZ, edgeScaleZ);
                    edgeItemZ.Coord = key;
                    edgeItemZ.ItemType = GridMarkerGenRuleType.EdgeZ;
                    edgesZ.Add(key, edgeItemZ);
                }
            }
            
            // Corners
            for (int X = Settings.Start.x; X <= Settings.End.x; X++)
            {
                for (int Y = Settings.Start.y; Y <= Settings.End.y; Y++)
                {
                    var key = new Vector2Int(X, Y);
                    var cornerLoc = new Vector3(X - 0.5f, 0, Y - 0.5f) * offset;
                    var cornerItem = CreateItem(cornerLoc, cornerScale);
                    cornerItem.Coord = key;
                    cornerItem.ItemType = GridMarkerGenRuleType.Corner;
                    corners.Add(key, cornerItem);
                }
            }
            
            // Setup the world bounds
            {
                var start = new Vector2(Settings.Start.x - 0.5f, Settings.Start.y - 0.5f) * offset - new Vector2(Settings.EdgeSize, Settings.EdgeSize) * 0.5f;
                var end = new Vector2(Settings.End.x, Settings.End.y) * offset + new Vector2(Settings.TileSize, Settings.TileSize) * 0.5f;
                WorldBounds = new Rect(start, end - start);
            }
        }

        public void OnMouseHover(Vector3 worldIntersection)
        {
            Deproject(worldIntersection, out var hoveredItem, out _);
            foreach (var item in Components)
            {
                if (item is SxGridMarkerGenGroundItemComponent groundItem)
                {
                    groundItem.SetHover(item == hoveredItem);
                }
            }
        }

        public bool Deproject(Vector3 worldIntersection, out SxGridMarkerGenGroundItemComponent item, out GridMarkerGenRuleType itemType)
        {
            GridMarkerGenEditorUtils.DeprojectGround(worldIntersection, Settings.TileSize, Settings.EdgeSize, out var coord, out itemType);

            Dictionary<Vector2Int, SxGridMarkerGenGroundItemComponent> itemMap;
            if (itemType == GridMarkerGenRuleType.Corner) {
                itemMap = corners;
            }
            else if (itemType == GridMarkerGenRuleType.EdgeZ) {
                itemMap = edgesZ;
            }
            else if (itemType == GridMarkerGenRuleType.EdgeX) {
                itemMap = edgesX;
            }
            else {	// Ground
                itemMap = tiles;
            }

            if (itemMap.ContainsKey(coord))
            {
                item = itemMap[coord];
                return true;
            }
            else
            {
                item = null;
                return false;
            }
        }

        SxGridMarkerGenGroundItemComponent CreateItem(Vector3 location, Vector2 scale)
        {
            var item = AddComponent<SxGridMarkerGenGroundItemComponent>();
            item.Initialize(location, scale);
            
            return item;
        }

    }
    
    
    public class SxMGGroundTileMesh : SxMesh
    {
        public SxMGGroundTileMesh()
        {
            Build(Color.white);
        }

        public SxMGGroundTileMesh(Color color)
        {
            Build(color);
        }
        
        void Build(Color color)
        {

            const float size = 0.5f;
            var vertices = new SxMeshVertex[]
            {
                new SxMeshVertex(new Vector3(-size, 0, -size), color, new Vector2(0, 0)),   // a
                new SxMeshVertex(new Vector3(-size, 0, size), color, new Vector2(0, 1)),     // d
                new SxMeshVertex(new Vector3(size, 0, size), color, new Vector2(1, 1)),     // c
                new SxMeshVertex(new Vector3(size, 0, -size), color, new Vector2(1, 0)),    // b
            };

            CreateSection(0, GL.QUADS, vertices);   
        }
    }
}