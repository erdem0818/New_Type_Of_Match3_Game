using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Gameplay.Signals;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay
{
    public class MatchHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Platform _platform;

        private const int RequiredMatchCount = 3;

        public MatchHandler(SignalBus signalBus, Platform platform)
        {
            _signalBus = signalBus;
            _platform = platform;   
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FoodPlacingMovementFinishedSignal>(OnFoodPlacingMovementFinished);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodPlacingMovementFinishedSignal>(OnFoodPlacingMovementFinished);
        }

        public void RequestMatch(IEnumerable<(int index, FoodView food)> pairs)
        {
            PlayRequestedMatchAnimation(pairs.ToList()).Forget();
        }

        private async UniTask PlayRequestedMatchAnimation(IList<(int index, FoodView food)> pairs)
        {
            var uts = Enumerable.Select(pairs, pair 
                => UniTask.WaitUntil(() => pair.food.IsPlaced && pair.food.IsSliding == false)).ToList();

            await UniTask.WhenAll(uts);

            _signalBus.TryFire(new MatchAnimationStartSignal
            {
                IndexFoodTuples = pairs.ToList(),
            });
        }

        private void OnFoodPlacingMovementFinished(FoodPlacingMovementFinishedSignal signal)
        {
            if (IsThereAnyMatchCheck(out _)) return;
            
            if(_platform.Full())
            {
                Debug.Log($"FAIL"
                    .ToBold()
                    .ToColor(new Color(0.85f, 0.15f, 0.05f)));
            }
        }

        public bool IsThereAnyMatchCheck(out List<(int index, FoodView food)> matches)
        {
            int len = _platform.Parts.Count - RequiredMatchCount + 1;
            for(int i = 0; i < len; i++) 
            {
                if (_platform.Parts[i].CurrentFood == null) continue;
                if (_platform.Parts[i].CurrentFood.MarkedForMatch) continue;

                FoodView head = _platform.Parts[i].CurrentFood;
                int headID = head.Data.foodID;

                List<FoodView> looks = new()
                {
                    head
                };

                for (int j = 1; j < RequiredMatchCount; j++)
                {
                    FoodView neighbor = _platform.Parts[i + j].CurrentFood;
                    looks.Add(neighbor);
                }

                if (looks.Any(fv => fv == null))
                    continue;

                bool same = looks.All(fv => fv.Data.foodID == headID);

                if (!same) continue;
                
                List<(int Index, FoodView food)> temp = new();

                for (int j = 0; j < RequiredMatchCount; j++) // 0 1 2
                {
                    temp.Add(new ValueTuple<int, FoodView>(i + j, looks[j]));
                }

                temp.ForEach(pr => pr.food.MarkedForMatch = true);

                matches = temp;
                return true;
            }

            matches = null;
            return false;
        }
    }
}