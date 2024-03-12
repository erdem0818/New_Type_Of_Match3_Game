using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public interface IPanelService
    {
        public UniTask<T> Create<T>() where T : BasePanel;
    }
    
    public class PanelService : IPanelService
    {
        private readonly Settings _settings;
        private readonly DiContainer _diContainer;
        private readonly Transform _parent;
        private readonly Dictionary<Type, GameObject> _panelDictionary;
        
        public PanelService(Settings settings, DiContainer diContainer, Transform parent)
        {
            _diContainer = diContainer;
            _panelDictionary = settings.GetPanelDictionary();
            _settings = settings;
            _parent = parent;
        }

        public async UniTask<T> Create<T>() where T : BasePanel
        {
            T panel = _diContainer.InstantiatePrefab(_panelDictionary[typeof(T)], _parent).GetComponent<T>();
            panel.gameObject.SetActive(false);
            await UniTask.CompletedTask;
            return panel;
        }
    }
}