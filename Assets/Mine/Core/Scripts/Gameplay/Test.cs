using Assets.Mine.Core.Scripts.Framework.Extensions;
using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using System.Text;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public class Test : MonoBehaviour
    {
        [Inject] private Platform _platform;

        private void OnGUI()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0;i < _platform.Foods.Length; i++)
            {
                FoodView temp = _platform.Foods[i];
                if (temp == null)
                    builder.Append("-1,");
                else
                    builder.Append($"{temp.Data.foodID},".ToColor(Color.yellow));
            }
            GUI.Label(new Rect(10, 10, 200, 50),
                    $"{builder}".ToColor(Color.white), 
                    new GUIStyle() { fontSize = 65,
                    richText = true,});
        }
    }
}