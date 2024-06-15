using Sources.Frameworks.GameServices.UpdateServices.Interfaces.Methods;
using Sources.Frameworks.MVPPassiveView.Controllers.Interfaces.ControllerLifetimes;

namespace Sources.App.Ecs.Controllers.Interfaces
{
    public interface IEcsStartUp : IInitialize, IUpdatable, IDestroy
    {
    }
}