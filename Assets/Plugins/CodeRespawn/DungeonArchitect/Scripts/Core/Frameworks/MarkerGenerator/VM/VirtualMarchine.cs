using System.Collections;
using System.Collections.Generic;
using DungeonArchitect.Utils;
using DungeonArchitect.MarkerGenerator.Rule;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.VM
{
    public interface IMarkerGenVmAPI
    {
        bool MarkerExists(string markerName);
        bool ConditionScript(string scriptPath);
    }

    public class MarkerGenVM
    {
        private IMarkerGenVmAPI api;
        
        private MarkerGenRuleProgram program;
        private int instructionIndex = 0;
        private Stack<int> stack = new Stack<int>();
        private bool running = false;
        
        public string ErrorMessage { get; private set; } = "";

        public MarkerGenVM(IMarkerGenVmAPI api)
        {
            this.api = api;
        }
        
        /// <summary>
        /// Executes the bytecode program.  the result of the program is saved in the out varaible
        /// </summary>
        /// <param name="programToLoad"></param>
        /// <param name="result"></param>
        /// <returns>True if the program ran successfully</returns>
        public bool Run(MarkerGenRuleProgram programToLoad, out bool result)
        {
            
            this.program = programToLoad;
            stack = new Stack<int>();
            instructionIndex = 0;
            ErrorMessage = "";
            
            if (program != null && program.instructions != null && program.instructions.Length > 0)
            {
                running = true;
            }
            else
            {
                this.program = null;
                running = false;
            }

            
            while (running && GetInstruction(out var instruction))
            {
                if (instruction.opcode == OpCodes.Push)
                {
                    PushStack(instruction.arg0);
                }
                else if (instruction.opcode == OpCodes.MarkerExists)
                {
                    ExecMarkerExists();
                }
                else if (instruction.opcode == OpCodes.ConditionScript)
                {
                    ExecConditionScript();
                }
                else if (instruction.opcode == OpCodes.And)
                {
                    ExecLogicalAnd();
                }
                else if (instruction.opcode == OpCodes.Or)
                {
                    ExecLogicalOr();
                }
                else if (instruction.opcode == OpCodes.Not)
                {
                    ExecLogicalNot();
                }

                // Move to the next instruction
                Step();
            }

            // The result of the execution should be in the stack
            bool success = PopStack(out var resultValue); 
            result = success && VmUtils.ToBool(resultValue);
            return success;
        }


        private void ExecConditionScript()
        {
            if (!PopStack(out var stringIdx))
            {
                running = false;
                return;
            }
            
            if (!GetString(stringIdx, out var scriptFullPath))
            {
                running = false;
                return;
            }

            var result = api.ConditionScript(scriptFullPath);
            PushStack(result);
        }

        private void ExecMarkerExists()
        {
            if (!PopStack(out var stringIdx))
            {
                running = false;
                return;
            }

            if (!GetString(stringIdx, out var markerName))
            {
                running = false;
                return;
            }

            var exists = api.MarkerExists(markerName);
            PushStack(exists);
        }

        private void ExecLogicalAnd()
        {
            int lhs = 0;
            int rhs = 0;
            if (!PopStack(out lhs) || !PopStack(out rhs))
            {
                running = false;
                return;
            }

            // Logical AND operation
            var result = VmUtils.ToBool(lhs) && VmUtils.ToBool(rhs);
            PushStack(result);
        }

        private void ExecLogicalOr()
        {
            int lhs = 0;
            int rhs = 0;
            if (!PopStack(out lhs) || !PopStack(out rhs))
            {
                running = false;
                return;
            }

            // Logical OR operation
            var result = VmUtils.ToBool(lhs) || VmUtils.ToBool(rhs);
            PushStack(result);
        }

        private void ExecLogicalNot()
        {
            int input = 0;
            if (!PopStack(out input))
            {
                running = false;
                return;
            }

            // Logical NOT operation
            var result = !VmUtils.ToBool(input);
            PushStack(result);
        }

        private void PushStack(bool value)
        {
            stack.Push(VmUtils.ToInt(value));
        }
        
        private void PushStack(int value)
        {
            stack.Push(value);
        }

        private bool PopStack(out int value)
        {
            if (stack.Count == 0)
            {
                value = 0;
                ErrorMessage = "Invalid Stack State";
                return false;
            }
            
            value = stack.Pop();
            return true;
        }

        private bool GetString(int stringIdx, out string value)
        {
            if (stringIdx >= 0 && stringIdx < program.stringTable.Length)
            {
                value = program.stringTable[stringIdx];
                return true;
            }
            else
            {
                ErrorMessage = "Invalid String Table index";
                value = "";
                return false;
            }

        }
        
        /// <summary>
        /// Gets the current instruction.  Returns true if the program is still running
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        private bool GetInstruction(out Instruction instruction)
        {
            if (!running)
            {
                instruction = Instruction.Halt;
                return false;
            }

            instruction = program.instructions[instructionIndex];

            return instruction.opcode != OpCodes.Halt;
        }
        
        private Instruction Step()
        {
            if (!running)
            {
                return Instruction.Halt;
            }

            instructionIndex++;
            if (!GetInstruction(out var instruction))
            {
                running = false;
            } 

            return instruction;
        }
    }


    public class VmUtils
    {
        public static int ToInt(bool value)
        {
            return value ? 1 : 0;
        }
        
        public static bool ToBool(int value)
        {
            return value != 0;
        }
    }
}