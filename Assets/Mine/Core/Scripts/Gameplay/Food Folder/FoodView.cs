using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.FoodFolder
{
    //todo like VObject<> ??
    //todo click presenter ?
    public class FoodView : MonoBehaviour
    {
        [Inject] private SignalBus _signalBus;
        
        [SerializeField] private FoodData data;

        private void Awake() 
        {
            gameObject.OnMouseUpAsButtonAsObservable().Subscribe(_ => 
            {
                OnClickedFood();
            }).AddTo(gameObject);
        }

        private void OnClickedFood()
        {
            _signalBus.TryFire(new FoodClickedSignal{Food = this});
            Debug.Log("up as button");
        }
    }
}
