using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Zenject;
using UnityEngine;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public struct MatchHappenedSignal
    {
        public List<ValueTuple<int, FoodView>> IndexFoodTuples {get; set;}
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
            _signalBus.Subscribe<FoodPlacedSignal>(OnFoodPlaced);
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodPlacedSignal>(OnFoodPlaced);
        }

        private void OnFoodPlaced(FoodPlacedSignal signal)
        {
            if (IsThereAnyMatch() == false) return;

            Debug.Log("MATCH");
        }

        private bool IsThereAnyMatch()
        {
            int len = _platform.Foods.Length - _requiredMatchCount + 1;
            for(int i = 0; i < len; i++) 
            {
                if (_platform.Foods[i] == null) continue;

                FoodView head = _platform.Foods[i];
                int headID = head.Data.foodID;

                List<FoodView> looks = new()
                {
                    head
                };

                for (int j = 1; j < _requiredMatchCount; j++)
                {
                    FoodView neighbor = _platform.Foods[i + j];
                    looks.Add(neighbor);
                }

                if (looks.Any(fv => fv == null))
                    continue;

                bool same = looks.All(fv => fv.Data.foodID == headID);

                if(same)
                {
                    List<ValueTuple<int, FoodView>> matches = new()
                    {
                        new ValueTuple<int, FoodView>(i, looks[0]),
                        new ValueTuple<int, FoodView>(i + 1, looks[1]),
                        new ValueTuple<int, FoodView>(i + 2, looks[2])
                    };

                    //todo but wait animation
                    _signalBus.TryFire(new MatchHappenedSignal()
                    {
                        IndexFoodTuples = matches
                    });

                    return true;
                }
            }

            return false;
        }
    }
}

