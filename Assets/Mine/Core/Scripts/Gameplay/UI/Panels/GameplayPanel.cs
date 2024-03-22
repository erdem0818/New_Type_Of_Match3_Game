using Mine.Core.Scripts.Framework.Extensions_Folder;
using Mine.Core.Scripts.Framework.UI.Panel_Folder;
using NaughtyAttributes;
using TMPro;
using UniRx;
using UnityEngine;

namespace Mine.Core.Scripts.Gameplay.UI.Panels
{
    //INFO:: Vcontainer - uniRx - MV(R)P Example
    [System.Serializable]
    public class GameplayPanelView
    {
        [field: Header("View")] 
        [field: HorizontalLine(2, EColor.Pink)]
        [field: SerializeField] public TMP_Text TimerText { get; private set; }
        [field: SerializeField] public TMP_Text LevelText { get; private set; }
        [field: SerializeField] public Transform GoalContainer { get; private set; }
        [field: SerializeField] public Transform PowerUpContainer { get; private set; }
        [field: SerializeField] public TMP_Text MovesText { get; private set; }
    }

    public class GameplayPanelModel
    {
        public IntReactiveProperty Timer { get; } = new();
        public IntReactiveProperty Level { get; } = new();
        public IntReactiveProperty Moves { get; } = new();
    }

    public class GameplayPanelPresenter
    {
        private readonly GameplayPanelView _view;
        private readonly GameplayPanelModel _model;
        public GameObject DestroyObject { get; set; }

        public GameplayPanelPresenter(GameplayPanelView view, GameplayPanelModel model)
        {
            _view = view;
            _model = model;
        }

        public void Bind()
        {
            _model.Timer.SubscribeIntToTextPro(_view.TimerText).AddTo(DestroyObject);
            _model.Level.SubscribeIntToTextPro(_view.LevelText).AddTo(DestroyObject);
            _model.Moves.SubscribeIntToTextPro(_view.MovesText).AddTo(DestroyObject);
        }
    }

    public class GameplayPanel : DefaultPanel
    {
        [SerializeField] private GameplayPanelView view;

        private GameplayPanelPresenter _presenter;
        private GameplayPanelModel _model;
        
        protected override void Awake()
        {
            base.Awake();

            _model = new GameplayPanelModel();
            _presenter = new GameplayPanelPresenter(view, _model)
            {
                DestroyObject = gameObject
            };
            _presenter.Bind();
        }

        [Button]
        public void IncreaseTimer()
        {
            _model.Timer.Value++;
        }
        
        [Button]
        public void IncreaseLevel()
        {
            _model.Level.Value++;
        }
    }
}
