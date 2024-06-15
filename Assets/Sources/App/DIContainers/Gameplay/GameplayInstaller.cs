using Sources.App.Ecs;
using Zenject;

namespace Sources.App.DIContainers.Gameplay
{
    public class GameplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EcsStartUp>().AsSingle();
        }
    }
}