using System.Linq;
using Mine.Core.Scripts.Framework.Extensions_Folder;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.Databases
{
    [CreateAssetMenu(menuName = "Data/Sprite Database", fileName = "Sprite Database")]
    public class FruitSpriteDatabase : Database<Sprite>
    {
        private const string SpritesPath = "Assets/Simasart/Hungry Bat/Art/Fruits";
        
#if UNITY_EDITOR
        [Button]
        private void FillDB()
        {
            items.Clear();
            var paths = PathUtility.GetPathsInPath(Defines.SpriteFilter, SpritesPath);
            items = paths
                .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
                .Where(dbObj => dbObj != null)
                .ToList();
        }
#endif
    }
}
