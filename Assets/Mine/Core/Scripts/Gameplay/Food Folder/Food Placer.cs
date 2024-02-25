using System;
using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.FoodFolder
{
    public struct FoodPlacedSignal
    {
        public FoodView Food { get; set; }
        public int PlacedIndex { get; set; }
    }

    public class FoodPlacer : IInitializable, IDisposable
    {
        private readonly Platform _platform;
        private readonly SignalBus _signalBus;

        public FoodPlacer(Platform platform, SignalBus signalBus)
        {
            _platform = platform;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FoodClickedSignal>(OnClickedFood);
            _signalBus.Subscribe<MatchHappenedSignal>(OnMatchHappened);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodClickedSignal>(OnClickedFood);
            _signalBus.TryUnsubscribe<MatchHappenedSignal>(OnMatchHappened);
        }

        private void OnClickedFood(FoodClickedSignal signal)
        {
            if (CanPlace() == false)
                return;

            TryPlace(signal.Food).Forget();
        }

        private bool CanPlace()
        {
            return _platform.GetFirstEmptyIndex() != Defines.AllFull;
        }

        private async UniTask TryPlace(FoodView food)
        {
            //todo refactor all movement-animation logic.
            //todo place same objects near by near

            int gonnaPlaceIndex = _platform.GetPlaceIndex(food.Data.foodID);
            Debug.Log($"PlaceIndex: {gonnaPlaceIndex}".ToBold());

            _platform.SetOccupationStatus(gonnaPlaceIndex, food, true);

            food.transform.DOMove(_platform.GetPartPosition(gonnaPlaceIndex), 0.25f).SetAutoKill(true).SetEase(Ease.InCubic);
            food.transform.DORotateQuaternion(Quaternion.identity, 0.25f).SetAutoKill(true);
            food.IsSelected = true;
            food.SetPhysics(false);

            await UniTask.Delay(TimeSpan.FromMilliseconds(300));

            _signalBus.TryFire(new FoodPlacedSignal(){Food = food, PlacedIndex = gonnaPlaceIndex });
        }

        private void OnMatchHappened(MatchHappenedSignal signal)
        {
            //todo set occupation status
            for (int i = 0; i < signal.IndexFoodTuples.Count; i++)
            {
                _platform.SetOccupationStatus(signal.IndexFoodTuples[i].Item1, null, false);
                //todo animation
                UnityEngine.Object.Destroy(signal.IndexFoodTuples[i].Item2.gameObject);
            }

            _platform.ReOrderAll();
        }
    }
}