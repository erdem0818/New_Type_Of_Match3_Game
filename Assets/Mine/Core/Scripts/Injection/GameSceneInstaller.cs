using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Game;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Gameplay;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using Mine.Core.Scripts.Gameplay.Level_Folder;
using Mine.Core.Scripts.Gameplay.Pool;
using Mine.Core.Scripts.Gameplay.Signals;
using Mine.Core.Scripts.Gameplay.UI.Panels;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        [Inject] private readonly IPanelService _panelService;
        [Inject] private readonly GameHandler _gameHandler;
        
        [SerializeField] private Transform foodParent;
        [SerializeField] private Transform platformParent;
        
        public override void InstallBindings()
        {
            BindSignals();

            Container.BindInstance(foodParent);

            Container.BindInterfacesAndSelfTo<Platform>().AsSingle().WithArguments(platformParent);
            Container.BindInterfacesAndSelfTo<MatchHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<FoodPlacer>().AsSingle();
            Container.Bind<FoodCreator>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelHandler>().AsSingle();
            
            _panelService.ShowPanel<GameplayPanel>().Forget();
            //_gameHandler.GameplayState.Value = GameplayState.Running;
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