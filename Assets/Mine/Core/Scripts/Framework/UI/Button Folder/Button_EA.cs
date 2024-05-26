using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Mine.Core.Scripts.Framework.UI.Button_Folder
{
    [RequireComponent(typeof(Button))]
    public abstract class Button_EA : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Inject] protected GameHandler GameHandler;
        
        protected Button Button;
        private Tween _tween;

        private Vector3 _initialScale;
        
        [SerializeField] private UnityEvent onClick;
        
        public virtual void Awake() 
        {
            Button = GetComponent<Button>();
            _initialScale = Button.transform.localScale;
            Button.onClick.AddListener(OnClickCallback);
        }
        
        private void OnClickCallback()
        {
            OnClick().Forget();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressed();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            DoPressBackMovement().Forget();
        }
        
        protected virtual async UniTask OnClick()
        {
            await DoPressBackMovement();
            onClick?.Invoke();
        }

        private async UniTask DoPressBackMovement()
        {
            _tween = transform.DOScale(_initialScale, 0.075f)
                .SetEase(Ease.OutBack);
            await _tween.AsyncWaitForCompletion();
        }

        private async UniTask DoPressMovement()
        {
            _tween = transform.DOScale(_initialScale * 0.9f, 0.075f)
                .SetEase(Ease.InBack);
            await _tween.AsyncWaitForCompletion();
        }
        
        protected virtual async void OnPressed()
        {
            await DoPressMovement();
        }
        
        private void OnDestroy()
        {
            _tween?.Kill(true);
        }
    }
}

