using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        protected override async UniTask OnPostAppear()
        {
            await base.OnPostAppear();
            
            StartCoroutine(PlayLoadingAnimation());
        }

        private IEnumerator PlayLoadingAnimation()
        {
            while (State is VisibleState.Appearing or VisibleState.Appeared)
            {
                if (State is VisibleState.Disappearing or VisibleState.Disappeared)
                {
                    _tween?.Kill(true);
                    break;
                }
                
                foreach (var sprite in _fruitSpriteDatabase)
                {
                    loadingImage.sprite = sprite;
                    loadingImage.transform.localScale = Vector3.one * 0.75f;
                    _tween = loadingImage.transform.DOScale(Vector3.one, 0.5f);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
