using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mine.Core.Scripts.Framework
{
    public class Bootstrapper : MonoBehaviour
    {
        private const string PathPrefix = "Mine/Core/Scenes/";

        [Header("Scenes Path")]
        [HorizontalLine(2, EColor.Blue)]
        [SerializeField] private List<string> sceneNames;

        private async void Awake()
        {
            if(ValidateAllScenes() == false)
            {
                Debug.LogWarning("THERE ARE INCORRECT SCENE NAMES");
                return;
            }
            
            foreach (var currentScenePath in sceneNames.Select(t => PathPrefix + t))
            {
                var op = SceneManager.LoadSceneAsync(currentScenePath, LoadSceneMode.Additive);
                await op;
                if (!op.isDone) continue;
                Scene scene = SceneManager.GetSceneByPath(currentScenePath);
                if (scene.IsValid() && scene.isLoaded)
                    SceneManager.SetActiveScene(scene);
            }
        }
        
        private bool ValidateAllScenes()
        {
            return sceneNames.All(ValidateSceneExistence);
        }

        private static bool ValidateSceneExistence(string sceneName)
        {
            return SceneUtility.GetBuildIndexByScenePath(PathPrefix + sceneName) != -1;
        }
    }
}