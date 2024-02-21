using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public class PlatformPart
    {
        public Transform Transform { get; set; }
        public bool IsOccupied { get; set; }

        public Vector3 GetPlacePosition()
        {
            return Transform.position + Vector3.up * 0.1f;
        }
    }

    public class Platform : MonoBehaviour
    {
        private readonly List<PlatformPart> _parts = new();

        public int PlatformLength => _parts.Count;

        private void Awake() 
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                _parts.Add(new PlatformPart()
                {
                    Transform = transform.GetChild(i),
                    IsOccupied = false
                });
            }
        }

        public void SetOccupationStatus(int i, bool occupied)
        {
            _parts[i].IsOccupied = occupied;
        }

        public int GetFirstEmptyIndex()
        {
            if(AnyEmpty() == false)
                return Defines.AllFull;

            for (int i = 0; i < _parts.Count; i++)
            {
                if(_parts[i].IsOccupied == false)
                    return i;
            }

            return Defines.AllFull;
        }

        public bool AnyEmpty()
        {
            return _parts.Any(p => p.IsOccupied == false);
        }

        public Vector3 GetPartPosition(int index)
        {
            return _parts[index].GetPlacePosition();
        }
    }
}
