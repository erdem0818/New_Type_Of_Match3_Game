using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

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

    public class Platform : IInitializable
    {
        private Transform _transform;

        public Platform(Transform transform)
        {
            _transform = transform;
        }

        private readonly List<PlatformPart> _parts = new();
        private FoodView[] _foodViews;
        public FoodView[] Foods => _foodViews;

        public int PlatformLength => _parts.Count;

        public void Initialize() 
        {
            int childCount = _transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                _parts.Add(new PlatformPart()
                {
                    Transform = _transform.GetChild(i),
                    IsOccupied = false
                });
            }

            _foodViews = new FoodView[PlatformLength];
        }

        public void SetOccupationStatus(int indx, FoodView food, bool occupied)
        {
            if (occupied && _parts[indx].IsOccupied)
            {
                MoveAllRightSide(indx);
                Debug.Log("Already occupied".ToBold().ToColor(new Color(0.75f, 0.25f, 0.45f, 1.0f)));
            }

            _parts[indx].IsOccupied = occupied;
            _foodViews[indx] = food;
        }

        public void MoveAllRightSide(int startIndex)
        {
            int lastFullIndex = GetLastFullIndex();
            Debug.Log($"Last Occupied Index: {lastFullIndex}".ToBold());

            for (int i = lastFullIndex; i >= startIndex; i--)
            {
                if (_foodViews[i] == null)
                    continue;

                FoodView food = _foodViews[i];
                _foodViews[i + 1] = food;
                _parts[i + 1].IsOccupied = true;

                //todo refactor not here
                food.transform.DOMove(GetPartPosition(i + 1), 0.1f)
                    .SetAutoKill(true);
            }
        }

        public void MoveAllLeftSide(int startIndex)
        {
            // -1, -1, -1, 2, 2, 5, 5, 6
            for (int i = 0; i < _foodViews.Length; i++)
            {
                if (_foodViews[i] == null)
                    continue;

                FoodView food = _foodViews[i];

                //todo first make empty
                _parts[i].IsOccupied = false;
                _foodViews[i] = null;
                
                //then find the empty
                int empty = GetFirstEmptyIndex();
                _parts[empty].IsOccupied = true;
                _foodViews[empty] = food;

                //todo refactor not here
                food.transform.DOMove(GetPartPosition(empty), 0.1f)
                    .SetAutoKill(true);
            }
        }

        public int GetPlaceIndex(int foodId)
        {
            if (AllEmpty()) return 0;

            if (IsThereAnySameFood(foodId))
            {
                return GetFirstNeighborIndex(foodId);
            }
            else
            {
                return GetFirstEmptyIndex();
            }
        }

        public int GetFirstNeighborIndex(int foodId)
        {
            return FindLastFoodIndex(foodId) + 1;
        }

        public int FindLastFoodIndex(int foodId)
        {
            int index = 0;
            for (int i = 0; i < _foodViews.Length; i++)
            {
                FoodView temp = _foodViews[i];
                if (temp == null || temp.Data.foodID != foodId)
                    continue;

                index = i;
            }

            return index;
        }

        public int GetFirstEmptyIndex()
        {
            if (AnyEmpty() == false)
                return Defines.AllFull;

            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].IsOccupied == false)
                    return i;
            }

            return Defines.AllFull;
        }

        public bool IsThereAnySameFood(int id)
        {
            foreach (var item in _foodViews)
            {
                if (item == null)
                    continue;

                if(item.Data.foodID == id) return true;
            }
            return false;
        }

        public int GetLastFullIndex()
        {
            int lastFullIndex = 0;
            for (int i = 0; i < _foodViews.Length; i++)
            {
                if (_foodViews[i] != null) lastFullIndex = i;
            }
            return lastFullIndex;
        }

        public bool AllEmpty() => _parts.All(p => p.IsOccupied == false);
        public bool AnyEmpty() => _parts.Any(p => p.IsOccupied == false);
        public Vector3 GetPartPosition(int index) => _parts[index].GetPlacePosition();
    }
}
