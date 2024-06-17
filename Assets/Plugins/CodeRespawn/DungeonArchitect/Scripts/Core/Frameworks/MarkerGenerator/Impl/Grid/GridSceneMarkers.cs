
using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Grid
{
    class GridSceneCell
    {
        public readonly List<PropSocket> TileMarkers = new List<PropSocket>();
        public readonly List<PropSocket> CornerMarkers = new List<PropSocket>();
        public readonly List<PropSocket> EdgeXMarkers = new List<PropSocket>();
        public readonly List<PropSocket> EdgeZMarkers = new List<PropSocket>();

        public void Add(PropSocket marker, GridMarkerGenRuleType type)
        {
            var markerList = GetMarkerList(type);
            if (markerList != null)
            {
                var markerPosition = Matrix.GetTranslation(ref marker.Transform);
                var duplicate = false;
                foreach (var existingMarker in markerList)
                {
                    var existingPosition = Matrix.GetTranslation(ref existingMarker.Transform);
                    if (existingMarker.SocketType == marker.SocketType && markerPosition.Equals(existingPosition ))
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                {
                    markerList.Add(marker);
                }
            }
        }

        public void Remove(string markerName, GridMarkerGenRuleType type)
        {
            var markerList = GetMarkerList(type);
            if (markerList != null)
            {
                PropSocket markerToRemove = null;
                foreach (var marker in markerList)
                {
                    if (marker.SocketType == markerName)
                    {
                        markerToRemove = marker;
                        break;
                    }
                }

                if (markerToRemove != null)
                {
                    markerList.Remove(markerToRemove);
                }
            }
        }
        
        public bool Contains(string markerName, GridMarkerGenRuleType type)
        {
            var markerList = GetMarkerList(type);
            if (markerList != null)
            {
                foreach (var marker in markerList)
                {
                    if (marker.SocketType == markerName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        

        public List<PropSocket> GetMarkerList(GridMarkerGenRuleType type)
        {
            switch (type)
            {
                case GridMarkerGenRuleType.Ground:
                    return TileMarkers;
                
                case GridMarkerGenRuleType.Corner:
                    return CornerMarkers;
                
                case GridMarkerGenRuleType.EdgeX:
                    return EdgeXMarkers;
                
                case GridMarkerGenRuleType.EdgeZ:
                    return EdgeZMarkers;
                
                default:
                    return null;
            }
        }
    }
    
    class GridSceneCells<T> where T : new()
    {
        public GridSceneCells()
        {
        }

        public GridSceneCells(Vector2Int worldOffset, Vector2Int worldSize)
        {
            Init(worldOffset, worldSize);
        }

        protected void Init(Vector2Int worldOffset, Vector2Int worldSize)
        {
            this.WorldOffset = worldOffset;
            this.WorldSize = worldSize;

            Cells.Clear();
            int numItems = worldSize.x * worldSize.y;
            for (int i = 0; i < numItems; i++)
            {
                Cells.Add(new T());
            }
        }

        public T GetValue(Vector2Int coord)
        {
            if (!IsCoordValid(coord))
            {
                return default;
            }

            return Cells[GetIndex(coord)];
        }
        
        
        public bool IsCoordValid(Vector2Int coord) {
            return coord.x >= WorldOffset.x && coord.x < WorldOffset.x + WorldSize.x
                                                  && coord.y >= WorldOffset.y && coord.y < WorldOffset.y + WorldSize.y;
        }

        protected int GetIndex(Vector2Int coord)
        {
            var localCoord = coord - WorldOffset;
            return WorldSize.x * localCoord.y + localCoord.x;
        }
        
        public Vector2Int WorldOffset { get; protected set; }
        public Vector2Int WorldSize { get; protected set; }
        
        protected readonly List<T> Cells = new List<T>();
    }

    class GridCellOccupancyInfo
    {
        public bool GroundOccupied { get; private set; } = false;
        public bool EdgeXOccupied { get; private set; } = false;
        public bool EdgeZOccupied { get; private set; } = false;
        public bool CornerOccupied { get; private set; } = false;

        public void SetOccupied(GridMarkerGenRuleType type, bool value)
        {
            if (type == GridMarkerGenRuleType.Ground)
            {
                GroundOccupied = value;
            }
            else if (type == GridMarkerGenRuleType.EdgeX)
            {
                EdgeXOccupied = value;
            } 
            else if (type == GridMarkerGenRuleType.EdgeZ)
            {
                EdgeZOccupied = value;
            } 
            else if (type == GridMarkerGenRuleType.Corner)
            {
                CornerOccupied = value;
            } 
        }

        public bool IsOccupied(GridMarkerGenRuleType type)
        {
            if (type == GridMarkerGenRuleType.Ground) return GroundOccupied;
            if (type == GridMarkerGenRuleType.EdgeX) return EdgeXOccupied;
            if (type == GridMarkerGenRuleType.EdgeZ) return EdgeZOccupied;
            if (type == GridMarkerGenRuleType.Corner) return CornerOccupied;
            return false;
        }
    }

    class GridSceneCellsBool : GridSceneCells<GridCellOccupancyInfo>
    {
        public GridSceneCellsBool(Vector2Int worldOffset, Vector2Int worldSize) : base(worldOffset, worldSize)
        {
        }
    }


    class GridSceneMarkerList : GridSceneCells<GridSceneCell>
    {
        public GridSceneMarkerList(Vector3 cellSize3D, PropSocket[] markers, int boundsExpansion)
        {
            CellSize = new Vector2(cellSize3D.x, cellSize3D.z);
            CellHeight = cellSize3D.y;
            nextMarkerId = markers.Length;

            Vector2 boundsMin, boundsMax;
            
            if (markers.Length > 0)
            {
                var position3D = Matrix.GetTranslation(ref markers[0].Transform);
                boundsMin = boundsMax = new Vector2(position3D.x, position3D.z);
                foreach (var marker in markers) {
                    var location = Matrix.GetTranslation(ref marker.Transform);
                    boundsMin.x = Mathf.Min(boundsMin.x, location.x);
                    boundsMin.y = Mathf.Min(boundsMin.y, location.z);
                    boundsMax.x = Mathf.Max(boundsMax.x, location.x);
                    boundsMax.y = Mathf.Max(boundsMax.y, location.z);
                }
            }
            else
            {
                boundsMin = boundsMax = Vector2.zero;
            }

            if (CellSize.x > 0 && CellSize.y > 0) {
                boundsMin /= CellSize;
                boundsMax /= CellSize;
            }
            else {
                boundsMin = boundsMax = Vector2.zero;
            }

            int startX = Mathf.FloorToInt(boundsMin.x);
            int startY = Mathf.FloorToInt(boundsMin.y);
            int endX = Mathf.FloorToInt(boundsMax.x);
            int endY = Mathf.FloorToInt(boundsMax.y);

            WorldOffset = new Vector2Int(startX, startY);
            WorldSize = new Vector2Int(endX - startX + 1, endY - startY + 1);

            // Expand the bounds
            WorldOffset -= new Vector2Int(boundsExpansion, boundsExpansion);
            WorldSize += new Vector2Int(boundsExpansion, boundsExpansion) * 2;

            Init(WorldOffset, WorldSize);

            // Register the markers
            foreach (var marker in markers)
            {
                var position = Matrix.GetTranslation(ref marker.Transform);
                GetWorldToCellCoords(position, out var coord, out var coordType);
                var cell = GetValue(coord);
                if (cell != null) {
                    cell.Add(marker, coordType);
                }
            } 
        }
        
        public void GetWorldToCellCoords(Vector3 worldLocation, out Vector2Int coord, out GridMarkerGenRuleType coordType) {
            var coordF = new Vector2(worldLocation.x, worldLocation.z) / CellSize;

            coord = new Vector2Int(
                Mathf.FloorToInt(coordF.x),
                Mathf.FloorToInt(coordF.y));

            float dx = coordF.x - coord.x;
            float dz = coordF.y - coord.y;

            System.Func<float, float, bool> Equals = (float a, float b) => Mathf.Abs(a - b) < 1e-2f;
            if (Equals(dx, 0.0f) && Equals(dz, 0.0f)) {
                coordType = GridMarkerGenRuleType.Corner;
            }
            else if (Equals(dx, 0.5f) && Equals(dz, 0.0f)) {
                coordType = GridMarkerGenRuleType.EdgeX;
            }
            else if (Equals(dx, 0.0f) && Equals(dz, 0.5f)) {
                coordType = GridMarkerGenRuleType.EdgeZ;
            }
            else {
                coordType = GridMarkerGenRuleType.Ground;
            }
        }
            
        
        public void GetCellToWorldCoords(Vector2Int coord, int coordY, GridMarkerGenRuleType coordType, out Vector3 outWorldLocation) {
            var coordF = new Vector2(coord.x, coord.y);
            if (coordType == GridMarkerGenRuleType.Ground) {
                coordF += new Vector2(0.5f, 0.5f);
            }
            else if (coordType == GridMarkerGenRuleType.EdgeX) {
                coordF += new Vector2(0.5f, 0.0f);
            }
            else if (coordType == GridMarkerGenRuleType.EdgeZ) {
                coordF += new Vector2(0.0f, 0.5f);
            }

            var pos2D = Vector2.Scale(coordF, CellSize);
            outWorldLocation = new Vector3(pos2D.x, coordY * CellHeight, pos2D.y);
        }

        public PropSocket[] GenerateMarkerList()
        {
            var result = new List<PropSocket>();
            foreach (var cell in Cells)
            {
                result.AddRange(cell.TileMarkers);
                result.AddRange(cell.CornerMarkers);
                result.AddRange(cell.EdgeXMarkers);
                result.AddRange(cell.EdgeZMarkers);
            }

            // Reset their ids
            int indexCounter = 0;
            foreach (var marker in result)
            {
                marker.Id = indexCounter++;
            }

            return result.ToArray();
        }
        
        public int GenerateNextMarkerId()
        {
            return nextMarkerId++;
        }
            
        public Vector2 CellSize { get; private set; }
        public float CellHeight { get; private set; }
        private int nextMarkerId = 0;
    }
}
