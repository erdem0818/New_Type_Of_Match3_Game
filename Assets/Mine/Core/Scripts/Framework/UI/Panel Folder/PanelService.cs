using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder
{
    public interface IPanelService
    {
        public T Create<T>() where T : BasePanel;
    }
    
    public class PanelService : IPanelService
    {
        //todo 
        private readonly DiContainer _diContainer;
        private readonly Dictionary<Type, GameObject> _panelDictionary = new ();
        
        public PanelService(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public T Create<T>() where T : BasePanel
        {
            return null;
            //return _diContainer.InstantiatePrefab(_panelDictionary[typeof(T)]) as T;
        }
    }
}