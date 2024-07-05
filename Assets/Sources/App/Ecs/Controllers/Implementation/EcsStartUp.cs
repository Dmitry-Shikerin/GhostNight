using System;
using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs.Controllers.Interfaces;
using Sources.App.Ecs.Domain;
using Sources.BoundedContexts.CharacterMovements.Infrastructure.Features;
using Sources.BoundedContexts.CharacterStuns.Infrastructure.Features;
using Sources.BoundedContexts.CharacterTakeDamages.Infrastructure.Features;
using Sources.BoundedContexts.DealDamages.Domain.Events;
using Sources.BoundedContexts.EnemyAttacks.Infrastructure.Features;
using Sources.BoundedContexts.EnemyMovements.Infrastructure.Features;
using Sources.BoundedContexts.EnemyStuns.Infrastructure.Features;
using Sources.BoundedContexts.EnemyTakeDamages.Infrastructure.Features;
using Sources.BoundedContexts.EntityReferences.Infrastructure.Systems;
using Sources.BoundedContexts.Footsteps.Domain.Events;
using Sources.BoundedContexts.Hammers.Domain.Events;
using Sources.BoundedContexts.Hammers.Infrastructure.Features;
using Sources.BoundedContexts.Hearths.Domain.Events;
using Sources.BoundedContexts.Hearths.Infrastructure.Features;
using Sources.BoundedContexts.Stuns.Domain.Events;
using Sources.BoundedContexts.Traps.Infrastructure.Features;
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
            // AddEditorSystems();
            AddSystems();
            AddOneFrame();
            AddEvents();
            AddComponentsConverterWorld();
            AddUnityIntegrationSystem();
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

        private void AddSystems()
        {
            _systems
                .Add(new EntityReferenceInitializeSystem())
                .Add(new CharacterMovementFeature())
                .Add(new CharacterStunFeature())
                .Add(new HearthFeature())
                .Add(new HammerFeature())
                .Add(new EnemyMovementFeature())
                .Add(new EnemyAttackFeature())
                .Add(new CharacterTakeDamageFeature())
                .Add(new EnemyStunFeature())
                .Add(new EnemyTakeDamageFeature())
                .Add(new TrapFeature())
                ;
        }

        private void AddOneFrame()
        {
            _systems
                .AddOneFrame<FootstepEvent>()
                .AddOneFrame<PickUpHearthEvent>()
                .AddOneFrame<TakeDamageEvent>()
                .AddOneFrame<StartHummerAttackEvent>()
                .AddOneFrame<EndHammerAttackEvent>()
                .AddOneFrame<HammerAttackHitEvent>()
                .AddOneFrame<StartStunEvent>()
                .AddOneFrame<EndStunEvent>()
                ;
        }

        private void AddEvents()
        {
            _systems.Add(
                _sharedData.EventsBus
                    .GetDestroyEventsSystem()
                    // .IncSingleton<JumpEvent>()
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