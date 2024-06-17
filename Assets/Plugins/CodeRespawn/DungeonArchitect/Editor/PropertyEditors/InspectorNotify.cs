//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Collections.Generic;
using DungeonArchitect.Editors.SnapFlow;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Grammar;
using DungeonArchitect.MarkerGenerator;
using DungeonArchitect.MarkerGenerator.Grid;
using DungeonArchitect.MarkerGenerator.Nodes.Actions;
using DungeonArchitect.MarkerGenerator.Nodes.Condition;
using DungeonArchitect.MarkerGenerator.Rule.Grid;
using UnityEditor;

namespace DungeonArchitect.Editors
{
    public class InspectorNotify
    {
        // Delegates
        public delegate void OnFlowTaskPropertyChanged(FlowExecTask task);
        public delegate void OnSnapPropertyChanged(Object obj);

        public delegate void OnMarkerGenPropertyChanged(Object obj);
        
        // Events
        public static event OnFlowTaskPropertyChanged FlowTaskPropertyChanged;
        public static event OnSnapPropertyChanged SnapPropertyChanged;
        public static event OnMarkerGenPropertyChanged MarkerGenPropertyChanged;

        private static readonly HashSet<System.Type> SnapEditorTypes;
        private static readonly HashSet<System.Type> MarkerGenEditorTypes;

        static InspectorNotify()
        {
            SnapEditorTypes = new HashSet<Type>()
            {
                typeof(GrammarExecRuleNode),
                typeof(GrammarGraph),
                typeof(GrammarTaskNode),
                typeof(SnapEdResultGraphEditorConfig),
                typeof(GrammarProductionRule),
                typeof(GrammarNodeType),
                typeof(WeightedGrammarGraph)
            };
            
            MarkerGenEditorTypes = new HashSet<Type>()
            {
                typeof(GridMarkerGenRule),
                typeof(GridMarkerGenPattern),
                typeof(MarkerGenRuleNodeAddMarker),
                typeof(MarkerGenRuleNodeRemoveMarker),
                typeof(MarkerGenRuleNodeMarkerExists),
                typeof(MarkerGenRuleNodeConditionScript),
            };
        }
        
        public static void Dispatch(SerializedObject sobject, Object target)
        {
            if (sobject == null || target == null) return;
            var modified = sobject.ApplyModifiedProperties();
            if (modified)
            {
                if (target is FlowExecTask)
                {
                    if (FlowTaskPropertyChanged != null)
                    {
                        FlowTaskPropertyChanged.Invoke((FlowExecTask)target);
                    }
                }
                else if (SnapEditorTypes.Contains(target.GetType()))
                {
                    if (SnapPropertyChanged != null)
                    {
                        SnapPropertyChanged.Invoke(target);
                    }
                }
                else if (MarkerGenEditorTypes.Contains(target.GetType()))
                {
                    if (MarkerGenPropertyChanged != null)
                    {
                        MarkerGenPropertyChanged.Invoke(target);
                    }
                }
            }
        }
    }
}