using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mine.Core.Scripts.Framework.UI.Panel_Folder.Panel_Extensions
{
    public class FocusExtension : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private BasePanel panel;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click");
            if (panel != null) panel.HideAsync().Forget();
        }
    }
}
