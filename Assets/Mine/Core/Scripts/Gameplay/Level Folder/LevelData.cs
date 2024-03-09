using System.Collections.Generic;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [System.Serializable]
    public struct LevelDataPair
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
        public List<LevelDataPair> levelDataPairs;
        [Header("Quest")] 
        [HorizontalLine(2, EColor.Green)]
        public QuestData questData;
        [Header("Time")]
        [HorizontalLine(2, EColor.Green)]
        public float levelTime;
    }
}

