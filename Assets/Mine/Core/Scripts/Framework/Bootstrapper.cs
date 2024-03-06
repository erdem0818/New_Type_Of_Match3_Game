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
        //[Scene]
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
                await SceneManager.LoadSceneAsync(currentScenePath, LoadSceneMode.Additive);
            }
        }

        private bool ValidateAllScenes()
        {
            return sceneNames.All(ValidateSceneExistence);
        }

        private bool ValidateSceneExistence(string sceneName)
        {
            return SceneUtility.GetBuildIndexByScenePath(PathPrefix + sceneName) != -1;
        }
    }
}

