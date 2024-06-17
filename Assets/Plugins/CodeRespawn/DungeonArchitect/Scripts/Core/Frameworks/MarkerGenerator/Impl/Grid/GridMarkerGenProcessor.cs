using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.MarkerGenerator.Grid;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.MarkerGenerator.Rule.Grid.Assemblies;
using DungeonArchitect.MarkerGenerator.VM;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Processor.Grid
{
    public class GridMarkerGenProcessor : IMarkerGenProcessor
    {
        private Matrix4x4 dungeonTransform;
        private Vector3 gridSize;
        private ScriptInstanceCache<GridMarkerGenRuleUserScript> scriptCache;
        private DungeonBuilder dungeonBuilder;
        private DungeonModel dungeonModel;
        private DungeonConfig dungeonConfig;
        private DungeonQuery dungeonQuery;
        
        public GridMarkerGenProcessor(Matrix4x4 dungeonTransform, Vector3 gridSize, DungeonBuilder dungeonBuilder, DungeonModel dungeonModel, DungeonConfig dungeonConfig, DungeonQuery dungeonQuery)
        {
            this.dungeonTransform = dungeonTransform;
            this.gridSize = gridSize;
            scriptCache = new ScriptInstanceCache<GridMarkerGenRuleUserScript>();
            this.dungeonBuilder = dungeonBuilder;
            this.dungeonModel = dungeonModel;
            this.dungeonConfig = dungeonConfig;
            this.dungeonQuery = dungeonQuery;
        }

        public void Release()
        {
            if (scriptCache != null)
            {
                scriptCache.Release();
            }
        }
        
        struct PatternMatchCoord {
            public int X;
            public int Y;
            public int AsmIdx;
            public override string ToString()
            {
                return string.Format("({0}, {1}) - [{2}]", X, Y, AsmIdx);
            }
        }

        public bool Process(MarkerGenPattern pattern, PropSocket[] oldMarkers, System.Random random, out PropSocket[] newMarkers)
        {
            var gridPattern = pattern as GridMarkerGenPattern;
            if (gridPattern == null || !ShouldProcessAssembly(gridPattern))
            {
                newMarkers = Array.Empty<PropSocket>();
                return false;
            }

            var assemblies = new List<GridMarkerGenPatternAssembly>
            {
                GridMarkerGenPatternAssemblyBuilder.GenerateAssembly(gridPattern)
            };
            
            if (gridPattern.rotateToFit)
            {
                for (int i = 0; i < 3; i++)
                {
                    var asm = assemblies.Last();
                    var rotatedAsm = GridMarkerGenPatternAssemblyBuilder.RotateAssembly90(asm);
                    assemblies.Add(rotatedAsm);
                }
            }

            // Bring the markers to origin (by inverse multiplying it from the dungeon marker position
            TransformMarkers(dungeonTransform.inverse, oldMarkers);

            var boundsExpansion = gridPattern.expandMarkerDomain ? gridPattern.expandMarkerDomainAmount : 0;
            var markerList = new GridSceneMarkerList(gridSize, oldMarkers, boundsExpansion);
            var cellHeights = new GridSceneCellHeights(markerList, markerList.WorldOffset, markerList.WorldSize);

            var worldOffset = markerList.WorldOffset;
            var worldSize = markerList.WorldSize;

            var occupancyList = new GridSceneCellsBool(worldOffset, worldSize);

            var patternMatchCommands = new List<PatternMatchCoord>();
            for (int asmIdx = 0; asmIdx < assemblies.Count; asmIdx++) {
                var patternAsm = assemblies[asmIdx];
                GeneratePatternMatchCommands(patternAsm, asmIdx, worldOffset, worldSize, patternMatchCommands);
            }

            if (gridPattern.randomizeFittingOrder) {
                MathUtils.Shuffle(patternMatchCommands, random);
            }

            var matchedCommands = new List<PatternMatchCoord>();
            foreach (var cmd in patternMatchCommands) {
                if (gridPattern.probability > 0)
                {
                    var patternSettings = new ExecutePatternSettings()
                    {
                        SameHeightMarkers = gridPattern.sameHeightMarkers,
                        AllowInsertionOverlaps = gridPattern.allowInsertionOverlaps
                    };

                    if (ShouldExecutePattern(cmd.X, cmd.Y, assemblies[cmd.AsmIdx], patternSettings, occupancyList, markerList, cellHeights))
                    {
                        matchedCommands.Add(cmd);
                        
                        // Mark the occupied cells
                        MarkPatternOccupancy(cmd.X, cmd.Y, assemblies[cmd.AsmIdx], occupancyList);
                    }
                }
            }

            foreach (var cmd in matchedCommands)
            {
                if (random.NextFloat() <= gridPattern.probability)
                {
                    ExecutePattern(cmd.X, cmd.Y, assemblies[cmd.AsmIdx], markerList, cellHeights);
                }
            }

            newMarkers = markerList.GenerateMarkerList();
            TransformMarkers(dungeonTransform, newMarkers);
            return true;
        }

        struct ExecutePatternSettings
        {
            public bool AllowInsertionOverlaps;
            public string[] SameHeightMarkers;

        }

        private bool ShouldExecutePattern(int baseX, int baseY, GridMarkerGenPatternAssembly asm, ExecutePatternSettings patternSettings,
            GridSceneCellsBool occupancyList,
            GridSceneMarkerList markerList, GridSceneCellHeights cellHeights)
        {
            bool shouldExecutePattern = true;

            // We'll insert a geometry here.  Make sure it wasn't inserted already in this layer
            if (!patternSettings.AllowInsertionOverlaps)
            {
                foreach (var asmRule in asm.Rules)
                {
                    if (asmRule.Rule.IsAssetInsertedHere())
                    {
                        var worldCoord = new Vector2Int(baseX, baseY) + asmRule.Coord;
                        var cellOccupancy = occupancyList.GetValue(worldCoord);
                        if (cellOccupancy != null && cellOccupancy.IsOccupied(asmRule.RuleType))
                        {
                            // This insertion will overlap with a previous geometry we inserted in this layer
                            shouldExecutePattern = false;
                            break;
                        }
                    }
                }
            }

            // Check if the SameHeight constraint passes
            if (shouldExecutePattern && patternSettings.SameHeightMarkers.Length > 0)
            {
                var markersToCheck = new HashSet<string>(patternSettings.SameHeightMarkers);
                var markerHeights = new Dictionary<string, int>();
                foreach (var rule in asm.Rules)
                {
                    var worldCoord = new Vector2Int(baseX, baseY) + rule.Coord;
                    var cell = markerList.GetValue(worldCoord);
                    if (cell != null)
                    {
                        var cellMarkers = cell.GetMarkerList(rule.RuleType);
                        if (cellMarkers != null)
                        {
                            foreach (var markerInfo in cellMarkers)
                            {
                                if (markersToCheck.Contains(markerInfo.SocketType))
                                {
                                    var position = Matrix.GetTranslation(ref markerInfo.Transform);
                                    int markerCoordY = Mathf.RoundToInt(position.y / gridSize.y);
                                    if (markerHeights.ContainsKey(markerInfo.SocketType))
                                    {
                                        int existingCoordY = markerHeights[markerInfo.SocketType];
                                        if (markerCoordY != existingCoordY)
                                        {
                                            shouldExecutePattern = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        markerHeights.Add(markerInfo.SocketType, markerCoordY);
                                    }
                                }
                            }
                        }
                    }

                    if (!shouldExecutePattern)
                    {
                        break;
                    }
                }
            }

            if (shouldExecutePattern)
            {
                foreach (var ruleAsm in asm.Rules)
                {
                    var coord = new Vector2Int(baseX, baseY) + ruleAsm.Coord;
                    var coordType = ruleAsm.RuleType;
                    var apiSettings = new GridMarkerGenVmAPISettings()
                    {
                        Coord = coord,
                        CoordType = coordType,
                        MarkerList = markerList,
                        DungeonTransform = dungeonTransform,
                        GridSize = gridSize,
                        Builder = dungeonBuilder,
                        Model = dungeonModel,
                        Config = dungeonConfig,
                        Query = dungeonQuery,
                        CellHeights = cellHeights,
                        ScriptInstanceCache = scriptCache
                    };
                    var api = new GridMarkerGenVmAPI(apiSettings);

                    var vm = new MarkerGenVM(api);

                    // Run the condition rule program
                    if (!vm.Run(ruleAsm.Rule.program, out var ruleGraphResult))
                    {
                        // The program didn't run successfully (i.e. didn't execute properly in the vm for some reason).  set the graph result to false
                        ruleGraphResult = false;
                    }

                    if (!ruleGraphResult)
                    {
                        // The rule graph did not pass. do not execute the actions on this pattern
                        shouldExecutePattern = false;
                        break;
                    }
                }
            }

            return shouldExecutePattern;
        }
        
        private void ExecutePattern(int baseX, int baseY, GridMarkerGenPatternAssembly asm, GridSceneMarkerList markerList, GridSceneCellHeights cellHeights)
        {
            var actionApi = new GridMarkerGenActionAPI(markerList, cellHeights);
            
            foreach (var ruleAsm in asm.Rules)
            {
                if (ruleAsm != null && ruleAsm.Rule != null && ruleAsm.Rule.actions != null && ruleAsm.Rule.actions.actionList != null)
                {
                    actionApi.State = new GridRuleActionAPIState
                    {
                        Coord = new Vector2Int(baseX, baseY) + ruleAsm.Coord,
                        CoordType = ruleAsm.RuleType,
                        BaseAngleRad = asm.RotationAngleRad,
                        Rotation90Index = asm.Rotation90Index
                    };
                    
                    foreach (var actionInfo in ruleAsm.Rule.actions.actionList)
                    {
                        if (actionInfo is MarkerGenRuleActionInfoAddMarker addMarkerAction)
                        {
                            actionApi.AddMarker(addMarkerAction);
                        }
                        else if (actionInfo is MarkerGenRuleActionInfoRemoveMarker removeMarkerAction)
                        {
                            actionApi.RemoveMarker(removeMarkerAction);
                        }
                    }
                }
            }
        }

        private void MarkPatternOccupancy(int baseX, int baseY, GridMarkerGenPatternAssembly asm, GridSceneCellsBool occupancyList)
        {
            foreach (var asmRule in asm.Rules) {
                if (asmRule.Rule.IsAssetInsertedHere())
                {
                    var worldCoord = new Vector2Int(baseX, baseY) + asmRule.Coord;
                    var cellOccupancy = occupancyList.GetValue(worldCoord);
                    cellOccupancy?.SetOccupied(asmRule.RuleType, true);
                }
            }
        }
        
        private class GridCellHeightCoords
        {
            public int GroundCoordY = 0;
            public int CornerCoordY = 0;
            public int EdgeXCoordY = 0;
            public int EdgeZCoordY = 0;
	
            public int GetHeight(GridMarkerGenRuleType coordType) {
                if (coordType == GridMarkerGenRuleType.Ground) {
                    return GroundCoordY;
                }
                else if (coordType == GridMarkerGenRuleType.Corner) {
                    return CornerCoordY;
                }
                else if (coordType == GridMarkerGenRuleType.EdgeX) {
                    return EdgeXCoordY;
                }
                else if (coordType == GridMarkerGenRuleType.EdgeZ) {
                    return EdgeZCoordY;
                }
                else {
                    Debug.LogError("Unsupported rule type"); // Not implemented
                    return 0;
                }
            }
        };
        
        private struct GridRuleActionAPIState {
            public Vector2Int Coord;
            public GridMarkerGenRuleType CoordType;
            public float BaseAngleRad;
            public int Rotation90Index;
        };

        class GridSceneCellHeights : GridSceneCells<GridCellHeightCoords>
        {
            struct QueueItem {
                public Vector2Int Coord;
                public int CoordY;
            };
            
            public GridSceneCellHeights(GridSceneMarkerList markerList, Vector2Int worldOffset, Vector2Int worldSize)
                : base(worldOffset, worldSize)
            {
                var cellHeight = Mathf.Max(1.0f, markerList.CellHeight);
                
                var visited = new HashSet<Vector2Int>();
                var queue = new Queue<QueueItem>();

                var coordStart = markerList.WorldOffset;
                var coordEnd = markerList.WorldOffset + markerList.WorldSize;
                
                // Run through the initial ground tiles and fill up the queue
                {
                
                    for (int z = coordStart.y; z < coordEnd.y; z++) {
                        for (int x = coordStart.x; x < coordEnd.x; x++) {
                            var cellCoord = new Vector2Int(x, z);
                            
                            var cell = markerList.GetValue(cellCoord);
                            if (cell == null || cell.TileMarkers.Count == 0) continue;
					
                            var groundLocation = Matrix.GetTranslation(ref cell.TileMarkers[0].Transform);
                            var coordY = Mathf.RoundToInt(groundLocation.y / cellHeight);
                            visited.Add(cellCoord);
                            queue.Enqueue(new QueueItem()
                            {
                                Coord = cellCoord,
                                CoordY = coordY
                            });
                        }
                    }
                }
                
                // Run a flood fill algorithm on the initial ground tiles
                {
                    var neighborDeltas = new Vector2Int[] {
                        new Vector2Int(-1, 0),
                        new Vector2Int(1, 0),
                        new Vector2Int(0, -1),
                        new Vector2Int(0, 1)
                    };
                    
                    while (queue.Count > 0)
                    {
                        var front = queue.Dequeue();
                        var cellCoordsZ = GetValue(front.Coord);
                        if (cellCoordsZ != null)
                        {
                            cellCoordsZ.GroundCoordY = front.CoordY;

                            // Add the neighbors
                            for (int d = 0; d < 4; d++)
                            {
                                var neighborCoord = front.Coord + neighborDeltas[d];
                                if (IsCoordValid(neighborCoord) && !visited.Contains(neighborCoord))
                                {
                                    visited.Add(neighborCoord);
                                    queue.Enqueue(new QueueItem()
                                    {
                                        Coord = neighborCoord,
                                        CoordY = front.CoordY
                                    });
                                }
                            }
                        }
                    }
                }

                // Fill up the rest of the coords (edges, corners) based on the ground tiles
                {
                    for (int z = coordStart.y; z < coordEnd.y; z++) {
                        for (int x = coordStart.x; x < coordEnd.x; x++) {
                            var baseCoord = new Vector2Int(x, z);
                            var c00 = GetValue(baseCoord + new Vector2Int(0, 0));
                            var c10 = GetValue(baseCoord + new Vector2Int(-1, 0));
                            var c01 = GetValue(baseCoord + new Vector2Int(0, -1));
                            var c11 = GetValue(baseCoord + new Vector2Int(-1, -1));
					
                            var xEdgeY = c00.GroundCoordY;
                            if (c01 != null)
                            {
                                xEdgeY = Mathf.Max(xEdgeY, c01.GroundCoordY);
                            }

                            var zEdgeY = c00.GroundCoordY;
                            if (c10 != null)
                            {
                                zEdgeY = Mathf.Max(xEdgeY, c10.GroundCoordY);
                            }

                            var cornerY = Mathf.Max(xEdgeY, zEdgeY);
                            if (c11 != null)
                            {
                                cornerY = Mathf.Max(cornerY, c11.GroundCoordY);
                            }

                            c00.EdgeXCoordY = xEdgeY;
                            c00.EdgeZCoordY = zEdgeY;
                            c00.CornerCoordY = cornerY;
                        }
                    }
                }
            }
        }
        
        private class GridMarkerGenActionAPI
        {
            public GridRuleActionAPIState State { get; set; }
            private GridSceneMarkerList markerList;
            private GridSceneCellHeights cellHeights;

            public GridMarkerGenActionAPI(GridSceneMarkerList markerList, GridSceneCellHeights cellHeights)
            {
                this.markerList = markerList;
                this.cellHeights = cellHeights;
            }

            public void AddMarker(MarkerGenRuleActionInfoAddMarker action)
            {
                if (markerList == null || action == null)
                {
                    return;
                }

                var cell = markerList.GetValue(State.Coord);
                if (cell == null)
                {
                    return;
                }

                if (cell.Contains(action.markerName, State.CoordType))
                {
                    // Already contains a marker
                    return;
                }
                
                var heights = cellHeights.GetValue(State.Coord);
                Debug.Assert(heights != null);
                
                var coordY = heights.GetHeight(State.CoordType);

                bool useRotationOverride = false;
                var rotationOverride = Quaternion.identity;
                if (action.copyRotationFromMarkers.Length > 0) {
                    var copyRotMarkerSet = new HashSet<string>(action.copyRotationFromMarkers);
                    var cellMarkerList = cell.GetMarkerList(State.CoordType);
                    if (cellMarkerList != null) {
                        foreach (var markerInfo in cellMarkerList) {
                            if (copyRotMarkerSet.Contains(markerInfo.SocketType)) {
                                rotationOverride = Matrix.GetRotation(ref markerInfo.Transform);
                                useRotationOverride = true;
                                break;
                            }
                        }
                    }
                }

                var markerRotation = useRotationOverride ? rotationOverride : GetWorldRotation();
				
                // Calculate the transform
                Matrix4x4 transform;
                {
                    markerList.GetCellToWorldCoords(State.Coord, coordY, State.CoordType, out var worldLocation);

                    if (action.copyHeightFromMarkers.Length > 0) {
                        var copyHeightMarkerSet = new HashSet<string>(action.copyHeightFromMarkers);
                        var cellMarkerList = cell.GetMarkerList(State.CoordType);
                        if (cellMarkerList != null) {
                            foreach (var markerInfo in cellMarkerList) {
                                if (copyHeightMarkerSet.Contains(markerInfo.SocketType)) {
                                    worldLocation.y = Matrix.GetTranslation(ref markerInfo.Transform).y;
                                    break;
                                }
                            }
                        }
                    }
					
                    transform = Matrix4x4.TRS(worldLocation, markerRotation, Vector3.one);
                }
				
                var mewMarker = new PropSocket
                {
                    Id = markerList.GenerateNextMarkerId(),
                    SocketType = action.markerName,
                    Transform = transform
                };

                cell.Add(mewMarker, State.CoordType);
            }

            public void RemoveMarker(MarkerGenRuleActionInfoRemoveMarker action)
            {
                if (markerList != null)
                {
                    var cell = markerList.GetValue(State.Coord);
                    if (cell != null)
                    {
                        cell.Remove(action.markerName, State.CoordType);
                    }
                }
            }
            
            Quaternion GetWorldRotation() {
                float angle = State.BaseAngleRad;
                if (State.Rotation90Index == 0 && State.CoordType == GridMarkerGenRuleType.EdgeZ)
                {
                    angle += 90; //Mathf.PI * 0.5f;
                }
                else if (State.Rotation90Index == 2 && State.CoordType == GridMarkerGenRuleType.EdgeZ)
                {
                    angle += 270; //Mathf.PI * 1.5f;
                }
                else if (State.Rotation90Index == 1 && State.CoordType == GridMarkerGenRuleType.EdgeX)
                {
                    angle += 90; //Mathf.PI * 0.5f;
                }
                else if (State.Rotation90Index == 3 && State.CoordType == GridMarkerGenRuleType.EdgeX)
                {
                    angle += 270; //Mathf.PI * 1.5f;
                }

                return Quaternion.AngleAxis(angle, Vector3.up);
            }

        }


        struct GridMarkerGenVmAPISettings
        {
            public Vector2Int Coord;
            public GridMarkerGenRuleType CoordType;
            public GridSceneMarkerList MarkerList;
            public Matrix4x4 DungeonTransform;
            public Vector3 GridSize;
            public DungeonBuilder Builder;
            public DungeonModel Model;
            public DungeonConfig Config;
            public DungeonQuery Query;
            public GridSceneCellHeights CellHeights;
            public ScriptInstanceCache<GridMarkerGenRuleUserScript> ScriptInstanceCache;
        }
        
        private class GridMarkerGenVmAPI : IMarkerGenVmAPI
        {
            private GridMarkerGenVmAPISettings settings;
            public GridMarkerGenVmAPI(GridMarkerGenVmAPISettings settings)
            {
                this.settings = settings;
            }

            public bool MarkerExists(string markerName)
            {
                if (settings.MarkerList != null)
                {
                    var cell = settings.MarkerList.GetValue(settings.Coord);
                    if (cell != null)
                    {
                        return cell.Contains(markerName, settings.CoordType);
                    }
                }

                return false;
            }

            Vector3 GetWorldPosition()
            {
                var heights = settings.CellHeights.GetValue(settings.Coord);
                var heightY = heights?.GetHeight(settings.CoordType) ?? 0;
                var coordF = new Vector3(settings.Coord.x, heightY, settings.Coord.y);
                var worldPos = Vector3.Scale(coordF, settings.GridSize);
                return settings.DungeonTransform * worldPos;
            }
            
            public bool ConditionScript(string scriptPath)
            {
                var script = settings.ScriptInstanceCache.GetScript(scriptPath);
                if (script == null)
                {
                    return false;
                }

                var validateParams = new GridMarkerGenRuleUserScript.ValidateSettings()
                {
                    Position = GetWorldPosition(),
                    Coord = settings.Coord,
                    CoordType = settings.CoordType,
                    DungeonTransform = settings.DungeonTransform,
                    Builder = settings.Builder,
                    Config = settings.Config,
                    Model = settings.Model,
                    Query = settings.Query
                };
                
                return script.Validate(validateParams);
            }
        }
        
        private void GeneratePatternMatchCommands(GridMarkerGenPatternAssembly asm, int asmIdx, Vector2Int worldOffset, Vector2Int worldSize, List<PatternMatchCoord> commands)
        {
            if (asm.Rules.Length == 0) {
                return;
            }

            var patternSize = asm.BoundsMax - asm.BoundsMin + new Vector2Int(1, 1);
            var startCoord = worldOffset - patternSize;
            var endCoord = worldOffset + worldSize;
	
            for (int y = startCoord.y; y <= endCoord.y; y++) {
                for (int x = startCoord.x; x <= endCoord.x; x++) {
                    commands.Add(new PatternMatchCoord
                    {
                        X = x, 
                        Y = y, 
                        AsmIdx = asmIdx
                    });
                }
            }
        }

        private bool ShouldProcessAssembly(GridMarkerGenPattern pattern)
        {
            if (pattern == null || pattern.rules.Length == 0)
            {
                return false;
            }

            foreach (var rule in pattern.rules)
            {
                if (rule == null || rule.program == null || !rule.program.compiled)
                {
                    continue;
                }

                if (rule.actions != null && rule.actions.actionList != null && rule.actions.actionList.Length > 0)
                {
                    // We have actions defined for this rule.  We need to process this pattern
                    return true;
                }
            }

            return false;
        }

        private void TransformMarkers(Matrix4x4 transform, PropSocket[] markers)
        {
            foreach (var marker in markers)
            {
                marker.Transform = transform * marker.Transform;
            }
        }
    }
}
