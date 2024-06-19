using System;
using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Controllers.Interfaces;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.Cameras.Infrastructure.Systems;
using Sources.BoundedContexts.CharacterMovements.Domain.Events;
using Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems;
using Sources.BoundedContexts.CharacterSounds.Infrastructure.Systems;
using Sources.BoundedContexts.EnemyMovements.Infrastructure.Systems;
using Sources.BoundedContexts.FootstepParticles.Infrastructure.Systems;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.Frameworks.MyLeoEcsExtensions.OneFrames.Extensions;
using Zenject;

namespace Sources.App.Ecs.Controllers.Implementation
{
    public class EcsStartUp : IEcsStartUp
    {
        private readonly DiContainer _container;

        private IEcsSystems _systems;
        private IEcsSystems _editorSystems;
        private EcsWorld _world;
        private SharedData _sharedData;
        private EventsBus _eventsBus;

        private bool _isInitialize;

        public EcsStartUp(DiContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void Initialize()
        {
            _world = new EcsWorld();
            _eventsBus = new EventsBus();
            _sharedData = new SharedData
            {
                EventsBus = _eventsBus,
                DiContainer = _container
            };
            _systems = new EcsSystems(_world, _sharedData);
            AddEditorSystems();
            AddRunSystems();
            AddOneFrame();
            AddEvents();
            AddComponentsConverterWorld();
            // AddUnityIntegrationSystem();
            Inject();
            _systems.Init();
            _isInitialize = true;
        }

        public void Update(float deltaTime)
        {
            if (_isInitialize == false)
                return;
            
            _systems?.Run();
            RunEditorSystems();
        }

        public void Destroy()
        {
            DestroyEditorSystems();
            
            if (_systems != null)
            {
                _systems.Destroy();
                _systems = null;
                _world.Destroy();
                _world = null;
            }
        }

        private void AddRunSystems()
        {
            _systems
                .Add(new JumpSystem())
                .Add(new MovementSystem())
                .Add(new PlayerInputSystem())
                .Add(new RotateSystem())
                .Add(new GroundCheckSystem())
                .Add(new JumpGravitySystem())
                .Add(new BlockJumpRemoveSystem())
                .Add(new CharacterAnimationSystem())
                .Add(new FootstepParticleSystem())
                .Add(new CharacterSoundSystem())
                .Add(new EnemyMovementSystem())
                ;
        }

        private void AddOneFrame()
        {
            _systems
                .AddOneFrame<FootstepEvent>()
                ;
        }

        private void AddEvents()
        {
            _systems.Add(
                _sharedData.EventsBus
                    .GetDestroyEventsSystem()
                    .IncSingleton<JumpEvent>()
            );
        }

        private void Inject()
        {
            _systems
                .Inject();
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
            _editorSystems = new EcsSystems(_world);
            _editorSystems
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                .Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem())
                .Init();
#endif
        }

        private void AddUnityIntegrationSystem()
        {
#if UNITY_EDITOR
            _systems
                .Add(new Mitfart.LeoECSLite.UnityIntegration.EcsWorldDebugSystem());
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