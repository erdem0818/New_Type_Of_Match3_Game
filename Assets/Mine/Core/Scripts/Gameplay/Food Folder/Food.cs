using DG.Tweening;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using Mine.Core.Scripts.Gameplay.Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Food_Folder
{
    public class FoodModel
    {
        //todo remove setters ?
        public ReactiveProperty<FoodData> Data { get; } = new();
        public BoolReactiveProperty IsSelected { get; } = new();
        public BoolReactiveProperty IsPlaced { get; } = new();
        public BoolReactiveProperty IsSliding { get; } = new();
        public BoolReactiveProperty MarkedForMatch { get; } = new();
        
        public Sequence Sequence { get; set; }
        public Tween SlideTween { get; set; }
    }

    public class FoodPresenter
    {
        private readonly SignalBus _signalBus;
        private readonly FoodModel _foodModel;
        private readonly FoodView _foodView;

        public FoodPresenter(SignalBus signalBus, FoodModel foodModel, FoodView foodView)
        {
            _signalBus = signalBus;
            _foodModel = foodModel;
            _foodView = foodView;
        }

        public void InitView()
        {
            _foodView.Food.gameObject.OnMouseUpAsButtonAsObservable().Subscribe(_ => 
            {
                OnClickedFood();
            }).AddTo(_foodView.Food.gameObject);
        }

        private void OnClickedFood()
        {
            if(_foodModel.IsSelected.Value) return;
            
            _signalBus.TryFire(new FoodClickedSignal{Food = _foodView.Food});
        }
    }

    //INFO:: this is kinda over-engineering -> view should be mono behaviour ?
    public class FoodView
    {
        public Food Food { get; }

        public FoodView(Food food)
        {
            Food = food;
        }
        
        public void SetPhysics(bool active)
        {
            Rigidbody rb = Food.GetComponent<Rigidbody>();

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = !active;

            Food.GetComponent<Collider>().isTrigger = !active;
        }
    }
    
    //INFO this is like installer - scope(Vcontainer)
    public class Food : MonoBehaviour
    {
        [Inject] private readonly SignalBus _signalBus;

        [SerializeField] private FoodData data;

        private FoodModel _model;
        private FoodPresenter _presenter;
        private FoodView _foodView;

        public FoodData Data => _model.Data.Value;
        public bool IsSelected
        {
            get => _model.IsSelected.Value;
            set => _model.IsSelected.Value = value;
        }

        public bool IsPlaced
        {
            get => _model.IsPlaced.Value;
            set => _model.IsPlaced.Value = value;
        }

        public bool IsSliding
        {
            get => _model.IsSliding.Value;
            set => _model.IsSliding.Value = value;
        }

        public bool MarkedForMatch
        {
            get => _model.MarkedForMatch.Value;
            set => _model.MarkedForMatch.Value = value;
        }

        public Sequence Sequence
        {
            get => _model.Sequence;
            set => _model.Sequence = value;
        }

        public Tween SlideTween
        {
            get => _model.SlideTween;
            set => _model.SlideTween = value;
        }
        
        private void Awake()
        {
            Install();
        }

        private void Install() 
        {
            _model = new FoodModel
            {
                Data = {Value = data}
            };
            _foodView = new FoodView(this);
            _presenter = new FoodPresenter(_signalBus, _model, _foodView);
            _presenter.InitView();
        }

        public void SetPhysics(bool active)
        {
            _foodView.SetPhysics(active);
        }
    }
}