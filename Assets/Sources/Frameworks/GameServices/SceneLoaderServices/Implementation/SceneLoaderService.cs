using Cysharp.Threading.Tasks;
using Sources.Frameworks.GameServices.SceneLoaderServices.Interfaces;
using UnityEngine.SceneManagement;

namespace Sources.Frameworks.GameServices.SceneLoaderServices.Implementation
{
    public class SceneLoaderService : ISceneLoaderService
    {
        public async UniTask LoadSceneAsync(string sceneName) =>
            await SceneManager.LoadSceneAsync(sceneName);
    }
}