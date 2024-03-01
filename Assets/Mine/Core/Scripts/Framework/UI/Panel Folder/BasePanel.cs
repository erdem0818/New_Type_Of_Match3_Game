using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public abstract class BasePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image backgroundImage;
        
        [Header("Animation Settings")]
        [HorizontalLine(2, EColor.Green)]
        [SerializeField] private float appearDuration;
        [SerializeField] private float disappearDuration;

        [Header("Events")] 
        [HorizontalLine(2f, EColor.Orange)] 
        [SerializeField] private UnityEvent onPreAppearEvent; 
        [SerializeField] private UnityEvent onPostAppearEvent; 
        [SerializeField] private UnityEvent onPreDisappearEvent; 
        [SerializeField] private UnityEvent onPostDisappearEvent; 

        protected virtual void OnPreAppear()
        {
            Debug.Log("Pre Appear".ToBold());
            float target = backgroundImage.color.a;
            backgroundImage.color = backgroundImage.color.SetAlpha(0.0f);
            backgroundImage.DOFade(target, appearDuration);
            
            onPreAppearEvent?.Invoke();
        }

        public void Appear()
        {
            Debug.Log("Appear".ToBold());
        }

        protected virtual void OnPostAppear()
        {
            Debug.Log("Post Appear".ToBold());
            onPostAppearEvent?.Invoke();
        }

        protected virtual void OnPreDisappear()
        {
            onPreDisappearEvent?.Invoke();
        }

        public void Disappear()
        {
            
        }

        protected virtual void OnPostDisappear()
        {
            onPostDisappearEvent?.Invoke();
        }

        public void Hide()
        {
            OnPreDisappear();
            Disappear();
            OnPostDisappear();
        }

        public void Show()
        {
            OnPreAppear();
            Appear();
            OnPostAppear();
        }
    }
}