using Sources.BoundedContexts.EnemyHealths.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.EnemyHealths.Infrastructure.Features
{
    public class EnemyHealthFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new EnemyHealthUiSystem());
        }
    }
}