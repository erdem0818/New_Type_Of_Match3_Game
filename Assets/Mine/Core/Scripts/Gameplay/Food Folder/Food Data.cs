using NaughtyAttributes;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Food_Folder
{
    [CreateAssetMenu(menuName = "Data/Food Data", fileName = "Food Data")]
    public class FoodData : ScriptableObject
    {
        [Header("Food Info")]
        [HorizontalLine(1, EColor.Orange)]
        public string foodName;
        [Min(0)]
        public int foodID;
        [ShowAssetPreview(64,64)]        
        public GameObject foodPrefab;
    }
}

