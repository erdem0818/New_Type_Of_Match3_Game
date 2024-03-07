using Assets.Mine.Core.Scripts.Utility;
using Mine.Core.Scripts.Gameplay.Databases;
using Mine.Core.Scripts.Gameplay.Pool;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [System.Serializable]
    public struct LevelArgs
    {
        public int LastPlayedLevel { get; set; }
        public static LevelArgs Empty = new() { LastPlayedLevel = 0 };
    }

    public class LevelHandler : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly LevelDatabase _database;
        private readonly FoodCreator _creator;

        public LevelHandler(SignalBus signalBus, LevelDatabase database, FoodCreator creator)
        {
            _signalBus = signalBus;
            _database = database;
            _creator = creator;
        }

        public void Initialize()
        {
            CreateFoods();
        }

        private void CreateFoods()
        {
            //todo get from save
            var save = SaveAPI.GetOrCreateData(Defines.LevelSaveKey, LevelArgs.Empty);
            LevelData data = _database.GetDBItem(save.LastPlayedLevel);
            if (data == null) 
            {
                Debug.LogError("Level Data is null");
                return; 
            }

            foreach(LevelDataPair pair in data.levelDataPairs)
            {
                for (int i = 0; i < pair.count; i++)
                    _creator.Create(pair.food.foodID);
            }
        }
    }
}