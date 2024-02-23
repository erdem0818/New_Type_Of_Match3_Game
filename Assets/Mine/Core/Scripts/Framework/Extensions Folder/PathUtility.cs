using System.Collections.Generic;
using UnityEditor;

namespace Assets.Mine.Core.Scripts.Framework.Extensions_Folder
{
    public static class PathUtility
    {
        public static List<string> GetPathsInPath(string filter, string actualPath)
        {
            List<string> result = new();

            string[] guids = AssetDatabase.FindAssets(filter, new[] { actualPath });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                result.Add(path);
            }
            return result;
        }
    }
}
