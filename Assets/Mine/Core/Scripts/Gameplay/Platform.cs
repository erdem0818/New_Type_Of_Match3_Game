using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
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

    public class Platform : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Transform _transform;
        private readonly FoodPlacer _foodPlacer;

        public Platform(SignalBus signalBus, Transform transform, FoodPlacer foodPlacer)
        {
            _signalBus = signalBus;
            _transform = transform;
            _foodPlacer = foodPlacer;
        }

        private readonly List<PlatformPart> _parts = new();
        public FoodView[] Foods { get; private set; }

        public int PlatformLength => _parts.Count;

        public void Initialize() 
        {
            InitArrays();
            _signalBus.Subscribe<FoodClickedSignal>(OnFoodClicked);
            _signalBus.Subscribe<MatchHappenedSignal>(OnMatchHappened);
            _signalBus.Subscribe<MatchAnimationFinishedSignal>(OnMatchAnimationFinished);
        }

        private void InitArrays()
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

            Foods = new FoodView[PlatformLength];
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodClickedSignal>(OnFoodClicked);
            _signalBus.TryUnsubscribe<MatchHappenedSignal>(OnMatchHappened);
            _signalBus.TryUnsubscribe<MatchAnimationFinishedSignal>(OnMatchAnimationFinished);
        }

        private void OnFoodClicked(FoodClickedSignal signal)
        {
            if (Full()) return;
            Place(signal);
        }

        private void Place(FoodClickedSignal signal)
        {
            int placeIndex = GetPlaceIndex(signal.Food.Data.foodID);
            Debug.Log($"PlaceIndex: {placeIndex}".ToBold());

            signal.Food.IsSelected = true;
            signal.Food.SetPhysics(false);

            SetOccupationStatus(placeIndex, signal.Food, true);

            _signalBus.TryFire(new FoodPlacedSignal()
            {
                Food = signal.Food,
                PlacedIndex = placeIndex,
                PlacePosition = GetPartPosition(placeIndex)
            });
        }

        private void OnMatchHappened(MatchHappenedSignal signal)
        {
            signal.IndexFoodTuples.ForEach(tuple =>
            {
                SetOccupationStatus(tuple.index, null, false);
            });
        }

        private void OnMatchAnimationFinished(MatchAnimationFinishedSignal signal)
        {
            MoveAllLeftSide();
        }

        public void SetOccupationStatus(int indx, FoodView food, bool occupied)
        {
            if (occupied && _parts[indx].IsOccupied)
            {
                MoveAllRightSide(indx);
                Debug.Log("Already occupied"
                    .ToBold()
                    .ToColor(new Color(0.75f, 0.25f, 0.45f, 1.0f)));
            }

            _parts[indx].IsOccupied = occupied;
            Foods[indx] = food;
        }

        public void MoveAllRightSide(int startIndex)
        {
            int lastFullIndex = GetLastFullIndex();
            Debug.Log($"Last Occupied Index: {lastFullIndex}".ToBold());

            for (int i = lastFullIndex; i >= startIndex; i--)
            {
                if (Foods[i] == null)
                    continue;

                FoodView food = Foods[i];
                Foods[i + 1] = food;
                _parts[i + 1].IsOccupied = true;

                //todo refactor not here
                _foodPlacer.SlideTheFood(food, GetPartPosition(i + 1)).Forget();
            }
        }

        public void MoveAllLeftSide()
        {
            for (int i = 0; i < Foods.Length; i++)
            {
                if (Foods[i] == null)
                    continue;

                FoodView food = Foods[i];

                //todo first make empty
                _parts[i].IsOccupied = false;
                Foods[i] = null;
                
                //then find the empty
                int empty = GetFirstEmptyIndex();
                _parts[empty].IsOccupied = true;
                Foods[empty] = food;

                //todo refactor not here
                _foodPlacer.SlideTheFood(food, GetPartPosition(empty)).Forget();
            }
        }

        public int GetPlaceIndex(int foodId)
        {
            if (AllEmpty()) return 0;

            if (IsThereAnySameFood(foodId))
                return GetFirstNeighborIndex(foodId);
            else
                return GetFirstEmptyIndex();
        }

        public int GetFirstNeighborIndex(int foodId)
        {
            return FindLastFoodIndex(foodId) + 1;
        }

        public int FindLastFoodIndex(int foodId)
        {
            int index = 0;
            for (int i = 0; i < Foods.Length; i++)
            {
                FoodView temp = Foods[i];
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

        public int GetLastFullIndex()
        {
            int lastFullIndex = 0;
            for (int i = 0; i < Foods.Length; i++)
            {
                if (Foods[i] != null) lastFullIndex = i;
            }
            return lastFullIndex;
        }

        public bool IsThereAnySameFood(int id)
        {
            foreach (var item in Foods)
            {
                if (item == null)
                    continue;

                if (item.Data.foodID == id) return true;
            }
            return false;
        }

        public bool Full() => _parts.All(p => p.IsOccupied);
        public bool AllEmpty() => _parts.All(p => p.IsOccupied == false);
        public bool AnyEmpty() => _parts.Any(p => p.IsOccupied == false);
        public Vector3 GetPartPosition(int index) => _parts[index].GetPlacePosition();
    }
}
