using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.FoodFolder
{
    public struct FoodPlacedSignal
    {
        public int PlacedIndex { get; set; }
        public FoodView Food { get; set; }
        public Vector3 PlacePosition { get; set; }
    }

    public struct FoodPlacingMovementFinishedSignal
    {}

    public struct MatchAnimationFinishedSignal
    {}

    public class FoodPlacer : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        public FoodPlacer(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FoodPlacedSignal>(OnFoodPlaced);
            _signalBus.Subscribe<MatchHappenedSignal>(OnMatchHappened);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodPlacedSignal>(OnFoodPlaced);
            _signalBus.TryUnsubscribe<MatchHappenedSignal>(OnMatchHappened);
        }

        private void OnFoodPlaced(FoodPlacedSignal signal)
        {
            PlaceFoodOnPlatformAsync(signal.PlacedIndex, signal.Food, signal.PlacePosition)
                .Forget();
        }

        public async UniTask PlaceFoodOnPlatformAsync(int index, FoodView food, Vector3 placePosition)
        {
            //info move stuff
            Sequence seq = DOTween.Sequence();
            await seq
                .Append(food.transform.DOMove(placePosition, 0.45f))
                .Join(food.transform.DORotateQuaternion(Quaternion.identity, 0.45f))
                .SetEase(Ease.OutCubic)
                .SetAutoKill(true)
                .AsyncWaitForCompletion();

            _signalBus.TryFire<FoodPlacingMovementFinishedSignal>();
        }

        public async UniTask SlideTheFood(FoodView food, Vector3 placePosition)
        {
            await food.transform.DOMove(placePosition, 0.1f)
                .SetAutoKill(true)
                .AsyncWaitForCompletion();
        }

        private void OnMatchHappened(MatchHappenedSignal signal)
        {
            for (int i = 0; i < signal.IndexFoodTuples.Count; i++)
            {
                //todo animation
                UnityEngine.Object.Destroy(signal.IndexFoodTuples[i].food.gameObject);
            }

            _signalBus.TryFire<MatchAnimationFinishedSignal>();
        }
    }
}