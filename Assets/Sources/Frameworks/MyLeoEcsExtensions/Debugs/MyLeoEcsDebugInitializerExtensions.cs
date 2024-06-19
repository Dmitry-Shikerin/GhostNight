using Leopotam.EcsLite;

namespace Sources.Frameworks.MyLeoEcsExtensions.Debugs
{
    public static class MyLeoEcsDebugInitializerExtensions
    {
        public static void AddEditorSystems(this IEcsSystems editorSystems, IEcsSystems mainSystems, EcsWorld world)
        {
#if UNITY_EDITOR
            editorSystems
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Add(new Mitfart.LeoECSLite.UnityIntegration.EcsWorldDebugSystem())
                .Init();
#endif

#if UNITY_EDITOR
            mainSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem());
#endif
        }
    }
}