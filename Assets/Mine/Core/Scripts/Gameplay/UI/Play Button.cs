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
        
        protected override async UniTask OnClick()
        {
            await base.OnClick();
            await ShowLoadingPanel();
        }
        
        private async UniTask ShowLoadingPanel()
        {
            LoadingPanel loadingPanel = await _panelService.Create<LoadingPanel>();
            await loadingPanel.ShowAsync();
            await UniTask.Delay(100, cancellationToken: destroyCancellationToken);
            await SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive).ToUniTask();
            await _panelService.HidePanel<MainMenuPanel>();
            await HideLoadingPanel();
        }

        private async UniTask HideLoadingPanel()
        {
            await UniTask.Delay(2000);
            await _panelService.HidePanel<LoadingPanel>();
        }
    }
}

