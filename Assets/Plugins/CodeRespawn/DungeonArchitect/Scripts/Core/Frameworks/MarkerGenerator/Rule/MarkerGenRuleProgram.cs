using System;
using DungeonArchitect.MarkerGenerator.VM;
using UnityEngine;

namespace DungeonArchitect.MarkerGenerator.Rule
{
    public class MarkerGenRuleProgram : ScriptableObject
    {
        public Instruction[] instructions = Array.Empty<Instruction>();
        public string[] stringTable = Array.Empty<string>();
        public bool compiled = false;
    }
}
