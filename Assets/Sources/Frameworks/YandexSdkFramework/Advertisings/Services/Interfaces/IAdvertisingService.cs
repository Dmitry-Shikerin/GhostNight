using Sources.BoundedContexts.PlayerWallets.Domain.Models;
using Sources.Frameworks.MVPPassiveView.Controllers.Interfaces.ControllerLifetimes;
using Sources.Frameworks.MVPPassiveView.Presentations.Interfaces.PresentationsInterfaces.Views.Constructors;

namespace Sources.Frameworks.YandexSdkFramework.Advertisings.Services.Interfaces
{
    public interface IAdvertisingService : IConstruct<PlayerWallet>, IInitialize, IDestroy
    {
    }
}