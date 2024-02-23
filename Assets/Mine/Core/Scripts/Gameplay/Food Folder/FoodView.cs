using Assets.Mine.Core.Scripts.Gameplay.Signals;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Assets.Mine.Core.Scripts.Gameplay.FoodFolder
{
    public class FoodView : MonoBehaviour
    {
        [Inject] private readonly SignalBus _signalBus;

        [SerializeField] private FoodData data;

        public bool IsSelected { get; set; } = false;

        private void Awake() 
        {
            gameObject.OnMouseUpAsButtonAsObservable().Subscribe(_ => 
            {
                OnClickedFood();
            }).AddTo(gameObject);
        }

        private void OnClickedFood()
        {
            if(IsSelected) return;

            _signalBus.TryFire(new FoodClickedSignal{Food = this});
        }

        public void SetPhysics(bool active)
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.isKinematic = !active;
        }
    }
}
