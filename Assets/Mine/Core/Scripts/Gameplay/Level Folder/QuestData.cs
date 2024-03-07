using System.Collections.Generic;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [CreateAssetMenu(fileName = "Quest Data", menuName = "Data/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [SerializeField] private List<FoodData> questFoods;
        public List<FoodData> QuestFoods => questFoods;
    }
}