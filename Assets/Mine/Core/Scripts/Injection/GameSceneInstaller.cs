using Assets.Mine.Core.Scripts.Gameplay;
using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Zenject;
using UnityEngine;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;

namespace Assets.Mine.Core.Scripts.Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private Platform platform;

        public override void InstallBindings()
        {
            Container.BindInstance(platform);
            Container.BindInterfacesAndSelfTo<FoodPlacer>().AsSingle();
            Container.BindInterfacesAndSelfTo<MatchHandler>().AsSingle();

            BindSignals();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<FoodClickedSignal>().OptionalSubscriber();
            Container.DeclareSignal<MatchHappenedSignal>().OptionalSubscriber();
            Container.DeclareSignal<FoodPlacedSignal>().OptionalSubscriber();
        }
    }
}

