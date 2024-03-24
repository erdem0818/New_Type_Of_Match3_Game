using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Gameplay;
using UniRx;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Framework.Game
{
    public enum AppState
    {
        Bootstrapped = 0,
        MainMenu = 1,
        Gameplay = 2
    }

    public enum GameplayState
    {
        Ready = 0,
        Stopped = 1,
        Running = 2,
        Finished = 3,
        InUI = 4
    }
    
    //todo maybe UI State ?
    
    public class GameHandler : IInitializable
    {
        //todo subject and IObservables ?

        //todo make them private and define only enums
        private ReactiveProperty<AppState> AppStateProp { get; } = new();
        private ReactiveProperty<GameplayState> GameplayStateProp { get; } = new();

        public AppState AppState
        {
            get => AppStateProp.Value;
            set => AppStateProp.Value = value;
        }
        public GameplayState GameplayState
        {
            get => GameplayStateProp.Value;
            set => GameplayStateProp.Value = value;
        }
        
        public BoolReactiveProperty IsGameStarted { get; set; }
        public BoolReactiveProperty IsGameFinished { get; set; }
        
        public void Initialize()
        {
            AppState = AppState.Bootstrapped;
            GameplayState = GameplayState.Ready;

            AppStateProp.Subscribe(_ =>
            {
                Debug.Log(AppStateProp.Value.ToString().ToBold().ToColor(Defines.Chocolate));
            });
            
            GameplayStateProp.Subscribe(_ =>
            {
                Debug.Log(GameplayStateProp.Value.ToString().ToBold().ToColor(Defines.Carrot));
            });
        }
    }
}
