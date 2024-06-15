using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems;
using UnityEngine;

namespace Sources.App.Ecs
{
    public class EcsStartUp : MonoBehaviour
    {
        private IEcsSystems _systems;
        private IEcsSystems _editorSystems;
        private EcsWorld _world;
        private SharedData _sharedData;

        private void Start()
        {
            _world = new EcsWorld();
            _sharedData = new SharedData
            {
                EventsBus = new EventsBus(),
            };
            _systems = new EcsSystems(_world, _sharedData);
            AddEditorSystems();
            AddRunSystems();
            AddEvents();
            AddComponentsConverterWorld();
            Inject();
            _systems.Init();
        }

        private void Update()
        {
            _systems?.Run();
            RunEditorSystems();
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
                _world.Destroy();
                _world = null;
            }

            DestroyEditorSystems();
        }

        private void AddRunSystems()
        {
            _systems
                .Add(new JumpSystem())
                .Add(new MovementSystem())
                .Add(new PlayerInputSystem())
                .Add(new RotateSystem())
                .Add(new CharacterAnimationSystem())
                ;
        }

        private void AddEvents()
        {
            _systems.Add(
                _sharedData.EventsBus
                    .GetDestroyEventsSystem()
                );
        }

        private void Inject()
        {
            _systems.Inject();
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
            _editorSystems = new EcsSystems(_systems.GetWorld());
            _editorSystems
                .Add(new Mitfart.LeoECSLite.UnityIntegration.EcsWorldDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Init();
#endif
            
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem());
#endif
        }

        private void RunEditorSystems()
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