using System.Collections.Generic;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Signals
{
    public struct FoodClickedSignal
    {
        public FoodView Food { get; set; }
    }
    
    public struct FoodPlacingMovementStartSignal
    {
        public int PlacedIndex { get; set; }
        public FoodView Food { get; set; }
        public Vector3 PlacePosition { get; set; }
    }

    public struct FoodPlacingMovementFinishedSignal
    {}

    public struct MatchAnimationStartSignal
    {
        public List<(int index, FoodView food)> IndexFoodTuples {get; set;}
    }
    
    public struct MatchAnimationFinishedSignal
    {
        public List<(int index, FoodView food)> IndexFoodTuples {get; set;}
    }
}
