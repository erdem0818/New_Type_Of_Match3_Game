using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;

namespace Mine.Core.Scripts.Framework.UI.Button_Folder
{
    public class CloseButton : DefaultButton
    {
        private BasePanel _panel;

        public override void Awake()
        {
            base.Awake();

            _panel = GetComponentInParent<BasePanel>();
            if (_panel == null)
                _panel = GetComponentInChildren<BasePanel>();
        }

        protected override async UniTask OnClick()
        {
            await base.OnClick();
            if (_panel != null)
                await _panel.HideAsync();
        }
    }
}
