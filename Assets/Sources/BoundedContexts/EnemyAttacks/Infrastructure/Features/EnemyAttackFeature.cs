using Sources.BoundedContexts.EnemyAttacks.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.EnemyAttacks.Infrastructure.Features
{
    public class EnemyAttackFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new EnemyAttackSystem());
            AddSystem(new EnemyBlockAttackSystem());
        }
    }
}