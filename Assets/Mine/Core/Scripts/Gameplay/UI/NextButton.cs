using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Button_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.UI
{
    public class NextButton : DefaultButton
    {
        protected override async UniTask OnClick()
        {
            await base.OnClick();

            Debug.Log("Next Button On Click");
        }
        
        [PreAppear, PreDisappear]
        public void OnPreAppear()
        {
            Button.interactable = false;
        }
        
        [PostAppear]
        public void OnPostAppear()
        {
            Button.interactable = true;
        }
    }
}
