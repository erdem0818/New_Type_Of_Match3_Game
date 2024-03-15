using UnityEngine;

namespace Mine.Core.Scripts.Gameplay
{
    public static class Defines
    {
        public const int AllFull = -111;

        public const string LevelSaveKey = "LevelSave";

        public const string PrefabFilter = "t:Prefab";
        public const string SoFilter     = "t:ScriptableObject";
        public const string SpriteFilter  = "t:Sprite";

        public static readonly Color Banana    = new(0.912f, 0.914f, 0.920f, 1.0f);
        public static readonly Color Blood     = new(0.634f, 0.532f, 0.111f, 1.0f);
        public static readonly Color Carrot    = new(0.713f, 0.170f, 0.026f, 1.0f);
        public static readonly Color Chocolate = new(0.162f, 0.091f, 0.060f, 1.0f);
        public static readonly Color Sand      = new(0.440f, 0.386f, 0.231f, 1.0f);
        public static readonly Color Lemon     = new(0.718f, 0.483f, 0.000f, 1.0f);
    }
}

