using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Mine.Core.Scripts.Framework.UI.ButtonEA
{
    [RequireComponent(typeof(Button))]
    public abstract class Button_EA : MonoBehaviour
    {
        private Button _button;
        private List<UnityAction> _callbacks = new();
        private Tween _tween;

        private void Awake() 
        {
            _button = GetComponent<Button>();    
            _button.onClick.AddListener(OnClickCallback);
        }

        private void OnClickCallback()
        {
            DoMovement();
            OnClick();
            _callbacks.ForEach(ac => ac?.Invoke());
        }

        public void AddOnClickCallback(UnityAction action)
        {
            if(_callbacks.Contains(action) == false)
            {
                _callbacks.Add(action);
            }
        }

        private void DoMovement()
        {
            _tween?.Kill(true);
            _tween = transform.DOPunchScale(new Vector3(0.35f, 0.35f, 1), 0.2f, 3, 0.2f).SetAutoKill(true);
        }

        public virtual void OnClick(){}
    }
}

