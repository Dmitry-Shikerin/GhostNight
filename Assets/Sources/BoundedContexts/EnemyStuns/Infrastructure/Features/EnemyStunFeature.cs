using Sources.BoundedContexts.EnemyStuns.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.EnemyStuns.Infrastructure.Features
{
    public class EnemyStunFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new EnemyStunSystem());
            AddSystem(new EnemyStunParticleSystem());
        }
    }
}