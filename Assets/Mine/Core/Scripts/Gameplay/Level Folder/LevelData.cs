using System.Collections.Generic;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [System.Serializable]
    public struct FoodCountPair
    {
        public FoodData food;
        [Min(0)]
        public int count;
    }

    [CreateAssetMenu(menuName = "Data/Level Data", fileName = "Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Start Foods")]
        [HorizontalLine(2, EColor.Yellow)]
        public List<FoodCountPair> levelDataPairs;
        [Header("Quest")] 
        [HorizontalLine(2, EColor.Green)]
        public QuestData questData;
        [Header("Time")]
        [HorizontalLine(2, EColor.Green)]
        public float levelTime;
    }
}

