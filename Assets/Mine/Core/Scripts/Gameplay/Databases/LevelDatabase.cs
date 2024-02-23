using Assets.Mine.Core.Scripts.Framework.Extensions_Folder;
using Assets.Mine.Core.Scripts.Gameplay.Level;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Assets.Mine.Core.Scripts.Gameplay.Database
{
    [CreateAssetMenu(menuName = "Data/Level Database", fileName = "Level Database")]
    public class LevelDatabase : Database<LevelData>
    {
        private const string FoodPrefabPath = "Assets/Mine/Core/Scripts/Data/Level Datas";

#if UNITY_EDITOR
        [Button]
        private void FillDB()
        {
            items.Clear();
            var paths = PathUtility.GetPathsInPath(Defines.SOFilter, FoodPrefabPath);
            foreach (var path in paths)
            {
                var dbObj = AssetDatabase.LoadAssetAtPath<LevelData>(path);
                if (dbObj != null) items.Add(dbObj);
            }
        }
#endif
    }
}