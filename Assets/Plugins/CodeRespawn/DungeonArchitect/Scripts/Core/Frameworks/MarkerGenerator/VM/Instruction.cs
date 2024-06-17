namespace DungeonArchitect.MarkerGenerator.VM
{
    [System.Serializable]
    public struct Instruction
    {
        public int opcode;
        public int arg0;
        public int arg1;

        public Instruction(int opcode, int arg0 = 0, int arg1 = 0)
        {
            this.opcode = opcode;
            this.arg0 = arg0;
            this.arg1 = arg1;
        }

        public static readonly Instruction NoOp = new Instruction(OpCodes.NoOp);
        public static readonly Instruction Halt = new Instruction(OpCodes.Halt);
    }
}