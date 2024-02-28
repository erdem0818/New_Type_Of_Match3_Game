using Assets.Mine.Core.Scripts.Framework.Extensions;
using System.Text;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay
{
    public class Test : MonoBehaviour
    {
        [Inject] private Platform _platform;

        private void Awake()
        {
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _platform.ReorderAllMovement();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    _platform.Reset();
                }
            }).AddTo(gameObject);
        }

        private void OnGUI()
        {
            if(_platform?.Parts == null) return;

            StringBuilder builder = new StringBuilder();
            builder.Append('[');
            foreach (var part in _platform.Parts)
                builder.Append(part.CurrentFood == null ? "-1," : $"{part.CurrentFood.Data.foodID},".ToColor(Color.yellow));
            builder.Append(']');
            GUI.Label(new Rect(10, 10, 200, 50),
                    $"{builder}".ToColor(Color.white), 
                    new GUIStyle { fontSize = 45,
                    richText = true,});

            StringBuilder builder2 = new StringBuilder();
            builder2.Append('[');
            foreach (var part in _platform.Parts)
                builder2.Append(part.IsOccupied == false? "e," : $"o,".ToColor(Color.yellow));
            builder2.Append(']');
            GUI.Label(new Rect(10, 50, 200, 50),
                $"{builder2}".ToColor(Color.white), 
                new GUIStyle { fontSize = 45,
                    richText = true,});
        }
    }
}