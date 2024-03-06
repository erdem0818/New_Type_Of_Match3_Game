using Assets.Mine.Core.Scripts.Gameplay.Database;
using Assets.Mine.Core.Scripts.Gameplay.Pool;
using Assets.Mine.Core.Scripts.Utility;
using Mine.Core.Scripts.Gameplay;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.Level
{
    [System.Serializable]
    public struct LevelArgs
    {
        public int LastPlayedLevel { get; set; }

        public static LevelArgs Empty = new LevelArgs { LastPlayedLevel = 0 };
    }

    public class LevelHandler : IInitializable
    {
        private readonly LevelDatabase _database;
        private readonly FoodCreator _creator;

        public LevelHandler(LevelDatabase dataBase, FoodCreator creator)
        {
            _database = dataBase;
            _creator = creator;
        }

        public void Initialize()
        {
            CreateFoods();   
        }

        private void CreateFoods()
        {
            //todo get from save
            var save = SaveAPI.GetOrCreateData<LevelArgs>(Defines.LevelSaveKey, LevelArgs.Empty);
            LevelData data = _database.GetDBItem(save.LastPlayedLevel);
            if (data == null) 
            {
                Debug.LogError("Level Data is null");
                return; 
            }

            foreach(LevelDataPair pair in data.levelDataPairs)
            {
                for (int i = 0; i < pair.count; i++)
                {
                    _creator.Create(pair.food.foodID);
                }
            }
        }
    }
}