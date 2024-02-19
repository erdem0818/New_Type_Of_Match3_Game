using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Zenject;

namespace Assets.Mine.Core.Scripts.Injection
{
    public class GameSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindSignals();
        }

        private void BindSignals()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<FoodClickedSignal>().OptionalSubscriber();
        }
    }
}

