using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Mine.Core.Scripts.Framework
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

            for(int i = 0; i < sceneNames.Count; i++)
            {
                string currentScenePath = PathPrefix + sceneNames[i];
                await SceneManager.LoadSceneAsync(currentScenePath, LoadSceneMode.Additive);
            }
        }

        private bool ValidateAllScenes()
        {
            return sceneNames.All(sm => ValidateSceneExistence(sm));
        }

        private bool ValidateSceneExistence(string sceneName)
        {
            return SceneUtility.GetBuildIndexByScenePath(PathPrefix + sceneName) == -1 ? false : true;
        }
    }
}

