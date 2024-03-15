using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Button_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Gameplay.UI.Panels;
using UnityEngine.SceneManagement;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.UI
{
    public class PlayButton : Button_EA
    {
        [Inject] private IPanelService _panelService;
        
        protected override void OnClick()
        {
            base.OnClick();
            
            ShowLoadingPanel().Forget();
        }

        private async UniTask ShowLoadingPanel()
        {
            LoadingPanel loadingPanel = await _panelService.Create<LoadingPanel>();
            //move this in to base panel
            loadingPanel.gameObject.SetActive(true);
            await loadingPanel.ShowAsync();
            await SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive).ToUniTask();
            await HideLoadingPanel();
            //todo hide main panel
        }

        private async UniTask HideLoadingPanel()
        {
            await UniTask.Delay(2000);
            await _panelService.HidePanel<LoadingPanel>();
        }
    }
}

