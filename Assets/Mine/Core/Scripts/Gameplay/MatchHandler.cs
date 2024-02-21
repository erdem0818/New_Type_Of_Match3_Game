using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public struct MatchHappenedSignal
    {
        public List<FoodView> Foods {get; set;}
    }

    public class MatchHandler
    {
        private readonly Platform _platform;
        private readonly FoodView[] _foodArray;

        public MatchHandler(Platform platform)
        {
            _platform = platform;
            _foodArray = new FoodView[_platform.PlatformLength];
        }
    }
}

