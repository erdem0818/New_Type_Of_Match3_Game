using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Gameplay.Databases;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.UI.Panels
{
    public class LoadingPanel : DefaultPanel
    {
        [Inject] private FruitSpriteDatabase _fruitSpriteDatabase;
        [SerializeField] private Image loadingImage;
        
        private Tween _tween;
        
        protected override UniTask WhenPostAppearAsync()
        {
            StartCoroutine(PlayLoadingAnimation());
            
            return base.WhenPostAppearAsync();
        }

        private IEnumerator PlayLoadingAnimation()
        {
            bool play = true;
            while (play) //state.Value is VisibleState.Appearing or VisibleState.Appeared
            {
                foreach (var sprite in _fruitSpriteDatabase)
                {
                    loadingImage.sprite = sprite;
                    loadingImage.transform.localScale = Vector3.one * 0.75f;
                    //bug throws exception
                    _tween = loadingImage.transform.DOScale(Vector3.one, 0.5f)
                        .OnUpdate(() =>
                        {
                            if (state.Value is not (VisibleState.Disappearing or VisibleState.Disappeared))
                                return;
                            
                            play = false;
                            _tween?.Kill();
                        });

                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
