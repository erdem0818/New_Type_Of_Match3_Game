using System.Collections.Generic;
using System.Linq;
using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Framework.Extensions_Folder;
using Assets.Mine.Core.Scripts.Gameplay;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Editor
{
    public class FoodDataCreator : ZenjectEditorWindow
    {
        private const string Path = "Assets/Mine/Core/Prefabs/Food Prefabs";
        private const string DataPath = "Assets/Mine/Core/Scripts/Data/Food Datas";
        private Vector2 _scrollPosition;
        private readonly List<FoodData> _foodDataList = new();
        private readonly List<GameObject> _prefabs = new();

        private Color _originalColor;

        public override void InstallBindings()
        {
            // TODO
        }

        [MenuItem("Window/FoodDataCreator")]
        public static FoodDataCreator GetOrCreateWindow()
        {
            var window = GetWindow<FoodDataCreator>();
            window.titleContent = new GUIContent("FoodDataCreator");
            return window;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            LoadAllFoodData();
            LoadAllPrefabs();
            _originalColor = GUI.backgroundColor;
        }

        public override void OnGUI()
        {
            GUILayout.Label("Prefabs in path: " + Path, EditorStyles.boldLabel);

            if (GUILayout.Button("Refresh List"))
            {
                LoadAllFoodData();
                LoadAllPrefabs();
            } 

            DisplayPrefabs();

            if(GUILayout.Button("Validata All Data", new GUILayoutOption[]
               {
                   GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(35)
               }))
            {
                ValidateAllData();
            }

            if (GUILayout.Button("Give All ID", new GUILayoutOption[]
                {
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(35)
                }))
            {
                GiveAllID();
            }
        }

        private void DisplayPrefabs()
        {
            List<string> paths = PathUtility.GetPathsInPath(Defines.PrefabFilter, Path);
            List<string> dataPaths = PathUtility.GetPathsInPath(Defines.SOFilter, DataPath);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, new GUIStyle() { });

            foreach (var path in paths)
            {
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
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
                        GUILayout.Width(150), 
                        GUILayout.Height(50),
                        GUILayout.ExpandWidth(false)))
                {
                    Selection.activeObject = obj;
                }

                bool anyOut = dataPaths.AnyOut(dp => dp.Contains(obj.name), out var outPath);

                GUI.backgroundColor = anyOut == true ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.1f, 0.6f, 0.85f); 

                if (GUILayout.Button($"Create Data For {obj.name}",
                        GUILayout.Width(150), 
                        GUILayout.Height(50),
                        GUILayout.ExpandWidth(false)))
                {
                    if(anyOut == false)
                    {
                        FoodData data = CreateInstance(typeof(FoodData)) as FoodData;
                        data.name = $"{obj.name} Data";
                        string dataPath = $"{DataPath}/{obj.name}.asset";
                        AssetDatabase.CreateAsset(data, dataPath);
                        AssetDatabase.SaveAssets();

                        Selection.activeObject = data;
                    }
                    else
                    {
                        FoodData data = _foodDataList.FirstOrDefault(food => outPath.Contains(food.name));
                        Selection.activeObject = data;
                    }
                }

                GUI.backgroundColor = _originalColor;

                if (anyOut)
                {
                    FoodData data = AssetDatabase.LoadAssetAtPath<FoodData>(outPath);
                    DisplayPrefab((obj as GameObject)?.GetComponent<FoodView>());
                    DisplayFoodData(data);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void LoadAllFoodData()
        {
            _foodDataList.Clear();
            var foodDataPaths = PathUtility.GetPathsInPath(Defines.SOFilter, DataPath);

            foreach (var data in foodDataPaths.Select(AssetDatabase.LoadAssetAtPath<FoodData>).Where(data => data != null))
            {
                _foodDataList.Add(data);
            }
        }

        private void LoadAllPrefabs()
        {
            _prefabs.Clear();
            var prefabPaths = PathUtility.GetPathsInPath(Defines.PrefabFilter, Path);

            foreach (var go in prefabPaths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(go => go != null))
            {
                _prefabs.Add(go);
            }
        }

        private void DisplayFoodData(FoodData food)
        {
            SerializedObject serializedObject = new SerializedObject(food);
            SerializedProperty idProp = serializedObject.FindProperty("foodID");
            SerializedProperty nameProp = serializedObject.FindProperty("foodName");
            SerializedProperty prefabProp = serializedObject.FindProperty("foodPrefab");

            EditorGUILayout.BeginVertical();

            EditorGUILayout.PropertyField(idProp, new GUIContent("ID"));
            EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
            EditorGUILayout.PropertyField(prefabProp, new GUIContent("foodPrefab"));

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginVertical();
            GUI.backgroundColor = new Color(0.80f, 0.0f, 0.80f);
            if (GUILayout.Button($"Save {food.name}"))
            {
                EditorUtility.SetDirty(food);
                AssetDatabase.SaveAssets();
            }
            GUI.backgroundColor = new Color(0.75f, 0.25f, 0.1f);
            if(GUILayout.Button($"Delete {food.name}"))
            {
                EditorUtility.SetDirty(food);
                var foodDataPaths = PathUtility.GetPathsInPath(Defines.SOFilter, DataPath);
                foodDataPaths.AnyOut<string>(p => p.Contains(food.name), out string path);
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.SaveAssets();
            }
            GUI.backgroundColor = _originalColor;

            EditorGUILayout.EndVertical();
        }

        private void DisplayPrefab(FoodView prefab)
        {
            SerializedObject serializedObject = new(prefab);
            SerializedProperty dataProp = serializedObject.FindProperty("data");

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(dataProp, new GUIContent(), new GUILayoutOption[]
            {
                GUILayout.MinWidth(100), GUILayout.ExpandWidth(false)
            });
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateAllData()
        {
            var allDifferent = _foodDataList.Select(d => d.foodID).ToList().Distinct().Count() == _foodDataList.Count;
            Debug.Log(allDifferent ? "All Different" : "Not All Different");
        }

        private void GiveAllID()
        {
            int idCounter = 0;
            foreach(FoodData data in _foodDataList)
            {
                EditorUtility.SetDirty(data);
                data.foodID = idCounter;
                data.foodName = data.name;
                data.foodPrefab = _prefabs.FirstOrDefault(p => p.name == data.name);
                idCounter++;
            }

            AssetDatabase.SaveAssets();
        }
    }
}