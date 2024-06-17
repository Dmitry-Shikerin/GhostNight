using DungeonArchitect.Editors.MarkerGenerator;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Grid;
using DungeonArchitect.MarkerGenerator.Nodes.Actions;
using DungeonArchitect.MarkerGenerator.Nodes.Condition;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using DungeonArchitect.Utils;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGen
{
    public class MarkerGenInspectorBase : DAInspectorBase
    {
        
        protected virtual void HandleInspectorGUI()
        {
        }

        public override void OnInspectorGUI()
        {
            sobject.Update();

            HandleInspectorGUI();

            InspectorNotify.Dispatch(sobject, target);
        }
    }

    [CustomEditor(typeof(MarkerGenRuleNodeMarkerExists), false)]
    public class MarkerGenRuleMarkerExistsInspector : MarkerGenInspectorBase
    {
        protected override void HandleInspectorGUI()
        {
            DrawHeader("Marker Exists");
            {
                EditorGUI.indentLevel++;
                DrawProperties("markerName");
                EditorGUI.indentLevel--;
            }
            
        }
    }
    
    [CustomEditor(typeof(MarkerGenRuleNodeConditionScript), false)]
    public class MarkerGenRuleScriptInspector : MarkerGenInspectorBase
    {
        InstanceCache instanceCache = new InstanceCache();
        protected override void HandleInspectorGUI()
        {
            var scriptNode = target as MarkerGenRuleNodeConditionScript;
            if (scriptNode == null)
            {
                return;
            }
            
            DrawHeader("Script Node");
            {
                EditorGUI.indentLevel++;
                var oldClassName = scriptNode.scriptClassName;
                DrawRule<GridMarkerGenRuleUserScript>("Script", oldClassName, out var newClassName);
                if (oldClassName != newClassName)
                {
                    var propClassName = GetProperty("scriptClassName");
                    propClassName.stringValue = newClassName;
                }
                EditorGUILayout.HelpBox("Specify a script that inherits from GridMarkerGenRuleUserScript. Override the method ValidateRule with your own logic", MessageType.Info);
                EditorGUI.indentLevel--;
            }
        }
        
        void DrawRule<T>(string caption, string ruleClassName, out string newRuleClassName) where T : ScriptableObject
        {
            newRuleClassName = ruleClassName;
            
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            MonoScript script = null;
            EditorGUILayout.LabelField(caption, new GUILayoutOption[] { GUILayout.MaxWidth(80) });
            if (!string.IsNullOrEmpty(ruleClassName))
            {
                var rule = instanceCache.GetInstance(ruleClassName) as ScriptableObject;
                if (rule != null)
                {
                    script = MonoScript.FromScriptableObject(rule);
                } 
            }
            var oldScript = script;
            script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;
            if (oldScript != script && script != null)
            {
                var ruleType = script.GetClass();
                if (ruleType.IsSubclassOf(typeof(T)))
                {
                    newRuleClassName = script.GetClass().AssemblyQualifiedName;
                }
                else
                {
                    newRuleClassName = null;
                }
            }
            else if (script == null)
            {
                newRuleClassName = null;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
    
    [CustomEditor(typeof(MarkerGenRuleNodeAddMarker), false)]
    public class MarkerGenRuleAddMarkerInspector : MarkerGenInspectorBase
    {
        protected override void HandleInspectorGUI()
        {
            DrawHeader("Add Marker");
            {
                EditorGUI.indentLevel++;
                DrawProperties("markerName");
                DrawProperty("copyRotationFromMarkers", true);
                DrawProperty("copyHeightFromMarkers", true);
                EditorGUI.indentLevel--;
            }
            
        }
    }
    
    [CustomEditor(typeof(MarkerGenRuleNodeRemoveMarker), false)]
    public class MarkerGenRuleRemoveMarkerInspector : MarkerGenInspectorBase
    {
        protected override void HandleInspectorGUI()
        {
            DrawHeader("Remove Marker Marker");
            {
                EditorGUI.indentLevel++;
                DrawProperties("markerName");
                EditorGUI.indentLevel--;
            }
            
        }
    }
    
    [CustomEditor(typeof(GridMarkerGenPattern), false)]
    public class GridMarkerGenLayerInspector : MarkerGenInspectorBase
    {
        protected override void HandleInspectorGUI()
        {
            DrawHeader("Grid Layer");
            {
                EditorGUI.indentLevel++;
                DrawProperties("patternName", "probability", "rotateToFit", "randomizeFittingOrder", "allowInsertionOverlaps");
                EditorGUI.indentLevel--;
            }

            DrawHeader("Advanced");
            {
                EditorGUI.indentLevel++;
                DrawProperty("sameHeightMarkers", true);
                DrawProperties("expandMarkerDomain", "expandMarkerDomainAmount");
                EditorGUI.indentLevel--;
            }

        }
    }
    
    [CustomEditor(typeof(GridMarkerGenRule), false)]
    public class GridMarkerGenRuleInspector : MarkerGenInspectorBase
    {
        protected override void HandleInspectorGUI()
        {
            DrawHeader("Pattern Rule");
            {
                EditorGUI.indentLevel++;
                DrawProperties("hintWillInsertAssetHere");
                EditorGUI.indentLevel--;
            }
            
            DrawHeader("Misc");
            {
                EditorGUI.indentLevel++;
                DrawProperties("visuallyDominant");
                if (GUILayout.Button("Randomize Color"))
                {
                    if (target is GridMarkerGenRule rule)
                    {
                        var colorProperty = sobject.FindProperty("color");
                        colorProperty.colorValue = MarkerGenEditorUtils.CreateRandomColor(); 
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}