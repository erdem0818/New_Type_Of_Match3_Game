using Assets.Mine.Core.Scripts.Gameplay.Database;
using Mine.Core.Scripts.Gameplay;
using Mine.Core.Scripts.Gameplay.Databases;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using Mine.Core.Scripts.Gameplay.Level_Folder;
using Mine.Core.Scripts.Gameplay.Pool;
using Mine.Core.Scripts.Gameplay.Signals;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform foodParent;
        [SerializeField] private FoodDatabase foodDatabase;
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private Transform platform;

        public override void InstallBindings()
        {
            Container.BindInstance(foodParent);
            Container.BindInstance(foodDatabase);
            Container.BindInstance(levelDatabase);

            Container.BindInterfacesAndSelfTo<Platform>().AsSingle().WithArguments(platform);
            Container.BindInterfacesAndSelfTo<FoodPlacer>().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchHandler>().AsSingle();
            Container.Bind<FoodCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelHandler>().AsSingle();

            BindSignals();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<FoodClickedSignal>().OptionalSubscriber();
            Container.DeclareSignal<FoodPlacingMovementStartSignal>().OptionalSubscriber();
            Container.DeclareSignal<FoodPlacingMovementFinishedSignal>().OptionalSubscriber();
            Container.DeclareSignal<MatchAnimationStartSignal>().OptionalSubscriber();
            Container.DeclareSignal<MatchAnimationFinishedSignal>().OptionalSubscriber();
        }
    }
}