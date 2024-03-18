using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public interface IPanelService
    {
        public UniTask<T> Create<T>() where T : BasePanel;
        public UniTask HidePanel<T>() where T : BasePanel;
    }
    
    public class PanelService : IPanelService, IInitializable
    {
        private readonly DiContainer _diContainer;
        private readonly Transform _parent;
        private readonly Dictionary<Type, GameObject> _panelDictionary;

        private readonly List<BasePanel> _activeViews = new();

        public PanelService(Settings settings, DiContainer diContainer, Transform parent)
        {
            _diContainer = diContainer;
            _panelDictionary = settings.GetPanelDictionary();
            _parent = parent;
        }
        
        public void Initialize()
        {
            BasePanel[] panels = _parent.GetComponentsInChildren<BasePanel>();
            if (panels == null) return;
            foreach (BasePanel panel in panels)
                _activeViews.Add(panel);
        }

        public async UniTask<T> Create<T>() where T : BasePanel
        {
            try
            {
                //await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: token);
                T panel = _diContainer.InstantiatePrefab(_panelDictionary[typeof(T)], _parent).GetComponent<T>();
                await UniTask.CompletedTask;
                _activeViews.Add(panel);
                return panel;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message.ToBold().ToColor(new Color(0.9f, 0.25f, 0.2f, 1.0f)));
                return null;
            }
        }

        public async UniTask HidePanel<T>() where T : BasePanel
        {
            try
            {
                var panel = _activeViews.First(p => typeof(T) == p.GetType());
                await panel.HideAsync();
                _activeViews.Remove(panel);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message.ToBold().ToColor(new Color(0.9f, 0.25f, 0.2f, 1.0f)));
            }
        }

        public void AddPanelToViews(BasePanel panel)
        {
            _activeViews.Add(panel);
        }
    }
}