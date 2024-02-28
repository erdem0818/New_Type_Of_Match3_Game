using Assets.Mine.Core.Scripts.Gameplay.Database;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.Pool
{
    public class FoodCreator
    {
        private readonly DiContainer _diContainer;
        private readonly FoodDatabase _foodDatabase;
        private readonly Transform _parent;

        public FoodCreator(DiContainer diContainer, FoodDatabase foodDatabase, Transform parent)
        {
            _diContainer = diContainer;
            _foodDatabase = foodDatabase;   
            _parent = parent;
        }

        public FoodView Create(int id)
        {
            GameObject prefab = _foodDatabase.First(fd => fd.foodID == id).foodPrefab;

            return _diContainer.InstantiatePrefab(prefab, new GameObjectCreationParameters()
            {
                ParentTransform = _parent,
                Position = (Random.insideUnitSphere * 2.5f) + (Vector3.up * Random.Range(3,5))
            }).GetComponent<FoodView>();
        }
    }
}