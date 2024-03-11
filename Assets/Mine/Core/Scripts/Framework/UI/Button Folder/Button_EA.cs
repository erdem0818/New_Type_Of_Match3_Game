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
            _tween = transform.DOPunchScale(
                    new Vector3(0.15f, 0.15f, 1),
                    0.075f,
                    1,
                    0.075f)
                .SetAutoKill(true)
                .OnComplete(() => transform.localScale = Vector3.one);
        }

        private void DoPressMovement()
        {
            _tween?.Kill();
            _tween = transform.DOScale(Vector3.one * 0.9f, 0.075f)
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

