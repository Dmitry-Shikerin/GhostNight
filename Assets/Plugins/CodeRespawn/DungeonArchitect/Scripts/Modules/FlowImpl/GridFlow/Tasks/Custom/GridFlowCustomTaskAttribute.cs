using DungeonArchitect.Flow.Exec;

namespace DungeonArchitect.Flow.Impl.GridFlow.Tasks
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GridFlowCustomTaskAttribute : FlowExecNodeInfoAttribute
    {
        public GridFlowCustomTaskAttribute(string title) : base(title)
        {
        }

        public GridFlowCustomTaskAttribute(string title, string menuPrefix) : base(title, menuPrefix)
        {
        }

        public GridFlowCustomTaskAttribute(string title, string menuPrefix, float weight) : base(title, menuPrefix, weight)
        {
        }
    }
}