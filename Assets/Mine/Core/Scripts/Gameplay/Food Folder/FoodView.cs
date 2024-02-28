using Assets.Mine.Core.Scripts.Gameplay.Signals;
using DG.Tweening;
using UniRx.Triggers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Mine.Core.Scripts.Gameplay.FoodFolder
{
    public class FoodMovement
    {
        private readonly Transform _transform;
        private readonly AnimationCurve _ease;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;

        private float _counter = 0f;
        private bool _start = false;
        
        public FoodMovement(Transform transform, AnimationCurve curve)
        {
            _transform = transform;
            _ease = curve;
        }
        
        public void FixedTick()
        {
            if (IsArrived() || _start == false)
                return;

            _counter += Time.deltaTime;
            float evaluated = _ease.Evaluate(_counter / _duration);
            Vector3 lerp = Vector3.Lerp(_startPosition, _targetPosition, evaluated);
            _transform.position = lerp;
        }

        public void SetParameters(Vector3 target, float duration)
        {
            _start = true;
            _startPosition = _transform.position;
            _counter = 0f;
            _targetPosition = target;
            _duration = duration;
        }

        private bool IsArrived()
        {
            return Vector3.Distance(_transform.position, _targetPosition) <= 0.01f;
        }
    }
    
    public class FoodView : MonoBehaviour
    {
        [Inject] private readonly SignalBus _signalBus;

        [SerializeField] private FoodData data;
        public FoodData Data => data;
        private FoodMovement _foodMovement;
        
        public bool IsSelected { get; set; } = false;
        public bool IsPlaced { get; set; } = false;
        public bool IsSliding { get; set; } = false;
        public bool MarkedForMatch { get; set; } = false;

        public Sequence Sequence { get; set; }
        public Tween SlideTween { get; set; }

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
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = !active;

            GetComponent<Collider>().isTrigger = !active;
        }
    }
}
