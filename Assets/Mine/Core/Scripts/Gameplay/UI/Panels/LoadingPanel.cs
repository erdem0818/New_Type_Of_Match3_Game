using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Gameplay.Databases;
using UniRx;
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
        private CancellationTokenSource _cts;

        protected override void Awake()
        {
            base.Awake();
            //Info Logs only once
            // OnUpdate.FirstOrDefault().Subscribe(_ =>
            // {
            //     Debug.Log("Panel On Update");
            // }).AddTo(gameObject);

            //INFO:: logs every update
            // OnUpdate.Subscribe(_ =>
            // {
            //                     
            // }).AddTo(gameObject);

            _cts = new CancellationTokenSource();
            OnDisappeared.Subscribe(_ =>
            {
                _cts.Cancel();
                _cts.Dispose();
                _tween?.Kill();
            });
        }

        protected override UniTask WhenPostAppearAsync()
        {
            PlayLoadingAnimation(_cts.Token).Forget();
            return base.WhenPostAppearAsync();
        }

        private  async UniTask PlayLoadingAnimation(CancellationToken token)
        {
            //state.Value is VisibleState.Appearing or VisibleState.Appeared
            while (token.IsCancellationRequested == false) 
            {
                foreach (var sprite in _fruitSpriteDatabase)
                {
                    token.ThrowIfCancellationRequested();
                    
                    loadingImage.sprite = sprite;
                    loadingImage.transform.localScale = Vector3.one * 0.75f;
                    _tween = loadingImage.transform.DOScale(Vector3.one, 0.5f);
                    await _tween.AsyncWaitForCompletion();
                }
            }
        }
    }
}