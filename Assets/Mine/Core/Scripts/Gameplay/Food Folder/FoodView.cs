using Assets.Mine.Core.Scripts.Gameplay.FoodFolder;
using DG.Tweening;
using Mine.Core.Scripts.Gameplay.Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Mine.Core.Scripts.Gameplay.Food_Folder
{
    public class FoodModel
    {
        //reactive properties maybe, view responds these R.properties
        // public FoodData Data { get; set; }
        // public bool IsSelected { get; set; }
        // public bool IsPlaced { get; set; }
        // public bool IsSliding { get; set; }
        // public bool MarkedForMatch { get; set; }
        //
        // public Sequence Sequence { get; set; }
        // public Tween SlideTween { get; set; }

        //todo remove setters ?
        public ReactiveProperty<FoodData> Data { get; set; } = new();
        public BoolReactiveProperty IsSelected { get; set; } = new();
        public BoolReactiveProperty IsPlaced { get; set; } = new();
        public BoolReactiveProperty IsSliding { get; set; } = new();
        public BoolReactiveProperty MarkedForMatch { get; set; } = new();
        
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
            _foodView.gameObject.OnMouseUpAsButtonAsObservable().Subscribe(_ => 
            {
                OnClickedFood();
            }).AddTo(_foodView.gameObject);
        }

        private void OnClickedFood()
        {
            if(_foodModel.IsSelected.Value) return;
            
            _signalBus.TryFire(new FoodClickedSignal{Food = _foodView});
        }
    }

    public class FoodViewRename
    {
        
    }
    
    //Food installer actually
    public class FoodView : MonoBehaviour
    {
        [Inject] private readonly SignalBus _signalBus;

        [SerializeField] private FoodData data;

        private FoodModel _model;
        private FoodPresenter _presenter;

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

        //todo food mvrp
        private void Awake() 
        {
            // gameObject.OnMouseUpAsButtonAsObservable().Subscribe(_ => 
            // {
            //     OnClickedFood();
            // }).AddTo(gameObject);

            _model = new FoodModel
            {
                Data =
                {
                    Value = data
                },
            };
            _presenter = new FoodPresenter(_signalBus, _model, this);
            _presenter.InitView();
        }

        // private void OnClickedFood()
        // {
        //     if(IsSelected) return;
        //
        //     _signalBus.TryFire(new FoodClickedSignal{Food = this});
        // }

        //todo move in to view or presenter
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