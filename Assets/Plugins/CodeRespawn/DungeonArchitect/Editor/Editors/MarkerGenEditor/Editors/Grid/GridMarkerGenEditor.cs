using System;
using System.Linq;
using DungeonArchitect.Editors.MarkerGenerator.Editors.Grid.Actors;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Grid;
using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.SxEngine;
using DungeonArchitect.SxEngine.Utils;
using DungeonArchitect.UI;
using DungeonArchitect.Utils;
using UnityEditor;
using UnityEngine;
using MathUtils = DungeonArchitect.Utils.MathUtils;

namespace DungeonArchitect.Editors.MarkerGenerator.Editors.Grid
{
    public class GridMarkerGenEditor : MarkerGenEditor
    {
        public override Type PatternType => typeof(GridMarkerGenPattern);
        public override Type RuleType => typeof(GridMarkerGenRule);
        
        private GridMarkerGenPattern gridPattern = null;

        private SxGridMarkerGenGroundActor groundActor;
        private SxCursorActor cursorActor;
        private readonly MouseDeltaTracker deltaTracker;
        private SxGridPatternRuleActor hoveredActor = null;
        private SxGridPatternRuleActor selectedActor = null;
        private SxGridPatternRuleActor draggedActor = null;
        
        
        public GridMarkerGenEditor()
        {
            deltaTracker = new MouseDeltaTracker();
            deltaTracker.OnLeftClick += OnLeftClick;
            deltaTracker.OnRightClick += OnRightClick;
            deltaTracker.OnDrag += OnDrag;
            deltaTracker.OnDragStart += OnDragStart;
            deltaTracker.OnDragEnd += OnDragEnd;
        }

        public override void Update(double frameTime)
        {
            if (draggedActor != null)
            {
                cursorActor.SetSmoothPosition(draggedActor.Position, true);
            }
            if (FrameRequiresRepaint())
            {
                RequestRepaint = true;
            }
        }

