using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public class PlatformPart
    {
        public Transform Transform { get; set; }
        public bool IsOccupied { get; set; }
        public FoodView CurrentFood{ get; set; }

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
        
        [Inject] private readonly MatchHandler _matchHandler;

        public Platform(SignalBus signalBus, Transform transform, FoodPlacer foodPlacer)
        {
            _signalBus = signalBus;
            _transform = transform;
            _foodPlacer = foodPlacer;
        }

        private readonly List<PlatformPart> _parts = new();
        public List<PlatformPart> Parts => _parts;
        //public FoodView[] Foods { get; private set; }

        private int PlatformLength => _parts.Count;

        public void Initialize() 
        {
            InitArrays();
            _signalBus.Subscribe<FoodClickedSignal>(OnFoodClicked);
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

            //Foods = new FoodView[PlatformLength];
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodClickedSignal>(OnFoodClicked);
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
            Debug.Log($"PlaceIndex: {placeIndex}".ToBold().ToColor(new Color32(207,186,250,255)));

            signal.Food.IsSelected = true;
            signal.Food.SetPhysics(false);

            SetOccupationStatus(placeIndex, signal.Food, true);
            
            _signalBus.TryFire(new FoodPlacingMovementStartSignal()
            {
                Food = signal.Food,
                PlacedIndex = placeIndex,
                PlacePosition = GetPartPosition(placeIndex)
            });
            
            bool b = _matchHandler.IsThereAnyMatchCheck(out var matches); //_matchHandler.IsThereAnyMatch();
             Debug.Log(b ?
                 "There is Match"
                     .ToBold()
                     .ToColor(new Color(0.9f, 0.1f, 0.9f)) :
                 "No Match".
                     ToBold()
                     .ToColor(new Color(.1f, .1f, .1f)));

             if (!b) return;
             //SetOccupationStatusForMatchElements(matches);
             //MoveAllLeftSide();
             //todo maybe crate another MatchDetected Signal and sub to it
             _matchHandler.RequestMatch(matches);
        }

        private void SetOccupationStatusForMatchElements(IEnumerable<(int index, FoodView food)> pairs)
        {
            foreach ((int index, FoodView food) pair in pairs)
            {
                int correctIndex = Parts.FindIndex(part => ReferenceEquals(part.CurrentFood, pair.food));
                Debug.Log($"Correct Index: {correctIndex}");
                SetOccupationStatus(correctIndex /*pair.index*/, null, false);
            }
        }

        private void OnMatchAnimationFinished(MatchAnimationFinishedSignal signal)
        {
            SetOccupationStatusForMatchElements(signal.IndexFoodTuples);
            MoveAllLeftSide();
            ReorderAllMovement();
        }

        private void SetOccupationStatus(int idx, FoodView food, bool occupied)
        {
            if (occupied && _parts[idx].IsOccupied)
            {
                MoveAllRightSide(idx);
                ReorderAllMovement();
                
                Debug.Log("Already occupied"
                    .ToBold()
                    .ToColor(new Color(0.75f, 0.25f, 0.45f, 1.0f)));
            }

            _parts[idx].IsOccupied = occupied;
            _parts[idx].CurrentFood = food;
            //Foods[idx] = food;

            Debug.Log($"Occupation: {occupied}".ToBold());
        }
        
        public void ReorderAllMovement()
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                FoodView food = _parts[i].CurrentFood;
                if (food == null)
                    continue;

                Vector3 placePosition = GetPartPosition(i);
                _foodPlacer.SlideTheFood(food, placePosition).Forget();
            }
        }

        private void MoveAllRightSide(int startIndex)
        {
            Debug.Log("Move All Right Side".ToBold().ToColor(new Color(.15f, .5f, .8f)));
            int lastFullIndex = GetLastFullIndex();
            Debug.Log($"Last Occupied Index: {lastFullIndex}".ToBold());

            for (int i = lastFullIndex; i >= startIndex; i--)
            {
                if (_parts[i].CurrentFood == null)
                    continue;

                FoodView food = _parts[i].CurrentFood;
                _parts[i + 1].IsOccupied = true;
                _parts[i + 1].CurrentFood = food;
            }
        }

        private void MoveAllLeftSide()
        {
            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].CurrentFood == null)
                    continue;

                FoodView food = _parts[i].CurrentFood;
                
                _parts[i].IsOccupied = false;
                _parts[i].CurrentFood = null;
                
                int empty = GetFirstEmptyIndex();
                _parts[empty].IsOccupied = true;
                _parts[empty].CurrentFood = food;
            }
        }
        
        public void Reset()
        {
            foreach (var part in _parts)
            {
                part.IsOccupied = false;
            }

            for (var i = 0; i < _parts.Count; i++)
            {
                var food = _parts[i].CurrentFood;
                if (food == null)
                    continue;
                food.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2f, ForceMode.Impulse);
                food.SetPhysics(true);
                food.IsSelected = false;
                food.IsPlaced = false;
                food.MarkedForMatch = false;
                food.Sequence.Kill();
                _parts[i].CurrentFood = null;
            }
        }

        private int GetPlaceIndex(int foodId)
        {
            if (AllEmpty()) return 0;

            return IsThereAnySameFood(foodId) ? GetFirstNeighborIndex(foodId) : GetFirstEmptyIndex();
        }

        private int GetFirstNeighborIndex(int foodId)
        {
            return FindLastFoodIndex(foodId) + 1;
        }

        private int FindLastFoodIndex(int foodId)
        {
            int idx = 0;
            for (int i = 0; i < _parts.Count; i++)
            {
                FoodView temp = _parts[i].CurrentFood;
                if (temp == null || temp.Data.foodID != foodId)
                    continue;

                idx = i;
            }

            return idx;
        }

        private int GetFirstEmptyIndex()
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

        private int GetLastFullIndex()
        {
            int lastFullIndex = 0;
            for (int i = 0; i < _parts.Count; i++)
            {
                if (_parts[i].CurrentFood != null) lastFullIndex = i;
            }
            return lastFullIndex;
        }

        private bool IsThereAnySameFood(int id)  => 
            _parts.Where(part => part.CurrentFood != null)
                .Any(part => part.CurrentFood.Data.foodID == id);
        public bool Full() => _parts.All(p => p.IsOccupied);
        private bool AllEmpty() => _parts.All(p => p.IsOccupied == false);
        private bool AnyEmpty() => _parts.Any(p => p.IsOccupied == false);
        private Vector3 GetPartPosition(int idx) => _parts[idx].GetPlacePosition();
    }
}