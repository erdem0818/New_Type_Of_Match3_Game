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
        public List<FoodView> Foods {get; set;}
    }

    public class MatchHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        //private readonly Platform _platform;

        private readonly FoodView[] _foodArray;

        private readonly int _requiredMatchCount = 3;

        public MatchHandler(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _foodArray = new FoodView[8];
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
            _foodArray[signal.PlacedIndex] = signal.Food;

            //todo look matches

            if (IsThereAnyMatch() == false) return;

            Debug.Log("MATCH");
        }

        private bool IsThereAnyMatch()
        {
            int len = _foodArray.Length - _requiredMatchCount + 1;
            for(int i = 0; i < len; i++) 
            {
                if (_foodArray[i] == null) continue;

                FoodView head = _foodArray[i];
                int headID = head.Data.foodID;

                List<FoodView> looks = new()
                {
                    head
                };

                for (int j = 1; j < _requiredMatchCount; j++)
                {
                    FoodView neighbor = _foodArray[i + j];
                    looks.Add(neighbor);
                }

                if (looks.Any(fv => fv == null))
                    continue;

                bool same = looks.All(fv => fv.Data.foodID == headID);

                if(same)
                {
                    _signalBus.TryFire(new MatchHappenedSignal() { Foods = looks });

                    return true;
                }
            }

            return false;
        }
    }
}

