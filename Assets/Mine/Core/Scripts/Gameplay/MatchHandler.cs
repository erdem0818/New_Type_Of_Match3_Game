using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Zenject;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public struct MatchAnimationStartSignal
    {
        public List<(int index, FoodView food)> IndexFoodTuples {get; set;}
        //public Vector3 MidPosition { get; set;}
    }

    public class MatchHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Platform _platform;

        private readonly int _requiredMatchCount = 3;

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
            //Info for debug
            await UniTask.Delay(2000);
            
            var uts = Enumerable.Select(pairs, pair 
                => UniTask.WaitUntil(() => pair.food.IsPlaced && pair.food.IsSliding == false)).ToList();

            await UniTask.WhenAll(uts);

            _signalBus.TryFire(new MatchAnimationStartSignal
            {
                IndexFoodTuples = pairs.ToList(),
                //MidPosition = midPosition
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

        /*private bool IsThereAnyMatch()
        {
            if (!IsThereAnyMatchCheck(out var matches)) return false;
            
            Debug.Log("MATCH".ToBold().ToColor(Color.blue));

            _signalBus.TryFire(new MatchAnimationStartSignal()
            {
                IndexFoodTuples = matches,
                MidPosition = matches[1].food.transform.position + Vector3.up * 0.5f
            });
            return true;
        }*/

        public bool IsThereAnyMatchCheck(out List<(int index, FoodView food)> matches)
        {
            int len = _platform.Parts.Count - _requiredMatchCount + 1;
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

                for (int j = 1; j < _requiredMatchCount; j++)
                {
                    FoodView neighbor = _platform.Parts[i + j].CurrentFood;
                    looks.Add(neighbor);
                }

                if (looks.Any(fv => fv == null))
                    continue;

                bool same = looks.All(fv => fv.Data.foodID == headID);

                if (!same) continue;
                List<(int index, FoodView food)> temp = new()
                {
                    //Info not hard code 3 -> required
                    new ValueTuple<int, FoodView>(i, looks[0]),
                    new ValueTuple<int, FoodView>(i + 1, looks[1]),
                    new ValueTuple<int, FoodView>(i + 2, looks[2])
                };
                
                temp.ForEach(pr => pr.food.MarkedForMatch = true);

                matches = temp;
                return true;
            }

            matches = null;
            return false;
        }
    }
}

