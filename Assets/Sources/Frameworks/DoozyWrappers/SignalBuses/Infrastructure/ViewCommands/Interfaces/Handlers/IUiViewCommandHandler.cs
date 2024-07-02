using Sources.Frameworks.UiFramework.Views.Domain;

namespace Sources.Frameworks.DoozyWrappers.SignalBuses.Infrastructure.ViewCommands.Interfaces.Handlers
{
    public interface IUiViewCommandHandler
    {
        void Handle(FormCommandId formCommandId);
    }
}