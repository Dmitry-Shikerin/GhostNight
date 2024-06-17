using System.Collections.Generic;
using DungeonArchitect.Graphs;
using DungeonArchitect.MarkerGenerator.Nodes.Condition;
using DungeonArchitect.MarkerGenerator.Pins;
using DungeonArchitect.MarkerGenerator.Rule;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.VM
{
    public class ConditionGraphCompiler
    {
        public static void Compile(MarkerGenRuleGraph graph, MarkerGenRuleProgram program)
        {
            new ConditionGraphCompiler().CompileImpl(graph, program);
        }
        
        private readonly List<Instruction> instructions = new List<Instruction>();
        private readonly List<string> stringTable = new List<string>();
        private Dictionary<GraphPin, GraphPin> incomingPinMap;
        private readonly HashSet<MarkerGenRuleGraphNode> visited = new HashSet<MarkerGenRuleGraphNode>();
        private void CompileImpl(MarkerGenRuleGraph graph, MarkerGenRuleProgram program)
        {   
            instructions.Clear();
            stringTable.Clear();
            visited.Clear();
            
            if (graph == null || graph.resultNode == null)
            {
                // invalid graph state. emit a program that returns false
                CreateDefaultFalseProgram();
                AssembleProgram(program);
                program.compiled = false;
                return;
            }
            
            incomingPinMap = BuildPinLookup(graph);
            bool success = GenerateNodeInstructions(graph.resultNode);
            if (!success)
            {
                program.compiled = false;
                return;
            }

            AssembleProgram(program);
            program.compiled = true;
        }

        private bool GenerateNodeInstructions(MarkerGenRuleGraphNode node)
        {
            if (visited.Contains(node))
            {
                return false;
            }
            visited.Add(node);
            
            // Process all the input nodes first
            foreach (var inputPin in node.InputPins)
            {
                var linkedPin = incomingPinMap.ContainsKey(inputPin) ? incomingPinMap[inputPin] : null;
                if (linkedPin != null)
                {
                    // Traverse the incoming node
                    if (linkedPin.Node != null && linkedPin.Node is MarkerGenRuleGraphNode incomingNode)
                    {
                        var success = GenerateNodeInstructions(incomingNode);
                        if (!success)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    // Push the default value of the pin (the checkbox on an unconnected pin widget in the graph editor)
                    var defaultValue = false;
                    if (inputPin is MarkerGenRuleGraphPinBool boolPin)
                    {
                        defaultValue = boolPin.defaultValue;
                    }
                    
                    AddInstruction(OpCodes.Push, VmUtils.ToInt(defaultValue));
                }
            }
            
            // Emit out the node opcode (the required function params should be available in the stack)
            if (node is MarkerGenRuleNodeMarkerExists markerExistNode)
            {
                int stringIdx = RegisterString(markerExistNode.markerName);
                AddInstruction(OpCodes.Push, stringIdx);
                AddInstruction(OpCodes.MarkerExists);
            }
            else if (node is MarkerGenRuleNodeConditionScript scriptNode)
            {
                int stringIdx = RegisterString(scriptNode.scriptClassName);
                AddInstruction(OpCodes.Push, stringIdx);
                AddInstruction(OpCodes.ConditionScript);
            }
            else if (node is MarkerGenRuleNodeAnd)
            {
                AddInstruction(OpCodes.And);
            }
            else if (node is MarkerGenRuleNodeOr)
            {
                AddInstruction(OpCodes.Or);
            }
            else if (node is MarkerGenRuleNodeNot)
            {
                AddInstruction(OpCodes.Not);
            }
            else if (node is MarkerGenRuleNodeResult)
            {
                AddInstruction(OpCodes.Halt);
            }
            else
            {
                Debug.Log("Unsupported code gen node: " + node.GetType());
                AddInstruction(OpCodes.NoOp);
            }
            
            visited.Remove(node);
            return true;
        }

        void AssembleProgram(MarkerGenRuleProgram program)
        {
            program.instructions = instructions.ToArray();
            program.stringTable = stringTable.ToArray();
        }
        
        void CreateDefaultFalseProgram()
        {
            // Push false and halt
            instructions.Clear();
            AddInstruction(OpCodes.Push, VmUtils.ToInt(false));
            AddInstruction(OpCodes.Halt);
        }
        

        void AddInstruction(int opcode, int arg0 = 0, int arg1 = 0)
        {
            instructions.Add(new Instruction(opcode, arg0, arg1));
        }
        
        int RegisterString(string value)
        {
            stringTable.Add(value);
            return stringTable.Count - 1;
        }
        
        Dictionary<GraphPin, GraphPin> BuildPinLookup(Graph graph)
        {
            var linkedTo = new Dictionary<GraphPin, GraphPin>();
            foreach (var link in graph.Links)
            {
                var pinInput = link.Input as MarkerGenRuleGraphPin;
                var pinOutput = link.Output as MarkerGenRuleGraphPin;
                if (pinInput == null || pinOutput == null) continue;
                
                linkedTo[pinInput] = pinOutput;
            }

            return linkedTo;
        }
    }
}