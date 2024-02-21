using UnityEngine;
using UnityEditor;
using Zenject;
using System.Collections.Generic;
using Assets.Mine.Core.Scripts.Gameplay;
using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;

public class FoodDataCreator : ZenjectEditorWindow
{
    private Vector2 _scrollPosition;
    private const string _path = "Assets/Mine/Core/Prefabs/Food Prefabs";
    private const string _dataPath = "Assets/Mine/Core/Scripts/Data/Food Datas";

    [MenuItem("Window/FoodDataCreator")]
    public static FoodDataCreator GetOrCreateWindow()
    {
        var window = EditorWindow.GetWindow<FoodDataCreator>();
        window.titleContent = new GUIContent("FoodDataCreator");
        return window;
    }

    public override void OnGUI()
    {
        GUILayout.Label("Prefabs in path: " + _path, EditorStyles.boldLabel);

        /* if (GUILayout.Button("Refresh List"))
        {

        } */

        DisplayPrefabs();
    }

    private void DisplayPrefabs()
    {
        Color originalColor = GUI.backgroundColor;

        List<string> paths = GetPathsInPath(Defines.PrefabFilter, _path);
        List<string> dataPaths = GetPathsInPath(Defines.SOFilter, _dataPath);

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, new GUIStyle() { });

        foreach (var path in paths)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            Texture2D preview = AssetPreview.GetAssetPreview(obj);

            GUILayout.BeginHorizontal();
            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(80), GUILayout.Height(80));
            }
            else
            {
                GUILayout.Label("No Preview", GUILayout.Width(64),GUILayout.Height(64));
            }

            string buttonText = $"Select {obj.name}"; 
            if (GUILayout.Button(buttonText,
            GUILayout.Width(200), 
            GUILayout.Height(50),
            GUILayout.ExpandWidth(false)))
            {
                Selection.activeObject = obj;
            }

            //bool any = dataPaths.Any(dp => dp.Contains(obj.name));
            bool anyOut = dataPaths.AnyOut(dp => dp.Contains(obj.name), out var p);

            GUI.backgroundColor = anyOut ? new Color(0.7f, 0.7f, 0.7f) : Color.blue; 

            if (GUILayout.Button($"Create Data For {obj.name}",
            GUILayout.Width(200), 
            GUILayout.Height(50),
            GUILayout.ExpandWidth(false)))
            {
                //todo create food data asset for it in p
                if(anyOut == false)
                {
                    FoodData data = ScriptableObject.CreateInstance(typeof(FoodData)) as FoodData;
                    data.name = $"{obj.name} Data";
                    string dataPath = $"{_dataPath}/{obj.name}.asset";
                    AssetDatabase.CreateAsset(data, dataPath);
                    AssetDatabase.SaveAssets();
                }
            }

            GUI.backgroundColor = originalColor;
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    public override void InstallBindings()
    {
        // TODO
    }

    private List<string> GetPathsInPath(string filter, string actualPath)
    {
        List<string> result = new();

        string[] guids = AssetDatabase.FindAssets(filter, new[] {actualPath});
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            result.Add(path);
        }
        return result;
    }
}