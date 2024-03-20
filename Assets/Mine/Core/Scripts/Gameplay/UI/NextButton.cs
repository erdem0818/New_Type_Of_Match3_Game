using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Button_Folder;
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
        
        // [PreAppear]
        // public void DoMovement()
        // {
        //     RectTransform rectTransform = GetComponent<RectTransform>();
        //     Vector2 currentPos = rectTransform.anchoredPosition;
        //
        //     rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(0.0f);
        //     rectTransform.DOAnchorPos(currentPos, 0.75f)
        //         .SetEase(Ease.InOutBack);
        // }
    }
}
