﻿using System.Collections.Generic;
using System.Linq;
using Mine.Core.Scripts.Gameplay.Food_Folder;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Level_Folder
{
    [CreateAssetMenu(fileName = "Quest Data", menuName = "Data/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [SerializeField] private List<FoodCountPair> questFoods;
        public List<FoodCountPair> QuestFoods => questFoods;

        public bool Any(FoodData data) => questFoods.Any(d => d.food.foodID == data.foodID);
    }
}