using Cysharp.Threading.Tasks;
using Mine.Core.Scripts.Framework.UI.Panel_Folder.Popup_Folder;
using TMPro;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.UI.Panels
{
    public class WinPopup : DefaultPopup
    {
        [Header("Win Popup Related")]
        [SerializeField] private TMP_Text scoreText;

        protected override async UniTask WhenPreAppearAsync()
        {
            //Info We can imagine that there is a service where we get the score.
            scoreText.text = $"{1000}";
            await base.WhenPreAppearAsync();
        }
    }
}
