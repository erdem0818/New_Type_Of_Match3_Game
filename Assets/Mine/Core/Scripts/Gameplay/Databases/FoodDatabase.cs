using Assets.Mine.Core.Scripts.Framework.Extensions_Folder;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Assets.Mine.Core.Scripts.Gameplay.Database
{
    [CreateAssetMenu(menuName = "Data/Food Database", fileName = "Food Database")]
    public class FoodDatabase : Database<FoodData>
    {
        private const string FoodPrefabPath = "Assets/Mine/Core/Scripts/Data/Food Datas";

#if UNITY_EDITOR
        [Button]
        private void FillDB()
        {
            items.Clear();
            var paths = PathUtility.GetPathsInPath(Defines.SOFilter, FoodPrefabPath);
            foreach ( var path in paths ) 
            {
                var dbObj = AssetDatabase.LoadAssetAtPath<FoodData>(path);
                if (dbObj != null) items.Add(dbObj);
            }
        }
#endif
    }
}