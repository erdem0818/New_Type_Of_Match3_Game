using System;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using DG.Tweening;
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
        }

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodClickedSignal>(OnClickedFood);
        }

        private void OnClickedFood(FoodClickedSignal signal)
        {
            if(TryPlace(signal.Food))
            {
                signal.Food.SetPhysics(false);
            }
        }

        private bool TryPlace(FoodView food)
        {
            var ind = _platform.GetFirstEmptyIndex();
            if(ind == Defines.AllFull)
                return false;

            int emptyIndex = _platform.GetFirstEmptyIndex(); 
            _platform.SetOccupationStatus(emptyIndex, true);

            food.transform.DOMove(_platform.GetPartPosition(emptyIndex), 0.25f).SetAutoKill(true);
            food.IsSelected = true;

            _signalBus.TryFire(new FoodPlacedSignal(){Food = food, PlacedIndex = emptyIndex});

            return true;
        }
    }
}

