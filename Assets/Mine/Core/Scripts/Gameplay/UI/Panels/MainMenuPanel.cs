using Mine.Core.Scripts.Framework.Game;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using UniRx;

namespace Mine.Core.Scripts.Gameplay.UI.Panels
{
    public class MainMenuPanel : DefaultPanel
    {
        protected override void Awake()
        {
            base.Awake();

            OnPreInitialize.Subscribe(_ =>
            {
                GameHandler.AppState = AppState.MainMenu;
            });
        }
    }
}
