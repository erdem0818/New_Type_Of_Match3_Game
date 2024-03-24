using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.Game;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Popup_Folder;
using Mine.Core.Scripts.Gameplay.UI.Panels;
using UniRx;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay
{
    public class Test : MonoBehaviour
    {
        [Inject] private Platform _platform;
        [Inject] private IPanelService _panelService;
        [Inject] private GameHandler _gameHandler;

        [SerializeField] private bool debug = false;
        [SerializeField] private float yPos;
        
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
                
                if (Input.GetKeyDown(KeyCode.P))
                {
                    var panel = FindFirstObjectByType<DefaultPopup>(FindObjectsInactive.Include);
                    panel.gameObject.SetActive(true);
                    panel.ShowAsync().Forget();
                }
                
                if (Input.GetKeyDown(KeyCode.L))
                {
                    //var panel = FindFirstObjectByType<LoadingPanel>(FindObjectsInactive.Include);
                    CreateLoadingPanel().Forget();
                }
            }).AddTo(gameObject);
        }

        private async UniTask CreateLoadingPanel()
        {
            var panel = await _panelService.Create<WinPopup>();
            await panel.ShowAsync(0f, destroyCancellationToken);
            await UniTask.Delay(5000, cancellationToken: destroyCancellationToken);
            if (panel != null)
                await panel.HideAsync();
        }

        private void OnGUI()
        {
            if (debug == false) return;
            if(_platform?.Parts == null) return;

            string appStateText = $"App State: {_gameHandler.AppState}";
            string gmpStateText = $"GameplayState: {_gameHandler.GameplayState}";
            
            GUI.Label(new Rect(40, yPos, 250, 50),
                appStateText,
                new GUIStyle()
                {
                    fontSize = 45,
                    richText = true
                });
            
            GUI.Label(new Rect(40, yPos + 40, 250, 50),
                gmpStateText,
                new GUIStyle()
                {
                    fontSize = 45,
                    richText = true
                });

            /*StringBuilder builder = new StringBuilder();
            builder.Append('[');
            foreach (var part in _platform.Parts)
            {
                Color color = part.IsLocked ? new Color(0.35f, 0.35f, 0.35f, 1.0f) : Color.yellow;
                builder.Append(part.CurrentFood == null ? "-1," : $"{part.CurrentFood.Data.foodID},".ToColor(color));
            }
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
                    richText = true,});*/
        }
    }
}