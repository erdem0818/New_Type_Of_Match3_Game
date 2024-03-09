using System.Linq;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Gameplay.Level_Folder;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(menuName = "Data/Level Database", fileName = "Level Database")]
    public class LevelDatabase : Database<LevelData>
    {
        private const string LevelDataPath = "Assets/Mine/Core/Scripts/Data/Level Datas";

#if UNITY_EDITOR
        [Button]
        private void FillDB()
        {
            items.Clear();
            var paths = PathUtility.GetPathsInPath(Defines.SoFilter, LevelDataPath);
            //items = AssetDatabase.LoadAllAssetsAtPath(LevelDataPath).ToListAsConvert<Object, LevelData>();
            
            // items = paths
            //     .Select(AssetDatabase.LoadAssetAtPath<LevelData>)
            //     .Where(dbObj => dbObj != null)
            //     .ToList();

            foreach (var dbObj in paths
                         .Select(AssetDatabase.LoadAssetAtPath<LevelData>)
                         .Where(dbObj => dbObj != null))
            {
                items.Add(dbObj);
            }

            // foreach (var path in paths)
            // {
            //     var dbObj = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            //     if (dbObj != null) items.Add(dbObj);
            // }
        }
#endif
    }
}