using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public abstract class PanelExtension : MonoBehaviour
    {
        public abstract void DoExtension();
    }

    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared
    }
    
    public abstract class BasePanel : MonoBehaviour
    {
        [Header("Extensions")]
        [HorizontalLine(2f, EColor.Black)]
        [SerializeField] private List<PanelExtension> extensions;

        [SerializeField] [ReadOnly]
        protected VisibleState State;
        
        //INFO:: Subject<Unit> appear - disappear events ?
        
        private readonly Dictionary<Type, List<KeyValuePair<Component, MethodInfo>>> _attributedMethods = new();

        protected virtual void Awake()
        {
            FindCallbackAttributes();
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
        
        //TODO vcontaier-mv-unirx example look.
        #region Appear Methods
        protected virtual async UniTask OnPreAppear()
        {
            //extensions.ForEach(ex => ex.DoExtension());
            State = VisibleState.Appearing;
            InvokeAttMethods(typeof(PreAppearAttribute));
        }

        public async UniTask Appear()
        {
            
        }

        protected virtual async UniTask OnPostAppear()
        {
            State = VisibleState.Appeared;
            InvokeAttMethods(typeof(PostAppearAttribute));
        }

        protected virtual async UniTask OnPreDisappear()
        {
            State = VisibleState.Disappearing;
            InvokeAttMethods(typeof(PreDisappearAttribute));
        }

        public async UniTask Disappear()
        {
            
        }

        protected virtual async UniTask OnPostDisappear()
        {
            State = VisibleState.Disappeared;
            InvokeAttMethods(typeof(PostDisappearAttribute));
            
            //todo UIStates - await until state -> disappeared to destroy
            Destroy(gameObject);
        }
        
        protected virtual UniTask WhenCompletedAsync() => UniTask.CompletedTask;
        
        #endregion

        [Button]
        public async UniTask HideAsync()
        {
            await OnPreDisappear();
            await Disappear();
            await OnPostDisappear();
        }

        [Button]
        public async UniTask ShowAsync()
        {
            await OnPreAppear();
            await Appear();
            await OnPostAppear();
        }
    }
}