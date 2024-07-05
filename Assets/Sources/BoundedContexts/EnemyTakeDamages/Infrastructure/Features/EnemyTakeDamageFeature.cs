using Sources.BoundedContexts.EnemyTakeDamages.Infrastructure.Systems;
using Sources.Frameworks.MyLeoEcsExtensions.Features.Infrastructure.Systems.Implementation;

namespace Sources.BoundedContexts.EnemyTakeDamages.Infrastructure.Features
{
    public class EnemyTakeDamageFeature : EcsFeature
    {
        protected override void Register()
        {
            AddSystem(new EnemyTakeDamageSystem());
            AddSystem(new EnemyTakeDamageParticleSystem());
        }
    }
}