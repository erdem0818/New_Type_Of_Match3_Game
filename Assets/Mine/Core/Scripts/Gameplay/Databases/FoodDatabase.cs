using System.Linq;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Databases
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
            var paths = PathUtility.GetPathsInPath(Defines.SoFilter, FoodPrefabPath);
             items = paths
                 .Select(AssetDatabase.LoadAssetAtPath<FoodData>)
                 .Where(dbObj => dbObj != null)
                 .ToList();

            //items = AssetDatabase.LoadAllAssetsAtPath(FoodPrefabPath).ToListAsConvert<Object, FoodData>();
            // foreach (var dbObj in paths
            //              .Select(AssetDatabase.LoadAssetAtPath<FoodData>)
            //              .Where(dbObj => dbObj != null))
            // {
            //     items.Add(dbObj);
            // }

            // foreach ( var path in paths ) 
            // {
            //     var dbObj = AssetDatabase.LoadAssetAtPath<FoodData>(path);
            //     if (dbObj != null) items.Add(dbObj);
            // }
        }
#endif
    }
}