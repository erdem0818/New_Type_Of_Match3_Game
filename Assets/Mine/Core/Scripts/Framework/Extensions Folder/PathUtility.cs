using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Mine.Core.Scripts.Framework.Extensions_Folder
{
    public static class PathUtility
    {
        public static List<string> GetPathsInPath(string filter, string actualPath)
        {
            string[] guids = AssetDatabase.FindAssets(filter, new[] { actualPath });
            return guids.Select(AssetDatabase.GUIDToAssetPath).ToList();
        }
    }
}
