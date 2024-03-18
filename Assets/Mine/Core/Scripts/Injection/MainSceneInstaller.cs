using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Injection
{
    public class MainSceneInstaller : MonoInstaller
    {
        [Header("Main Canvas")] 
        [SerializeField] private Transform mainCanvas;

        public override void InstallBindings()
        {
            //todo audio, haptic etc.
            Container.BindInterfacesAndSelfTo<PanelService>().AsSingle().WithArguments(mainCanvas);
        }
    }
}
