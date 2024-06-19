using Sources.BoundedContexts.EnemyMovements.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.EnemyMovements.Infrastructure.Features
{
    public sealed class EnemyMovementFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new EnemyMovementSystem());
        }
    }
}