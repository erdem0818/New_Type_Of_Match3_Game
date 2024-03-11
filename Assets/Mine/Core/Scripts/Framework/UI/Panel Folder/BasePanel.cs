using System;
using System.Collections.Generic;
using System.Reflection;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Attribute_Folder;
using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public abstract class PanelExtension : MonoBehaviour
    {
        public abstract void DoExtension();
    }
    
    public abstract class BasePanel : MonoBehaviour
    {
        [Header("Extensions")]
        [HorizontalLine(2f, EColor.Black)]
        [SerializeField] private List<PanelExtension> extensions;

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

        //todo rename, not appear disappear maybe pre show pre hide
        #region Appear Methods
        protected virtual void OnPreAppear()
        {
            //todo keep extension but disabled for now.
            //extensions.ForEach(ex => ex.DoExtension());
            InvokeAttMethods(typeof(PreAppearAttribute));
        }

        public void Appear()
        {
         
        }

        protected virtual void OnPostAppear()
        {
            InvokeAttMethods(typeof(PostAppearAttribute));
        }

        protected virtual void OnPreDisappear()
        {
            InvokeAttMethods(typeof(PreDisappearAttribute));
        }

        public void Disappear()
        {
            
        }

        protected virtual void OnPostDisappear()
        {
            InvokeAttMethods(typeof(PostDisappearAttribute));
            Destroy(gameObject);
        }
        #endregion

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