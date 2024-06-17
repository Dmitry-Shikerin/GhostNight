using System;
using System.Collections.Generic;
using System.Linq;
using DungeonArchitect.Editors.MarkerGenerator.TextGen;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Nodes.Actions;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using DungeonArchitect.MarkerGenerator.Nodes.Condition;
using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.MarkerGenerator.VM;
using DungeonArchitect.UI;
using DungeonArchitect.UI.Widgets.GraphEditors;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonArchitect.Editors.MarkerGenerator
{
    public class MarkerGenEditorUtils
    {
        public static void InitAsset(MarkerGeneratorAsset asset, UIPlatform platform)
        {
            // TODO: initialize me
        }

        public static MarkerGenPattern AddNewPattern(System.Type patternType, MarkerGeneratorAsset asset, UIPlatform platform)
        {
            if (asset == null) return null;

            var pattern = CreateAssetObject(patternType, asset, platform) as MarkerGenPattern;
            if (pattern != null)
            {
                pattern.patternName = "Pattern";
                asset.patterns = asset.patterns.Concat(new[] { pattern }).ToArray();
            }

            return pattern;
        }
        
        public static T AddNewPattern<T>(MarkerGeneratorAsset asset, UIPlatform platform) where T : MarkerGenPattern
        {
            return AddNewPattern(typeof(T), asset, platform) as T;
        }
        
        public static void RemovePattern(MarkerGeneratorAsset asset, MarkerGenPattern pattern)
        {
            if (asset == null) return;

            asset.patterns = asset.patterns.Where(p => p != pattern).ToArray();

            // Destroy the pattern owned objects
            {
                foreach (var rule in pattern.rules)
                {
                    DestroyAssetObject(rule.ruleGraph);
                    DestroyAssetObject(rule.program);
                    DestroyAssetObject(rule.actions);
                    DestroyAssetObject(rule);
                }

                pattern.rules = Array.Empty<MarkerGenRule>();
            }

            DestroyAssetObject(pattern);
        }
        
        public static T AddNewRule<T>(MarkerGeneratorAsset asset, MarkerGenPattern pattern, UIPlatform platform) where T : MarkerGenRule
        {
            return AddNewRule(typeof(T), asset, pattern, platform) as T;
        }

        public static MarkerGenRule AddNewRule(System.Type ruleType, MarkerGeneratorAsset asset, MarkerGenPattern pattern, UIPlatform platform)
        {
            if (pattern == null) return null;

            var rule = CreateAssetObject(ruleType, asset, platform) as MarkerGenRule;
            if (rule != null)
            {
                rule.ruleGraph = CreateAssetObject<MarkerGenRuleGraph>(asset, platform);
                rule.program = CreateAssetObject<MarkerGenRuleProgram>(asset, platform);
                rule.actions = CreateAssetObject<MarkerGenRuleActionList>(asset, platform);
                InitRuleGraph(rule.ruleGraph, asset, platform);
                CompileRule(rule, asset, platform);
                pattern.rules = pattern.rules.Concat(new []{ rule }).ToArray();
            }
            
            return rule;
        }

        public static void CompileRule(MarkerGenRule rule, MarkerGeneratorAsset asset, UIPlatform platform)
        {
            if (rule == null || asset == null)
            {
                return;
            }
            
            if (rule.ruleGraph == null) { rule.ruleGraph = CreateAssetObject<MarkerGenRuleGraph>(asset, platform); }
            if (rule.program == null) { rule.program = CreateAssetObject<MarkerGenRuleProgram>(asset, platform); }
            if (rule.actions == null) { rule.actions = CreateAssetObject<MarkerGenRuleActionList>(asset, platform); }
            
            // Compile the condition graph
            ConditionGraphCompiler.Compile(rule.ruleGraph, rule.program);
            
            // Compile the action graph
            if (asset != null && platform != null)
            {
                ActionGraphCompiler.Compile(rule.ruleGraph, rule.actions);
                
                // Add the actions to the asset
                foreach (var action in rule.actions.actionList)
                {
                    action.hideFlags = HideFlags.HideInHierarchy;
                    platform.AddObjectToAsset(action, asset);
                }
            }
            
            MarkerGenRuleTextGenerator.GenerateRulePreviewText(rule);
        }

        
        private static void InitRuleGraph(MarkerGenRuleGraph ruleGraph, MarkerGeneratorAsset asset, UIPlatform platform)
        {
            // Add the result node
            var resultNode = GraphOperations.CreateNode(ruleGraph, typeof(MarkerGenRuleNodeResult), null);
            GraphEditorUtils.AddToAsset(platform, asset, resultNode);
            resultNode.Position = Vector2.zero;
            
            var execNode = GraphOperations.CreateNode(ruleGraph, typeof(MarkerGenRuleNodeOnPass), null);
            GraphEditorUtils.AddToAsset(platform, asset, execNode);
            execNode.Position = new Vector2(120, 0);

            ruleGraph.resultNode = resultNode as MarkerGenRuleNodeResult;
            ruleGraph.passNode = execNode as MarkerGenRuleNodeOnPass;
        }

        public static void RemoveRule(MarkerGeneratorAsset asset, MarkerGenPattern pattern, MarkerGenRule rule)
        {
            if (asset == null || pattern == null || rule == null)
            {
                Debug.LogError("RemoveRule: Invalid rule parameter");
                return;
            }
            DestroyAssetObject(rule.ruleGraph);
            rule.ruleGraph = null;

            pattern.rules = pattern.rules.Where(r => r != rule).ToArray();

            // Remove the owned objects
            DestroyAssetObject(rule);
        }
        
        
        private static T CreateAssetObject<T>(MarkerGeneratorAsset asset, UIPlatform platform) where T : ScriptableObject
        {
            return CreateAssetObject(typeof(T), asset, platform) as T;
        }

        private static ScriptableObject CreateAssetObject(System.Type type, MarkerGeneratorAsset asset, UIPlatform platform)
        {
            var obj = ScriptableObject.CreateInstance(type);
            obj.hideFlags = HideFlags.HideInHierarchy;
            platform.AddObjectToAsset(obj, asset);
            return obj;
        }
        
        private static void DestroyAssetObject<T>(T obj) where T : ScriptableObject
        {
            //AssetDatabase.RemoveObjectFromAsset(obj);
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
            }
        }

        public static Color CreateRandomColor()
        {
            var hue = Random.value;
            var saturation = 0.5f;
            var value = 1.0f;
            return Color.HSVToRGB(hue, saturation, value);
        }
    }
}