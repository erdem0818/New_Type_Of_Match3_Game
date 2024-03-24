using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.Game;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using Mine.Core.Scripts.Gameplay;
using NaughtyAttributes;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

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
        [Inject] protected readonly PanelService PanelService;
        [Inject] protected readonly GameHandler GameHandler;

        [SerializeField] private bool selfInit;
        private GameplayState _previousState;

        [SerializeField] [ReadOnly]
        protected ReactiveProperty<VisibleState> state = new (VisibleState.Appearing);

        private CanvasGroup _canvasGroup;
        private CanvasGroup CanvasGroup => _canvasGroup ?
            _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        #region Events
        private readonly Subject<Unit> _preInitializeEvent = new();
        private readonly Subject<Unit> _postInitializeEvent = new();
        private readonly Subject<Unit> _appearEvent = new();
        private readonly Subject<Unit> _appearedEvent = new();
        private readonly Subject<Unit> _disappearEvent = new();
        private readonly Subject<Unit> _disappearedEvent = new();
        
        public IObservable<Unit> OnPreInitialize => _preInitializeEvent.Share();
        public IObservable<Unit> OnPostInitialize => _postInitializeEvent.Share();
        public IObservable<Unit> OnAppear => _appearEvent.Share();
        public IObservable<Unit> OnAppearing => OnChangingVisibleState(OnAppear, OnAppeared);
        public IObservable<Unit> OnAppeared => _appearedEvent.Share();
        public IObservable<Unit> OnUpdate => OnChangingVisibleState(OnAppeared, OnDisappear);
        public IObservable<Unit> OnDisappear => _disappearEvent.Share();
        public IObservable<Unit> OnDisappearing => OnChangingVisibleState(OnDisappear, OnDisappeared);
        public IObservable<Unit> OnDisappeared => _disappearedEvent.Share();
        #endregion

        private readonly Dictionary<Type, List<KeyValuePair<Component, MethodInfo>>> _attributedMethods = new();

        protected virtual void Awake()
        {
            FindCallbackAttributes();
            state.Subscribe((vs) =>
            {
                Debug.Log(vs.ToString().ToColor(Defines.Lemon));
            });

            if (!selfInit) return;
            
            PanelService.AddPanelToViews(this);
            ShowAsync().Forget();
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
        public async UniTask ShowAsync(float delay = 0f, CancellationToken token = default)
        {
            //_previousState = GameHandler.GameplayState.Value;
            GameHandler.GameplayState = GameplayState.InUI;
            
            gameObject.SetActive(false);
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
            
            var rect = (RectTransform) transform;
            await InitializeRectTransform(rect);
            CanvasGroup.alpha = 1;
            
            _preInitializeEvent.OnNext(Unit.Default);

            InvokeAttMethods(typeof(PreAppearAttribute));
            await WhenPreAppearAsync();
            
            gameObject.SetActive(true);
            _postInitializeEvent.OnNext(Unit.Default);

            state.Value = VisibleState.Appearing;
            _appearEvent.OnNext(Unit.Default);
            
            InvokeAttMethods(typeof(PostAppearAttribute));
            await WhenPostAppearAsync();
            
            state.Value = VisibleState.Appeared;
            _appearedEvent.OnNext(Unit.Default); 
        }
        
        [Button]
        public async UniTask HideAsync()
        {
            state.Value = VisibleState.Disappearing;
            _disappearEvent.OnNext(Unit.Default);

            await UniTask.Yield(destroyCancellationToken);
            
            InvokeAttMethods(typeof(PreDisappearAttribute));
            await WhenPreDisappearAsync();
            
            gameObject.SetActive(false);
            
            InvokeAttMethods(typeof(PostDisappearAttribute));
            await WhenPostDisappearAsync();

            state.Value = VisibleState.Disappeared;
            _disappearedEvent.OnNext(Unit.Default);

            await RequestDestroy();
            //GameHandler.GameplayState.Value = _previousState;
        }
        
        private async UniTask RequestDestroy()
        {
            await UniTask.WaitUntil(() => state.Value == VisibleState.Disappeared);
            Destroy(gameObject);
        }
        
        #endregion

        private static async UniTask InitializeRectTransform(RectTransform rect)
        {
            //INFO:: for some reason zero does not expand to all screen.
            rect.sizeDelta = new Vector2(5f, 5f); //Vector2.zero;
            await UniTask.Yield();
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
        
        private IObservable<Unit> OnChangingVisibleState(IObservable<Unit> begin, IObservable<Unit> end)
        {
            return this.UpdateAsObservable()
                .SkipUntil(begin)
                .TakeUntil(end)
                .RepeatUntilDestroy(gameObject)
                .Share();
        }

        protected void OnDestroy()
        {
            _preInitializeEvent.Dispose();
            _postInitializeEvent.Dispose();
            _appearedEvent.Dispose();
            _appearedEvent.Dispose();
            _disappearEvent.Dispose();
            _disappearedEvent.Dispose();
        }
    }
}