        private bool FrameRequiresRepaint()
        {
            if (cursorActor != null && cursorActor.Animating)
            {
                return true;
            }
            
            if (PatternViewport != null && PatternViewport.World != null)
            {
                foreach (var ruleActor in PatternViewport.World.GetActorsOfType<SxGridPatternRuleActor>())
                {
                    if (ruleActor.Animating)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public override void OnRuleGraphChanged(MarkerGenRule rule)
        {
            // Find the rule actor and update the text
            if (PatternViewport != null && PatternViewport.World != null)
            {
                var ruleActors = PatternViewport.World.GetActorsOfType<SxGridPatternRuleActor>();
                foreach (var ruleActor in ruleActors)
                {
                    if (ruleActor.Rule == rule)
                    {
                        ruleActor.UpdateRuleText();
                        PatternViewport.Invalidate();
                        break;
                    }
                }
            }
        }
        
        public override void HandleInput(Event widgetEvent, UISystem uiSystem)
        {
            if (PatternViewport == null)
            {
                return;
            }
            
            SxCamera camera = PatternViewport.Camera;
            deltaTracker.HandleInput(widgetEvent, uiSystem);
            
            // Update the cursor
            if (cursorActor != null)
            {
                if (widgetEvent.type == EventType.MouseMove || (widgetEvent.type == EventType.MouseDrag && draggedActor == null))
                {
                    SxGridPatternRuleActor newHoveredRuleActor = null;
                    if (GetActorUnderMouse(widgetEvent.mousePosition, out var actor, out var intersectionPoint))
                    {
                        if (actor is SxGridMarkerGenGroundActor ground)
                        {
                            Debug.Assert(ground == groundActor);
                            var cursorPosition = MathUtils.ClampToRect(intersectionPoint, groundActor.WorldBounds);
                            
                            // Check if we have a rule actor in this cell
                            SxGridPatternRuleActor cellRuleActor = null;
                            if (groundActor.Deproject(cursorPosition, out var item, out var itemType))
                            {
                                if (PatternViewport != null && PatternViewport.World != null)
                                {
                                    foreach (var ruleActorToTest in PatternViewport.World.GetActorsOfType<SxGridPatternRuleActor>())
                                    {
                                        if (ruleActorToTest.Rule != null)
                                        {
                                            if (ruleActorToTest.Rule.coord == item.Coord && ruleActorToTest.Rule.ruleType == itemType)
                                            {
                                                cellRuleActor = ruleActorToTest;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                if (cellRuleActor == null)
                                {
                                    // Hovered over the grid actor.
                                    cursorActor.SetSmoothPosition(cursorPosition); 
                                    groundActor.OnMouseHover(cursorPosition);
                                }
                                else
                                {
                                    // Make it hover over the rule actor
                                    actor = cellRuleActor;
                                    intersectionPoint = cellRuleActor.Position;
                                }
                            }
                            
                        }
                        
                        if (actor is SxGridPatternRuleActor ruleActor)
                        {
                            if (draggedActor == null || draggedActor == ruleActor)
                            {
                                var cursorPosition = MathUtils.ClampToRect(intersectionPoint, groundActor.WorldBounds);
                                cursorActor.SetSmoothPosition(cursorPosition);
                                groundActor.OnMouseHover(cursorPosition);
                                HandleHoverRuleActor(ruleActor);
                                newHoveredRuleActor = ruleActor;
                            }
                        }
                    }

                    HandleHoverRuleActor(newHoveredRuleActor);

                    RequestRepaint = true;
                }
            }
        }

        private bool GetActorUnderMouse(Vector2 mousePosition, out SxActor actor, out Vector3 intersectionPoint)
        {
            return GetActorUnderMouse(mousePosition, Array.Empty<SxActor>(), out actor, out intersectionPoint);
        }

        public override void HandleRulePropertyChange(MarkerGenRule rule)
        {
            // Update the material and transform of the rule actor
            SxGridPatternRuleActor ruleActor = null;
            if (PatternViewport != null && PatternViewport.World != null)
            {
                foreach (var actor in PatternViewport.World.GetActorsOfType<SxGridPatternRuleActor>())
                {
                    if (actor != null && actor.Rule == rule)
                    {
                        ruleActor = actor;
                        break;
                    }
                }
            }

            if (ruleActor != null)
            {
                ruleActor.UpdateTransform(true);
                ruleActor.UpdateMaterial();
                PatternViewport?.Invalidate();
            }
        }

        private bool GetActorUnderMouse(Vector2 mousePosition, SxActor[] actorsToIgnore, out SxActor actor, out Vector3 intersectionPoint)
        {
            if (PatternViewport != null && PatternViewport.Camera != null && PatternViewport.World != null)
            {
                var camera = PatternViewport.Camera;
                var world = PatternViewport.World;
                var ray = camera.ScreenToRay(mousePosition);
                
                // Check if we hit an object actor
                {
                    SxActor bestActor = null;
                    float bestDistance = float.MaxValue;
                    var bestIntersectionPoint = Vector3.zero;
                    
                    var ruleActors = world.GetActorsOfType<SxGridPatternRuleActor>();
                    foreach (var ruleActor in ruleActors)
                    {
                        if (ruleActor == null || actorsToIgnore.Contains(ruleActor))
                        {
                            continue;
                        }
                        
                        var bounds = ruleActor.GetBounds();
                        if (bounds.IntersectRay(ray, out var intersectionDistance))
                        {
                            if (intersectionDistance < bestDistance)
                            {
                                bestActor = ruleActor;
                                bestIntersectionPoint = ruleActor.WorldTransform.Positon;
                                bestDistance = intersectionDistance;
                            }
                        }
                    }

                    if (bestActor != null)
                    {
                        actor = bestActor;
                        intersectionPoint = bestIntersectionPoint;
                        return true;
                    }
                }

                // Check if we hit a ground actor
                {
                    var plane = new Plane(Vector3.up, Vector3.zero);
                    if (MathUtils.RayPlaneIntersection(ray, plane, out var intersectionDistance))
                    {
                        actor = groundActor;
                        intersectionPoint = ray.GetPoint(intersectionDistance);
                        return true;
                    }
                }
            }
            
            actor = null;
            intersectionPoint = Vector3.zero;
            return false;
        }
        
        public override void LoadScene(MarkerGenPattern pattern, UIPlatform platform)
        {
            if (PatternViewport == null)
            {
                return;
            }
            
            var world = PatternViewport.World;
            cursorActor = world.SpawnActor<SxCursorActor>(true);
            
            gridPattern = pattern as GridMarkerGenPattern;
            if (gridPattern == null)
            {
                return;
            }

            // Build the ground actor
            {
                groundActor = world.SpawnActor<SxGridMarkerGenGroundActor>(true);
                
                var buildSettings = new GridMarkerGenGroundActorSettings();
                const int gridHalfSize = 5;
                buildSettings.Start = new Vector2Int(-gridHalfSize, -gridHalfSize);
                buildSettings.End = new Vector2Int(gridHalfSize, gridHalfSize);
                groundActor.Build(buildSettings);

            }
            
            // Create the rule actors
            {
                foreach (var rule in pattern.rules)
                {
                    if (rule is GridMarkerGenRule gridRule)
                    {
                        CreateRuleActor(world, gridRule);
                    }
                }
            }
            
            // build a grid
            {
                var gridMesh = world.SpawnActor<SxMeshActor>(true);
                gridMesh.SetMesh(SxMeshUtils.CreateGridMesh(10, 1.0f));
                gridMesh.SetMaterial<SxGridMaterial>();
            }
        }

        private void HandleSelectRuleActor(SxGridPatternRuleActor newSelectedActor)
        {
            Selection.activeObject = newSelectedActor?.Rule;
            
            if (selectedActor == newSelectedActor)
            {
                return;
            }

            var oldSelectedActor = selectedActor;
            if (oldSelectedActor != null)
            {
                oldSelectedActor.SetSelected(false);
            }

            if (newSelectedActor != null)
            {
                newSelectedActor.SetSelected(true);
            }
            selectedActor = newSelectedActor;
            
            var selectedRule = selectedActor?.Rule;
            NotifyRuleSelected(selectedRule);
        }
        
        private void HandleHoverRuleActor(SxGridPatternRuleActor newHoveredActor)
        {
            if (hoveredActor == newHoveredActor)
            {
                return;
            }

            var oldHoveredActor = hoveredActor;
            if (oldHoveredActor != null)
            {
                oldHoveredActor.SetHovered(false);
            }

            if (newHoveredActor != null)
            {
                newHoveredActor.SetHovered(true);
            }

            hoveredActor = newHoveredActor;
        }

        private void OnDragStart(Event e, UISystem uiSystem)
        {
            draggedActor = null;
            if (GetActorUnderMouse(e.mousePosition, out var actor, out var intersectionPoint))
            {
                if (actor is SxGridPatternRuleActor ruleActor)
                {
                    draggedActor = ruleActor;
                    HandleSelectRuleActor(draggedActor);
                }
            }
            
        }
        
        private void OnDragEnd(Event e, UISystem uiSystem)
        {
            if (draggedActor != null)
            {
                //draggedActor.UpdateTransform(false);
            }

            draggedActor = null;
        }
        
        private void OnDrag(Event e, UISystem uiSystem)
        {
            if (draggedActor == null || cursorActor == null) return;

            var actorsToIgnore = new SxActor[] { draggedActor };
            if (GetActorUnderMouse(e.mousePosition, actorsToIgnore, out var actor, out var intersection))
            {
                if (actor == groundActor)
                {
                    if (groundActor.Deproject(intersection, out var item, out var itemType))
                    {
                        var existingRule = GetRuleAt(item.Coord, itemType);
                        if (existingRule == null)
                        {
                            // No rule exists at this location. Move the actor here 
                            draggedActor.Rule.coord = item.Coord;
                            draggedActor.Rule.ruleType = itemType;
                            draggedActor.UpdateTransform(true);
                        }
                    }
                }
            }
        }
        
        private void OnLeftClick(Event e, UISystem uiSystem)
        {
            SxGridPatternRuleActor clickedRuleActor = null;
            if (GetActorUnderMouse(e.mousePosition, out var actor, out var intersection))
            {
                if (actor is SxGridPatternRuleActor ruleActor)
                {
                    clickedRuleActor = ruleActor;
                }
            }
            
            HandleSelectRuleActor(clickedRuleActor);
            PatternViewport.Invalidate();
        }

        private void OnRightClick(Event e, UISystem uiSystem)
        {
            if (GetActorUnderMouse(e.mousePosition, out var actor, out var intersection))
            {
                if (actor is SxGridMarkerGenGroundActor ground)
                {
                    if (ground.Deproject(intersection, out var item, out var itemType))
                    {
                        var menu = uiSystem.Platform.CreateContextMenu();
                        menu.AddItem("Add Rule", () =>
                        {
                            AddNewRule(item.Coord, itemType, uiSystem.Platform);
                        });
                        menu.Show();
                    }
                }
                else if (actor is SxGridPatternRuleActor ruleActor)
                {   
                    var menu = uiSystem.Platform.CreateContextMenu();
                    menu.AddItem("Delete Rule", () =>
                    {
                        string message = string.Format("Are you sure you want to delete the selected rule block?");
                        bool removeItem = EditorUtility.DisplayDialog("Delete Rule Block?", message, "Delete", "Cancel");
                        if (removeItem)
                        {
                            DeleteRule(ruleActor, uiSystem.Platform);
                        }
                    });
                    menu.Show();
                }
            }
        }

        void DeleteRule(SxGridPatternRuleActor ruleActor, UIPlatform platform)
        {
            if (ruleActor != null)
            {
                MarkerGenEditorUtils.RemoveRule(Asset, gridPattern, ruleActor.Rule);
                NotifyRuleSelected(ruleActor.Rule);
                ruleActor.Destroy();
            }
        }

        GridMarkerGenRule GetRuleAt(Vector2Int coord, GridMarkerGenRuleType ruleType)
        {
            foreach (var rule in gridPattern.rules)
            {
                if (rule is GridMarkerGenRule gridRule)
                {
                    if (gridRule.coord == coord && gridRule.ruleType == ruleType)
                    {
                        return gridRule;
                    }
                }
            }

            return null;
        }
        
        SxGridPatternRuleActor AddNewRule(Vector2Int coord, GridMarkerGenRuleType ruleType, UIPlatform platform)
        {
            // Make sure a rule with the same coords do not exist
            {
                var existingRule = GetRuleAt(coord, ruleType);
                if (existingRule != null)
                {
                    return null;
                }
            }

            var rule = MarkerGenEditorUtils.AddNewRule<GridMarkerGenRule>(Asset, gridPattern, platform);
            if (rule == null)
            {
                return null;
            }
            
            // Assign a random color to the rule
            rule.color = MarkerGenEditorUtils.CreateRandomColor();

            if (PatternViewport == null || PatternViewport.World == null)
            {
                return null;
            }
            
            rule.coord = coord;
            rule.ruleType = ruleType;

            var ruleActor = CreateRuleActor(PatternViewport.World, rule);
            HandleSelectRuleActor(ruleActor);
            return ruleActor;
        }

        SxGridPatternRuleActor CreateRuleActor(SxWorld world, GridMarkerGenRule rule)
        {
            var ruleActor = world.SpawnActor<SxGridPatternRuleActor>(true);
            ruleActor.Initialize(rule, groundActor.Settings.TileSize, groundActor.Settings.EdgeSize);
            return ruleActor;
        }
    }
}