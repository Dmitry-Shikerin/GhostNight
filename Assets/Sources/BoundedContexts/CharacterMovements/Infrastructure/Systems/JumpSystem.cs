using Leopotam.EcsLite;
using SevenBoldPencil.EasyEvents;
using Sources.App.Ecs;

namespace Sources.BoundedContexts.CharacterMovements.Infrastructure.Systems
{
    public class JumpSystem : IEcsRunSystem, IEcsInitSystem
    {
        private EventsBus _eventsBus;

        public void Init(IEcsSystems systems) =>
            _eventsBus = systems.GetShared<SharedData>().EventsBus;

        public void Run(IEcsSystems systems)
        {

        }
    }
}