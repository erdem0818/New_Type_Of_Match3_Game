using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using Mine.Core.Scripts.Gameplay.Signals;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay
{
    public class PlatformPart
    {
        public Transform Transform { get; set; }
        public bool IsOccupied { get; set; }
        public bool IsLocked { get; set; }
        public Food CurrentFood{ get; set; }

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
        private readonly MatchHandler _matchHandler;

        public Platform(SignalBus signalBus, Transform transform, FoodPlacer foodPlacer, MatchHandler matchHandler)
        {
            _signalBus = signalBus;
            _transform = transform;
            _foodPlacer = foodPlacer;
            _matchHandler = matchHandler;
        }

        public List<PlatformPart> Parts { get; } = new();

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
                Parts.Add(new PlatformPart
                {
                    Transform = _transform.GetChild(i),
                    IsOccupied = false
                });
            }
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
            
            bool b = _matchHandler.IsThereAnyMatchCheck(out var matches);
             Debug.Log(b ?
                 "There is Match"
                     .ToBold()
                     .ToColor(new Color(0.9f, 0.1f, 0.9f)) :
                 "No Match".
                     ToBold()
                     .ToColor(new Color(.1f, .1f, .1f)));

             if (!b) return;
             
             _matchHandler.RequestMatch(matches);
        }

        private void SetOccupationStatusForMatchElements(IEnumerable<(int index, Food food)> pairs)
        {
            foreach ((int index, Food food) pair in pairs)
            {
                //fix this line ??
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

        private void SetOccupationStatus(int idx, Food food, bool occupied)
        {
            if (occupied && Parts[idx].IsOccupied)
            {
                MoveAllRightSide(idx);
                ReorderAllMovement();
                
                Debug.Log("Already occupied"
                    .ToBold()
                    .ToColor(new Color(0.75f, 0.25f, 0.45f, 1.0f)));
            }

            Parts[idx].IsOccupied = occupied;
            Parts[idx].CurrentFood = food;

            Debug.Log($"Occupation: {occupied}".ToBold());
        }
        
        public void ReorderAllMovement()
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Food food = Parts[i].CurrentFood;
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
                if (Parts[i].CurrentFood == null)
                    continue;

                Food food = Parts[i].CurrentFood;
                Parts[i + 1].IsOccupied = true;
                Parts[i + 1].CurrentFood = food;
            }
        }

        private void MoveAllLeftSide()
        {
            foreach (var part in Parts)
            {
                if (part.CurrentFood == null)
                    continue;

                Food food = part.CurrentFood;
                
                part.IsOccupied = false;
                part.CurrentFood = null;
                
                int empty = GetFirstEmptyIndex();
                Parts[empty].IsOccupied = true;
                Parts[empty].CurrentFood = food;
            }
        }
        
        public void Reset()
        {
            foreach (var part in Parts)
            {
                part.IsOccupied = false;
            }

            foreach (var part in Parts)
            {
                var food = part.CurrentFood;
                if (food == null)
                    continue;
                food.GetComponent<Rigidbody>().AddForce(Vector3.forward * 2f, ForceMode.Impulse);
                food.SetPhysics(true);
                food.IsSelected = false;
                food.IsPlaced = false;
                food.MarkedForMatch = false;
                food.Sequence.Kill();
                part.CurrentFood = null;
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
            for (int i = 0; i < Parts.Count; i++)
            {
                Food temp = Parts[i].CurrentFood;
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

            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i].IsOccupied == false)
                    return i;
            }

            return Defines.AllFull;
        }

        private int GetLastFullIndex()
        {
            int lastFullIndex = 0;
            for (int i = 0; i < Parts.Count; i++)
            {
                if (Parts[i].CurrentFood != null) lastFullIndex = i;
            }
            return lastFullIndex;
        }

        private bool IsThereAnySameFood(int id)  => 
            Parts.Where(part => part.CurrentFood != null)
                .Any(part => part.CurrentFood.Data.foodID == id);
        public bool Full() => Parts.All(p => p.IsOccupied);
        private bool AllEmpty() => Parts.All(p => p.IsOccupied == false);
        private bool AnyEmpty() => Parts.Any(p => p.IsOccupied == false);
        private Vector3 GetPartPosition(int idx) => Parts[idx].GetPlacePosition();
    }
}