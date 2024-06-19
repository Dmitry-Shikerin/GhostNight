using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Interfaces;

namespace Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation
{
    public abstract class EcsFeature : IEcsFeature
    {
        private readonly List<IEcsSystem> _systems = new List<IEcsSystem>();
        private readonly List<IEcsInitSystem> _initSystems = new List<IEcsInitSystem>();
        private readonly List<IEcsRunSystem> _runSystems = new List<IEcsRunSystem>();
        private readonly List<IEcsDestroySystem> _destroySystems = new List<IEcsDestroySystem>();
        
        private bool _enabled = true;

        public void Enable() =>
            _enabled = true;

        public void Disable() =>
            _enabled = false;

        public void Init(IEcsSystems systems)
        {
            Register();
            Inject(systems);
            
            foreach (IEcsInitSystem system in _initSystems)
                system.Init(systems);
        }

        public void Run(IEcsSystems systems)
        {
            if (_enabled == false)
                return;

            foreach (IEcsRunSystem system in _runSystems)
                system.Run(systems);
        }

        public void Destroy(IEcsSystems systems)
        {
            foreach (IEcsDestroySystem system in _destroySystems)
                system.Destroy(systems);
        }

        protected abstract void Register();
        
        protected EcsFeature AddSystem(IEcsSystem system)
        {
            if (system is IEcsInitSystem initSystem)
                _initSystems.Add(initSystem);

            if (system is IEcsRunSystem runSystem)
                _runSystems.Add(runSystem);

            if (system is IEcsDestroySystem destroySystem)
                _destroySystems.Add(destroySystem);
            
            _systems.Add(system);

            return this;
        }

        private void Inject(IEcsSystems systems)
        {
            foreach (IEcsSystem system in _systems)
                Extensions.InjectToSystem(system, systems, null);
        }
    }
}