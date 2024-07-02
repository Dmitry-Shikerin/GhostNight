using Sources.Frameworks.MVPPassiveView.Controllers.Interfaces.ControllerLifetimes;

namespace Sources.Frameworks.UiFramework.Core.Services.Common
{
    public interface IViewService : IAwake, IEnable, IDisable, IDestroy
    {
    }
}