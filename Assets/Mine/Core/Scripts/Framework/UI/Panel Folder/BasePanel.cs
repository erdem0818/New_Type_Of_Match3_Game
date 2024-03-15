using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using Mine.Core.Scripts.Gameplay;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public abstract class PanelExtension : MonoBehaviour
    {
        public abstract void DoExtension();
    }

    public enum VisibleState
    {
        Appearing = 0,
        Appeared = 1,
        Disappearing = 2,
        Disappeared = 3
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour
    {
        [Header("Extensions")]
        [HorizontalLine(2f, EColor.Black)]
        [SerializeField] private List<PanelExtension> extensions;

        [SerializeField] [ReadOnly]
        protected ReactiveProperty<VisibleState> state = new (VisibleState.Appearing);

        private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup ?
            _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>(); 
        
        //INFO:: Subject<Unit> appear - disappear events ?
        //INFO:: public IObservable<Unit> OnPreInitialize => preInitializeEvent.Share();

        private readonly Dictionary<Type, List<KeyValuePair<Component, MethodInfo>>> _attributedMethods = new();

        protected virtual void Awake()
        {
            FindCallbackAttributes();
            state.Subscribe((vs) =>
            {
                Debug.Log(vs.ToString().ToColor(Defines.Lemon));
            });
        }
        
        #region Reflection
        //todo make this utility
        private void FindCallbackAttributes()
        {
            Component[] allComponents = GetComponentsInChildren<Component>();
            Type[] lookFor = {typeof(PreAppearAttribute), typeof(PostAppearAttribute), typeof(PreDisappearAttribute), typeof(PostDisappearAttribute)};
            foreach (var component in allComponents)
            {
                if(component == null || component == this) continue;

                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                MethodInfo[] methods = component.GetType().GetMethods(flags);

                foreach (MethodInfo methodInfo in methods)
                {
                    foreach (Type attType in lookFor)
                    {
                        if (!Attribute.IsDefined(methodInfo, attType)) continue;
                        
                        //Info adding new dictionary key pointer to list for an attribute type
                        if (!_attributedMethods.ContainsKey(attType))
                        {
                            _attributedMethods.Add(attType, new List<KeyValuePair<Component, MethodInfo>>());
                        }

                        _attributedMethods[attType].Add(new KeyValuePair<Component, MethodInfo>(component, methodInfo));
                    }
                }
            }
        }

        private void InvokeAttMethods(Type attributeType)
        {
            //Info still out?
            if (!_attributedMethods.TryGetValue(attributeType, out List<KeyValuePair<Component, MethodInfo>> methods)) return;
            foreach (var (comp, method) in methods)
            {
                method.Invoke(comp, null);
            }
        }
        #endregion
        
        //TODO vContainer - MVP - uniRx example look.
        #region Appear Methods
        
        protected virtual UniTask WhenPreAppearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostAppearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPreDisappearAsync() => UniTask.CompletedTask;
        protected virtual UniTask WhenPostDisappearAsync() => UniTask.CompletedTask;

        [Button]
        public async UniTask ShowAsync()
        {
            gameObject.SetActive(false);
            
            var rect = (RectTransform) transform;
            await InitializeRectTransform(rect);
            CanvasGroup.alpha = 1;
            
            InvokeAttMethods(typeof(PreAppearAttribute));
            await WhenPreAppearAsync();
            
            //INFO:: preInitializeEvent.OnNext(Unit.Default);
            gameObject.SetActive(true);
            //INFO:: postInitializeEvent.OnNext(Unit.Default);

            state.Value = VisibleState.Appearing;

            InvokeAttMethods(typeof(PostAppearAttribute));
            await WhenPostAppearAsync();
            
            state.Value = VisibleState.Appeared;
            
            //INFO:: appearedEvent.OnNext(Unit.Default); 
        }
        
        [Button]
        public async UniTask HideAsync()
        {
            state.Value = VisibleState.Disappearing;

            await UniTask.Yield(destroyCancellationToken);
            
            InvokeAttMethods(typeof(PreDisappearAttribute));
            await WhenPreDisappearAsync();
            
            gameObject.SetActive(false);
            await WhenPostDisappearAsync();

            state.Value = VisibleState.Disappeared;
            
            await RequestDestroy();

        }
        
        private async UniTask RequestDestroy()
        {
            await UniTask.WaitUntil(() => state.Value == VisibleState.Disappeared);
            Destroy(gameObject);
        }
        
        #endregion

        private async UniTask InitializeRectTransform(RectTransform rect)
        {
            rect.sizeDelta = Vector2.zero;
            await UniTask.Yield();
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
        
        //todo try this public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);
        //todo what it is doing
        //todo add IObservable<Unit> props
        private IObservable<Unit> OnChangingVisibleState(IObservable<Unit> begin, IObservable<Unit> end)
        {
            return this.UpdateAsObservable()
                .SkipUntil(begin)
                .TakeUntil(end)
                .RepeatUntilDestroy(gameObject)
                .Share();
        }
    }
}