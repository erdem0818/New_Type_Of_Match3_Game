using System.Collections.Generic;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Signals
{
    public struct FoodClickedSignal
    {
        public Food Food { get; set; }
    }
    
    public struct FoodPlacingMovementStartSignal
    {
        public int PlacedIndex { get; set; }
        public Food Food { get; set; }
        public Vector3 PlacePosition { get; set; }
    }

    public struct FoodPlacingMovementFinishedSignal
    {}

    public struct MatchAnimationStartSignal
    {
        public List<(int index, Food food)> IndexFoodTuples {get; set;}
    }
    
    public struct MatchAnimationFinishedSignal
    {
        public List<(int index, Food food)> IndexFoodTuples {get; set;}
    }
}
