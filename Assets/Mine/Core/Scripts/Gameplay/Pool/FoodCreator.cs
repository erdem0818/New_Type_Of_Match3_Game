using System.Linq;
using Mine.Core.Scripts.Gameplay.Databases;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Pool
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

        public Food Create(int id)
        {
            GameObject prefab = _foodDatabase.First(fd => fd.foodID == id).foodPrefab;

            return _diContainer.InstantiatePrefab(prefab, new GameObjectCreationParameters
            {
                ParentTransform = _parent,
                Position = (Random.insideUnitSphere * 2.5f) + (Vector3.up * Random.Range(3,5))
            }).GetComponent<Food>();
        }
    }
}