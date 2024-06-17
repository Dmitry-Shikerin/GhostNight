namespace DungeonArchitect.MarkerGenerator.VM
{
    public struct OpCodes
    {
        public static readonly int NoOp = 0;

        // Stack Ops
        public static readonly int Push = 10;
        
        // Logical Ops
        public static readonly int And = 100;
        public static readonly int Or = 101;
        public static readonly int Not = 102;
        
        // Internal Functions
        public static readonly int MarkerExists = 200;
        public static readonly int ConditionScript = 201;
        
        // System Ops
        public static readonly int Halt = 1000;
    }
}
