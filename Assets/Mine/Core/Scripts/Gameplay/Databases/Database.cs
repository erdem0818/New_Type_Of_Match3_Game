using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Databases
{
    public abstract class Database<T> : ScriptableObject, IEnumerable<T> where T : class
    {
        [Header("DB Items")]
        [HorizontalLine(1, EColor.Red)]
        [SerializeField] protected List<T> items;
    
        public T GetDBItem(int id)
        {
            if(id < 0) return items[0];
            if(id >= items.Count) return items[^1];
            return items[id];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }
    }
}