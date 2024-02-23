using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Mine.Core.Scripts.Gameplay.Database
{
    public abstract class Database<T> : ScriptableObject, IEnumerable<T>
    {
        [Header("DB Items")]
        [HorizontalLine(1, EColor.Red)]
        [SerializeField] protected List<T> items;
    
        public T GetDBItem(int id)
        {
            if(id < 0) return items[0];
            else if(id >= items.Count) return items[items.Count-1];
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