using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mine.Core.Scripts.Framework.UI.Button_Folder
{
    [RequireComponent(typeof(Button))]
    public abstract class Button_EA : MonoBehaviour, IPointerDownHandler
    {
        private Button _button;
        private Tween _tween;

        [SerializeField] private UnityEvent onClick;
        
        public virtual void Awake() 
        {
            _button = GetComponent<Button>();    
            _button.onClick.AddListener(OnClickCallback);
        }

        private void OnClickCallback()
        {
            DoMovement();
            OnClick();
        }

        private void DoMovement()
        {
            _tween?.Kill(true);
            _tween = transform.DOScale(Vector3.one, 0.075f)
                .SetEase(Ease.OutBack)
                .SetAutoKill(true);
        }

        private void DoPressMovement()
        {
            _tween?.Kill();
            _tween = transform.DOScale(Vector3.one * 0.85f, 0.075f)
                .SetEase(Ease.InBack)
                .SetAutoKill(true);
        }

        protected virtual void OnClick()
        {
            onClick?.Invoke();
        }

        protected virtual void OnPressed()
        {
            DoPressMovement();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnPressed();
        }
        
        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}

