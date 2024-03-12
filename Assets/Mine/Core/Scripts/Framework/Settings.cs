using System;
using System.Collections.Generic;
using System.Linq;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Framework
{
    [CreateAssetMenu(menuName = "Settings", fileName = "Settings")]
    public class Settings : ScriptableObject
    {
        [Header("Panels")] 
        [SerializeField] private List<BasePanel> panels;

        public Dictionary<Type, GameObject> GetPanelDictionary()
        {
            Dictionary<Type, GameObject> result = new();

            foreach (var panel in panels.Where(panel => result.ContainsKey(panel.GetType()) == false))
            {
                result.Add(panel.GetType(), panel.gameObject);
            }

            return result;
        }
    }
}
