using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mine.Core.Scripts.Framework.UI.Button_Folder
{
    [RequireComponent(typeof(Button))]
    public abstract class Button_EA : MonoBehaviour
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
                    new Vector3(0.25f, 0.25f, 1),
                    0.1f,
                    2,
                    0.1f)
                .SetAutoKill(true);
        }

        protected virtual void OnClick()
        {
            onClick?.Invoke();
        }

        private void OnDestroy()
        {
            _tween?.Kill(false);
        }
    }
}

