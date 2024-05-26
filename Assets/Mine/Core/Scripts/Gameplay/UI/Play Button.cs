using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Button_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Framework.Utility;
using Mine.Core.Scripts.Gameplay.UI.Panels;
using UnityEngine.SceneManagement;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.UI
{
    public class PlayButton : Button_EA
    {
        [Inject] private ISceneHandler _sceneHandler;
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
            await _sceneHandler.LoadSceneAsync("GameScene", LoadSceneMode.Additive);

            await HideLoadingPanel();
        }

        private async UniTask HideLoadingPanel()
        {
            await UniTask.Delay(3000, DelayType.DeltaTime, PlayerLoopTiming.Update, destroyCancellationToken);
            await _panelService.HidePanel<MainMenuPanel>();
            await _panelService.HidePanel<LoadingPanel>();
        }
    }
}

