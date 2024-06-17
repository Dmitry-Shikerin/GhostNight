using System;
using DungeonArchitect.MarkerGenerator.VM;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Rule
{
    public class MarkerGenRule : ScriptableObject
    {
        [HideInInspector]
        public MarkerGenRuleGraph ruleGraph;
        
        /// <summary>
        /// The condition graph compiled down to bytecode. This will run in a virtual machine
        /// </summary>
        [HideInInspector]
        public MarkerGenRuleProgram program;

        /// <summary>
        /// The actions to execute if the condition program passes
        /// </summary>
        [HideInInspector]
        public MarkerGenRuleActionList actions;

#if	UNITY_EDITOR
        /// <summary>
        /// Textual representation of the rule's condition graph, for preview purpose in the editor
        /// </summary>
        public string previewTextCondition = "";
        
        /// <summary>
        /// Textual representation of the rule's action graph, for preview purpose in the editor
        /// </summary>
        public string[] previewTextActions = Array.Empty<string>();

        /// <summary>
        /// The color of the rule object.  This is for visuals only
        /// </summary>
        public Color color = Color.white;


        [Tooltip(@"Make the rule block pop out in the scene.   This is purely visual, disable it for blocks whose visuals get in the way and don't really contribute to the pattern you're looking for")]
        public bool visuallyDominant = true;
#endif // UNITY_EDITOR

        public virtual bool IsAssetInsertedHere()
        {
            return (actions != null && actions.hints != null) ? actions.hints.emitsMarker : false;
        }
        
    }
}