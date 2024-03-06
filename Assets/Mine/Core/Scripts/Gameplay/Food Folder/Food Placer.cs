using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Gameplay.Signals;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Food_Folder
{
    public class FoodPlacer : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        public FoodPlacer(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FoodPlacingMovementStartSignal>(OnFoodPlacingMovementStart);
            _signalBus.Subscribe<MatchAnimationStartSignal>(OnMatchDetected);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodPlacingMovementStartSignal>(OnFoodPlacingMovementStart);
            _signalBus.TryUnsubscribe<MatchAnimationStartSignal>(OnMatchDetected);
        }

        private void OnFoodPlacingMovementStart(FoodPlacingMovementStartSignal signal)
        {
            PlaceFoodOnPlatformAsync(signal.Food, signal.PlacePosition).Forget();
        }
        
        private async UniTask PlaceFoodOnPlatformAsync(FoodView food, Vector3 placePosition)
        {
            Sequence seq = DOTween.Sequence();
            food.Sequence = seq
                .Append(food.transform.DOMove(placePosition, .5f))
                .Join(food.transform.DORotateQuaternion(Quaternion.identity, .5f))
                .SetId(food.GetInstanceID())
                .SetEase(Ease.OutCubic)
                .SetAutoKill(true);
            
            await seq.AsyncWaitForCompletion()
                .AsUniTask();

            food.IsPlaced = true;
            _signalBus.TryFire<FoodPlacingMovementFinishedSignal>();
        }

        public async UniTask SlideTheFood(FoodView food, Vector3 placePosition)
        {
            food.Sequence?.Kill();
            food.SlideTween?.Kill();
            food.IsSliding = true;
            
            food.SlideTween = food.transform.DOMove(placePosition, .15f)
                .SetId(food.GetInstanceID())
                .SetAutoKill(true);

            await food.SlideTween.AsyncWaitForCompletion().AsUniTask();
            
            food.IsSliding = false;
        }

        private void OnMatchDetected(MatchAnimationStartSignal signal)
        {
            PlayMatchAnimationAsync(signal).Forget();   
        }

        private async UniTask PlayMatchAnimationAsync(MatchAnimationStartSignal signal)
        {
            await PlayMatchAnimationAsync(signal.IndexFoodTuples);
            
            //Info after awaiting
            _signalBus.TryFire(new MatchAnimationFinishedSignal
            {
                IndexFoodTuples = signal.IndexFoodTuples
            });
            
            Debug.Log("All Match Anim Finished".ToBold().ToColor(new Color(0.75f, 0.75f, 0.25f)));

            foreach (var pair in signal.IndexFoodTuples)
            {
                Debug.Log($"index: {pair.index}");
            }
        }

        private async UniTask PlayMatchAnimationAsync(IEnumerable<(int index, FoodView food)> pairs)
        {
            var toList = pairs.ToList();
            
            //to debugHandler -> disable logs etc.
            Debug.Log("waiting for all foods placed".ToBold());
            
            var uts = Enumerable.Select(toList, pair=> UniTask.WaitUntil(() => pair.food.IsPlaced)).ToList();
            
            await UniTask.WhenAll(uts);
            
            Debug.Log("waiting for all foods placed finished".ToBold());
            
            //IMatchAnimationStrategy ?
            
            //Vector3 midPosition = toList[1].food.transform.position + Vector3.up * .5f;
            Vector3 midPosition = toList[0].food.transform.position;

            List<UniTask> tasks = Enumerable.Select(toList, pair => pair.food.transform.DOMove(midPosition, 0.25f)
                    //.SetEase(Ease.OutQuad)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                        {
                            Debug.Log("Anim complete".ToBold());
                            UnityEngine.Object.Destroy(pair.food.gameObject);
                        })
                    .AsyncWaitForCompletion()
                    .AsUniTask())
                .ToList();

            await UniTask.WhenAll(tasks);
        }
    }
}