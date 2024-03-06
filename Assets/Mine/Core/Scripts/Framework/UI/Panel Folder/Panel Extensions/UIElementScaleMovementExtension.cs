using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder.Panel_Extensions
{
    public class UIElementScaleMovementExtension : PanelExtension
    {
        [Header("Movement Settings")]
        [SerializeField] private float duration = 0.75f;
        [SerializeField] private float percent = -20;
        [SerializeField] private Ease ease = Ease.InOutBack;
        
        [PreAppear]
        public override void DoExtension()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            Vector3 target = rectTransform.localScale;
            Vector3 start = target.MultiplyByPercent(percent);
            rectTransform.localScale = start;
            rectTransform.DOScale(target, duration).SetEase(ease).SetAutoKill(true);
        }
    }
}
