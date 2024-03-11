using System;
using Mine.Core.Scripts.Gameplay.Databases;
using Mine.Core.Scripts.Gameplay.Pool;
using Mine.Core.Scripts.Gameplay.Signals;
using Mine.Core.Scripts.Utility;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [Serializable]
    public struct LevelArgs
    {
        public int LastPlayedLevel { get; set; }
        public static LevelArgs Empty = new() { LastPlayedLevel = 0 };
    }

    public class LevelHandler : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly LevelDatabase _database;
        private LevelData _currentData;
        private readonly FoodCreator _creator;

        public LevelHandler(SignalBus signalBus, LevelDatabase database, FoodCreator creator)
        {
            _signalBus = signalBus;
            _database = database;
            _creator = creator;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<FoodClickedSignal>(OnClickFood);
            
            CreateFoods();
        }
        
        public void Dispose()
        {
            _signalBus.TryUnsubscribe<FoodClickedSignal>(OnClickFood);
        }

        private void OnClickFood(FoodClickedSignal signal)
        {
            if (_currentData.questData.Any(signal.Food.Data))
            {
                Debug.Log($"reduce {signal.Food.Data.foodName} -1");
            }
        }

        private void CreateFoods()
        {
            //todo get from save
            var save = SaveAPI.GetOrCreateData(Defines.LevelSaveKey, LevelArgs.Empty);
            _currentData = _database.GetDBItem(save.LastPlayedLevel);
            if (_currentData == null) 
            {
                Debug.LogError("Level Data is null");
                return; 
            }

            foreach(FoodCountPair pair in _currentData.levelDataPairs)
            {
                for (int i = 0; i < pair.count; i++)
                    _creator.Create(pair.food.foodID);
            }
        }
    }
}