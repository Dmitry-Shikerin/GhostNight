using System.Collections.Generic;
using DungeonArchitect.MarkerGenerator.Nodes.Actions.Info;
using DungeonArchitect.MarkerGenerator.Rule;
using DungeonArchitect.MarkerGenerator.VM;
using DungeonArchitect.Utils;
using UnityEngine;

namespace DungeonArchitect.Editors.MarkerGenerator.TextGen
{
    struct TextExecStackInfo
    {
        public string Text;
        public bool Parenthesis;

        public TextExecStackInfo(string text, bool parenthesis)
        {
            this.Text = text;
            this.Parenthesis = parenthesis;
        }
    }

    public static class MarkerGenRuleTextGenerator
    {
        public static void GenerateRulePreviewText(MarkerGenRule rule)
        {
            if (!rule.program.compiled)
            {
                return;
            }
            
            // Generate the condition graph
            {
                var scriptTypes = new Dictionary<string, System.Type>();
                var execStack = new Stack<int>();
                var textStack = new Stack<TextExecStackInfo>();
                foreach (var instruction in rule.program.instructions)
                {
                    if (instruction.opcode == OpCodes.MarkerExists)
                    {
                        var stringIdx = execStack.Pop();
                        textStack.Pop();    // One of the string table true/false key
                        var markerName = rule.program.stringTable[stringIdx];
                        markerName = SanitizeMarkerName(markerName);
                        textStack.Push(new TextExecStackInfo() { Text = markerName, Parenthesis = false });
                    }
                    if (instruction.opcode == OpCodes.ConditionScript)
                    {
                        var stringIdx = execStack.Pop();
                        textStack.Pop();    // One of the string table true/false key
                        var scriptName = rule.program.stringTable[stringIdx];
                        System.Type scriptType = System.Type.GetType(scriptName);
                        var text = scriptType == null ? "<NONE>" : scriptType.Name;
                        textStack.Push(new TextExecStackInfo() { Text = text, Parenthesis = false });
                    }
                    else if (instruction.opcode == OpCodes.Not)
                    {
                        var text = "NOT " + PopTopText(textStack);
                        textStack.Push(new TextExecStackInfo() { Text = text, Parenthesis = false });
                    }
                    else if (instruction.opcode == OpCodes.And)
                    {
                        var b = PopTopText(textStack);
                        var a = PopTopText(textStack);
                        var text = a + " AND " + b;
                        textStack.Push(new TextExecStackInfo() { Text = text, Parenthesis = true });
                    }
                    else if (instruction.opcode == OpCodes.Or)
                    {
                        var b = PopTopText(textStack);
                        var a = PopTopText(textStack);
                        var text = a + " OR " + b;
                        textStack.Push(new TextExecStackInfo() { Text = text, Parenthesis = true });
                    }
                    else if (instruction.opcode == OpCodes.Push)
                    {
                        execStack.Push(instruction.arg0);

                        var text = instruction.arg0 == 0 ? "False" : "True";
                        textStack.Push(new TextExecStackInfo() { Text = text, Parenthesis = false });
                    }
                    else if (instruction.opcode == OpCodes.Halt)
                    {
                        break;
                    }
                }

                if (textStack.Count == 1)
                {
                    var result = textStack.Pop();
                    rule.previewTextCondition = result.Text;
                }
            }

            
            // Generate the action list text
            {
                var result = new List<string>();
                foreach (var action in rule.actions.actionList)
                {
                    if (action is MarkerGenRuleActionInfoAddMarker addAction)
                    {
                        result.Add("ADD " + SanitizeMarkerName(addAction.markerName));
                    }
                    else if (action is MarkerGenRuleActionInfoRemoveMarker removeAction)
                    {
                        result.Add("DEL " + SanitizeMarkerName(removeAction.markerName));
                    } 
                }

                rule.previewTextActions = result.ToArray();
            }
        }

        private static string PopTopText(Stack<TextExecStackInfo> stack)
        {
            if (stack.Count > 0)
            {
                var top = stack.Pop();
                return top.Parenthesis ? ApplyParenthesis(top.Text) : top.Text;
            }
            else
            {
                return "";
            }
        }
        
        private static string ApplyParenthesis(string text)
        {
            return "(" + text + ")";
        }

        private static string SanitizeMarkerName(string name)
        {
            return name.Trim().Length == 0 ? "<NONE>" : name;
        }

    }
}