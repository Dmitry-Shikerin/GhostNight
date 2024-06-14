using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using Sources.BoundedContexts.CharacterMovements.Events;
using Sources.BoundedContexts.CharacterMovements.Systems;
using UnityEngine;

namespace Sources.App.Ecs
{
    public class EcsStartUp : MonoBehaviour
    {
        private IEcsSystems _systems;
        private IEcsSystems _editorSystems;
        private EcsWorld _world;
        private SharedData _sharedData;

        private void Awake()
        {
            _world = new EcsWorld();
            _sharedData = new SharedData
            {
                EventsBus = new EventsBus(),
            };
            _systems = new EcsSystems(_world, _sharedData);
            AddEditorSystems();
            AddRunSystems();
            AddOneFrame();
            Inject();
            AddComponentsConverterWorld();
            _systems.Init();
        }

        private void Update()
        {
            _systems?.Run();
            UpdateEditorSystems();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems.GetWorld().Destroy();
                _systems = null;
            }

            DestroyEditorSystems();
        }

        private void AddRunSystems()
        {
            _systems
                .Add(new PlayerJumpSystem())
                ;
        }

        private void AddOneFrame()
        {
            _systems.Add(
                _sharedData.EventsBus
                    .GetDestroyEventsSystem()
                    // .IncSingleton<CharacterJumpEvent>()
                );
        }

        private void Inject()
        {
        }

        private void AddComponentsConverterWorld()
        {
            _systems
                .AddWorld(new EcsWorld(), "events")
                .ConvertScene();
        }

        private void AddEditorSystems()
        {
#if UNITY_EDITOR
            // Создаем отдельную группу для отладочных систем.
            _editorSystems = new EcsSystems(_systems.GetWorld());
            _editorSystems
                .Add(new Mitfart.LeoECSLite.UnityIntegration.EcsWorldDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Init();
#endif
            //_systems.Add ()
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem());
#endif
        }

        private void UpdateEditorSystems()
        {
#if UNITY_EDITOR
            _editorSystems?.Run();
#endif
        }

        private void DestroyEditorSystems()
        {
#if UNITY_EDITOR
            if (_editorSystems != null)
            {
                _editorSystems.Destroy();
                _editorSystems = null;
            }
#endif
        }
    }
}