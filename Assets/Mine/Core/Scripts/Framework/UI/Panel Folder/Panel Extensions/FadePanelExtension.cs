using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using UnityEngine;
using UnityEngine.UI;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder.Panel_Extensions
{
    public class FadePanelExtension : PanelExtension
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private float fadeDuration;

        [PreAppear]
        public override void DoExtension()
        {
            float target = backgroundImage.color.a;
            backgroundImage.color = backgroundImage.color.SetAlpha(0.0f);
            backgroundImage.DOFade(target, fadeDuration);
        }
    }
}
