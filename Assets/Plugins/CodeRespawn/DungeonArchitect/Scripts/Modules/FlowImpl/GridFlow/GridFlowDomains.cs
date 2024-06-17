//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System;
using System.Linq;
using System.Collections.Generic;
using DungeonArchitect.Flow.Domains;
using DungeonArchitect.Flow.Exec;
using DungeonArchitect.Flow.Impl.GridFlow.Tasks;

namespace DungeonArchitect.Flow.Impl.GridFlow
{
    public class GridFlowTilemapDomain : IFlowDomain
    {
        public Type[] SupportedTasks { get => supportedTypes; }
        public string DisplayName { get => displayName; }
        
        private static readonly string displayName = "Tilemap";
        private static readonly Type[] supportedTypes = new Type[]
        {
            typeof(GridFlowTilemapTaskInitialize),
            typeof(GridFlowTilemapTaskCreateOverlay),
            typeof(GridFlowTilemapTaskCreateElevations),
            typeof(GridFlowTilemapTaskMerge),
            typeof(GridFlowTilemapTaskOptimize),
            typeof(GridFlowTilemapTaskFinalize)
        };
    }
    
    public class GridFlowLayoutGraphDomain : IFlowDomain
    {
        public Type[] SupportedTasks { get => supportedTypes; }
        public string DisplayName { get => displayName; }
        
        private static readonly string displayName = "Layout Graph";
        private static readonly Type[] internalTaskTypes = new Type[]
        {
            typeof(GridFlowLayoutTaskCreateGrid),
            typeof(GridFlowLayoutTaskCreateMainPath),
            typeof(GridFlowLayoutTaskCreatePath),
            typeof(GridFlowLayoutTaskSpawnItems),
            typeof(GridFlowLayoutTaskCreateKeyLock),
            typeof(GridFlowLayoutTaskMirrorGraph),
            typeof(GridFlowLayoutTaskFinalizeGraph)
        };

        private static readonly Type[] supportedTypes;
        static GridFlowLayoutGraphDomain()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var handlers = new List<System.Type>();
            handlers.AddRange(internalTaskTypes);
            foreach (var assembly in assemblies)
            {
                var asmHandlers = (from t in assembly.GetTypes()
                    where t.IsClass && t.IsSubclassOf(typeof(FlowExecTask)) && Attribute.GetCustomAttribute(t, typeof(GridFlowCustomTaskAttribute)) != null
                    select t).ToArray();

                if (asmHandlers.Length > 0)
                {
                    handlers.AddRange(asmHandlers.ToArray());
                }
            }

            supportedTypes = handlers.ToArray();
        }
    }
}