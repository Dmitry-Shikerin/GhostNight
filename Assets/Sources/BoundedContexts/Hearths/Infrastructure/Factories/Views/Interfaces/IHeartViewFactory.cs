using Sources.BoundedContexts.Hearths.Presentation.Views.Implementation;
using Sources.BoundedContexts.Hearths.Presentation.Views.Interfaces;

namespace Sources.BoundedContexts.Hearths.Infrastructure.Factories.Views.Interfaces
{
    public interface IHeartViewFactory
    {
        IHearthView Create(HearthView view);
        IHearthView Create();
    }
}