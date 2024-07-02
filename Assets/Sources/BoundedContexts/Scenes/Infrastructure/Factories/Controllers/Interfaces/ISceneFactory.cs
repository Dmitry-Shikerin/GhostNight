using Cysharp.Threading.Tasks;
using Sources.Frameworks.MVPPassiveView.Controllers.Interfaces.Scenes;

namespace Sources.BoundedContexts.Scenes.Infrastructure.Factories.Controllers.Interfaces
{
    public interface ISceneFactory
    {
        UniTask<IScene> Create(object payload);
    }
}