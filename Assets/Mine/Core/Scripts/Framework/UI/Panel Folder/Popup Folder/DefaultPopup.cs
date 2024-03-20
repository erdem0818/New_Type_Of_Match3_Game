using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder.Popup_Folder
{
    public class DefaultPopup : BasePanel
    {
        //todo enable if attributes or custom editor - tabs
        [SerializeField] private Transform popupRect;
        [SerializeField] private bool showAnimation;
        [SerializeField] private bool hideAnimation;

        protected override async UniTask WhenPostAppearAsync()
        {
            await base.WhenPostAppearAsync();
            if (showAnimation)
            {
                Vector3 scale = popupRect.localScale;
                popupRect.localScale = scale.MultiplyByPercent(-20f);
                await popupRect.DOScale(scale, 0.35f)
                    .SetEase(Ease.InOutBack)
                    .AsyncWaitForCompletion();
            }
        }
        
        protected override async UniTask WhenPreDisappearAsync()
        {
            await base.WhenPostDisappearAsync();

            if (hideAnimation)
            {
                Vector3 scale = popupRect.localScale;
                await popupRect.DOScale(scale.MultiplyByPercent(-20), 0.2f)
                    .SetEase(Ease.InOutBack)
                    .AsyncWaitForCompletion();
            }
        }
    }
}
