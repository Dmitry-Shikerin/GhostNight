using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator.Grid;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Rule.Grid.Assemblies
{
    public class GridMarkerGenRuleAssembly
    {
        public Vector2Int Coord;
        public GridMarkerGenRuleType RuleType;
        public GridMarkerGenRule Rule;
        public bool HintWillInsertAssetHere;


        public GridMarkerGenRuleAssembly Clone()
        {
            return new GridMarkerGenRuleAssembly()
            {
                Coord = Coord,
                RuleType = RuleType,
                Rule = Rule,
                HintWillInsertAssetHere = HintWillInsertAssetHere,
            };
        }
    }
    
    public class GridMarkerGenPatternAssembly
    {
        public GridMarkerGenRuleAssembly[] Rules = System.Array.Empty<GridMarkerGenRuleAssembly>();
        public Vector2Int BoundsMin = Vector2Int.zero;
        public Vector2Int BoundsMax = Vector2Int.zero;
        public float RotationAngleRad = 0;
        public int Rotation90Index = 0;

        public GridMarkerGenPatternAssembly()
        {
        }
        
        public GridMarkerGenPatternAssembly(GridMarkerGenPatternAssembly other)
        {
            BoundsMin = other.BoundsMin;
            BoundsMax = other.BoundsMax;
            RotationAngleRad = other.RotationAngleRad;
            Rotation90Index = other.Rotation90Index;
            Rules = new GridMarkerGenRuleAssembly[other.Rules.Length];
            for (int i = 0; i < Rules.Length; i++)
            {
                Rules[i] = other.Rules[i].Clone();
            }
        }
    }

    public class GridMarkerGenPatternAssemblyBuilder
    {
        public static GridMarkerGenPatternAssembly GenerateAssembly(GridMarkerGenPattern pattern)
        {
            var asm = new GridMarkerGenPatternAssembly();
            var asmRules = new List<GridMarkerGenRuleAssembly>();
            foreach (var rule in pattern.rules)
            {
                if (rule is GridMarkerGenRule gridRule)
                {
                    var asmRule = new GridMarkerGenRuleAssembly
                    {
                        Coord = gridRule.coord,
                        RuleType = gridRule.ruleType,
                        HintWillInsertAssetHere = gridRule.hintWillInsertAssetHere,
                        Rule = gridRule
                    };

                    if (!asmRule.HintWillInsertAssetHere)
                    {
                        // Check if we have a "Add Marker" action node in the rule graph
                        foreach (var actionInfo in rule.actions.actionList)
                        {
                            if (actionInfo is MarkerGenRuleActionInfoAddMarker)
                            {
                                asmRule.HintWillInsertAssetHere = true;
                            }
                        }
                    }

                    asmRules.Add(asmRule);
                }
            }

            asm.Rules = asmRules.ToArray();
            UpdateBounds(ref asm);

            return asm;
        }

        public static GridMarkerGenPatternAssembly RotateAssembly90(GridMarkerGenPatternAssembly assembly)
        {
            var rotatedAssembly = new GridMarkerGenPatternAssembly(assembly);
            rotatedAssembly.RotationAngleRad += 90;
            rotatedAssembly.Rotation90Index++;

            var rotation = Quaternion.AngleAxis(90, Vector3.up);
            foreach (var rotatedRule in rotatedAssembly.Rules)
            {
                if (rotatedRule.RuleType == GridMarkerGenRuleType.Corner) {
                    Vector3 offset = new Vector3(0.5f, 0.0f, 0.5f);
                    Vector3 oldLocation = new Vector3(rotatedRule.Coord.x, 0, rotatedRule.Coord.y) - offset;
                    Vector3 newLocation = rotation * oldLocation + offset;
                    rotatedRule.Coord = new Vector2Int(
                        Mathf.RoundToInt(newLocation.x),
                        Mathf.RoundToInt(newLocation.z));
                }
                else if (rotatedRule.RuleType == GridMarkerGenRuleType.Ground) {
                    Vector3 oldLocation = new Vector3(rotatedRule.Coord.x, 0, rotatedRule.Coord.y);
                    Vector3 newLocation = rotation * oldLocation;
                    rotatedRule.Coord = new Vector2Int(
                        Mathf.RoundToInt(newLocation.x),
                        Mathf.RoundToInt(newLocation.z));
                }
                else if (rotatedRule.RuleType == GridMarkerGenRuleType.EdgeX) {
                    Vector3 oldLocation = new Vector3(rotatedRule.Coord.x, 0, rotatedRule.Coord.y - 0.5f);
                    Vector3 newLocation = rotation * oldLocation + new Vector3(0.5f, 0, 0);;
                    rotatedRule.RuleType = GridMarkerGenRuleType.EdgeZ;
                    rotatedRule.Coord = new Vector2Int(
                        Mathf.RoundToInt(newLocation.x),
                        Mathf.RoundToInt(newLocation.z));
                }
                else if (rotatedRule.RuleType == GridMarkerGenRuleType.EdgeZ) {
                    Vector3 oldLocation = new Vector3(rotatedRule.Coord.x - 0.5f, 0, rotatedRule.Coord.y);
                    Vector3 newLocation = rotation * oldLocation + new Vector3(0, 0, 0.5f);;
                    rotatedRule.RuleType = GridMarkerGenRuleType.EdgeX;
                    rotatedRule.Coord = new Vector2Int(
                        Mathf.RoundToInt(newLocation.x),
                        Mathf.RoundToInt(newLocation.z));
                }
                else {
                    // Not supported
                    Debug.Assert(false, "Invalid state");
                }
            }

            return rotatedAssembly;
        }

        public static void UpdateBounds(ref GridMarkerGenPatternAssembly assembly)
        {
            if (assembly.Rules.Length == 0)
            {
                assembly.BoundsMin = Vector2Int.zero;
                assembly.BoundsMax = Vector2Int.zero;
                return;
            }

            assembly.BoundsMin = assembly.Rules[0].Coord;
            assembly.BoundsMax = assembly.Rules[0].Coord;
            foreach (var rule in assembly.Rules)
            {
                assembly.BoundsMin.x = Mathf.Min(assembly.BoundsMin.x, rule.Coord.x);
                assembly.BoundsMin.y = Mathf.Min(assembly.BoundsMin.y, rule.Coord.y);
                assembly.BoundsMax.x = Mathf.Max(assembly.BoundsMax.x, rule.Coord.x);
                assembly.BoundsMax.y = Mathf.Max(assembly.BoundsMax.y, rule.Coord.y);
            }
        }
    }
}