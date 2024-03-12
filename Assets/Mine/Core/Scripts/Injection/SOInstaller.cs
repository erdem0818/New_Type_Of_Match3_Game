using Mine.Core.Scripts.Framework;
using Mine.Core.Scripts.Gameplay.Databases;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Injection
{
    [CreateAssetMenu(fileName = "SOInstaller", menuName = "Installers/SOInstaller")]
    public class SOInstaller : ScriptableObjectInstaller<SOInstaller>
    {
        [Header("Settings")]
        [HorizontalLine(2, EColor.Green)]
        [SerializeField] private Settings settings;

        [Header("Databases")]
        [HorizontalLine(2, EColor.Green)]
        [SerializeField] private FoodDatabase foodDatabase;
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private FruitSpriteDatabase fruitSpriteDatabase;
        
        public override void InstallBindings()
        {
            Container.BindInstance(settings);
            Container.BindInstance(foodDatabase);
            Container.BindInstance(levelDatabase);
            Container.BindInstance(fruitSpriteDatabase);
        }
    }
}