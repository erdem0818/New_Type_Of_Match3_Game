using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Button_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.UI
{
    public class NextButton : DefaultButton
    {
        protected override void OnClick()
        {
            base.OnClick();

            Debug.Log("Next Button On Click");
        }
        
        [PreAppear]
        public void DoMovement()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector2 currentPos = rectTransform.anchoredPosition;

            rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(0.0f);
            rectTransform.DOAnchorPos(currentPos, 0.75f)
                .SetEase(Ease.InOutBack);
        }

        /*[PostAppear]
        public void PostAppearTest()
        {
            Debug.Log("Post Appear");
        }
        
        [PreDisappear]
        public void PreDisappearTest()
        {
            Debug.Log("Pre Disappear");
        }
        
        [PostDisappear]
        public void PostDisappearTest()
        {
            Debug.Log("Post Disappear");
        }
        
        [PreAppear]
        [PostDisappear]
        public void MultipleTest()
        {
            Debug.Log("Multiple Test".ToBold());
        }*/
    }
}
