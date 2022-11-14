using System;
using System.Text;
using Sources.DataHolders;
using TMPro;
using UnityEngine;
using Zenject;

namespace Sources.GUI
{
    [RequireComponent(typeof(Canvas))]
    public class GUIGrowTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timer;
        [SerializeField] private Plant plant;

        private Canvas _canvas;
        private GameTimer _gameTimer;
        private MainGameCamera _mainGameCamera;
        private float _countdownTime;
        private StringBuilder _stringBuilder = new StringBuilder();
        
        private const string _timerFormatS = "{0}s";
        private const string _timerFormatMS = "{0}m:{1}s";
        private const string _timerFormatHMS = "{0}h:{1}m:{2}s";

        [Inject]
        private void Initialize(GameTimer gameTimer, MainGameCamera mainGameCamera)
        {
            _gameTimer = gameTimer;
            _mainGameCamera = mainGameCamera;
        }

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.worldCamera = _mainGameCamera.MainCamera;
        }

        private void Start()
        {
            plant.OnPlantGrownEvent += OnPlantGrownEventHandler;
            _gameTimer.OnTickedEvent += OnTickedEventHandler;
            _countdownTime = plant.Data.growthTime;
        }

        private void OnTickedEventHandler(float deltaTime)
        {
            _countdownTime -= deltaTime;
            var timeSpan = TimeSpan.FromSeconds(_countdownTime);

            _stringBuilder.Clear();
            if (timeSpan.Hours != 0)
            {
                _stringBuilder.AppendFormat(_timerFormatHMS, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }
            else if(timeSpan.Minutes != 0)
            {
                _stringBuilder.AppendFormat(_timerFormatMS, timeSpan.Minutes, timeSpan.Seconds);
            }
            else
            {
                _stringBuilder.AppendFormat(_timerFormatS, timeSpan.Seconds);
            }
            
            timer.SetText(_stringBuilder);
        }

        private void OnPlantGrownEventHandler() => gameObject.SetActive(false);

        private void OnDestroy()
        {
            plant.OnPlantGrownEvent -= OnPlantGrownEventHandler;
        }
    }
}