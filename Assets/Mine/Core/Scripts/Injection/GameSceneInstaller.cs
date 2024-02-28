using Assets.Mine.Core.Scripts.Gameplay;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Zenject;
using UnityEngine;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Assets.Mine.Core.Scripts.Gameplay.Database;
using Assets.Mine.Core.Scripts.Gameplay.Pool;
using Assets.Mine.Core.Scripts.Gameplay.Level;

namespace Assets.Mine.Core.Scripts.Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Transform _foodParent;
        [SerializeField] private FoodDatabase foodDatabase;
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private Transform platform;

        public override void InstallBindings()
        {
            Container.BindInstance(_foodParent);
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