using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Mine.Core.Scripts.Framework.Utility
{
    public interface ISceneHandler
    {
        public UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode);
        public UniTask UnLoadSceneAsync(string sceneName, UnloadSceneOptions options);
    }
    
    public class SceneHandler : ISceneHandler
    {
        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName, mode);
            await operation;
            if (operation.isDone)
            {
                Scene loadedScene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(loadedScene);
            }
        }

        public async UniTask UnLoadSceneAsync(string sceneName, UnloadSceneOptions options = UnloadSceneOptions.None)
        {
            await SceneManager.UnloadSceneAsync(sceneName, options);
        }
    }
}
