using System;
using System.Collections.Generic;
using DG.Tweening;
using Sources.Farmer.Actions;
using Sources.Interfaces;
using Sources.Signals;
using UnityEngine;
using Zenject;

namespace Sources.DataHolders
{
    public class Plant : FieldSelectableObject, IHarvestable
    {
        public event Action OnPlantGrownEvent;
        
        [SerializeField] private Sprite harvestActionSprite;
        [SerializeField] private Sprite growSprite;
        [SerializeField] private GameObject plantModel;

        public double PlantTime { get; private set; }
        public bool Grew { get; protected set; }

        public PlantData Data { get; protected set; }
        protected float _timePassed;
        protected GameTimer _timer;

        private void Awake()
        {
            PlantTime = _timer.GetCurrentTime;

            // dirty hack C:
            Data = objectData as PlantData;
        }

        [Inject]
        private void Initialize(GameTimer timer)
        {
            _timer = timer;
            _timer.OnTickedEvent += OnTimerTickedEventHandler;
        }

        public override List<BaseAction> GetActionsOnSelected()
        {
            if (!Grew) return new List<BaseAction>();

            if (selectedActions.Count == 0 && Data.harvestable)
            {
                selectedActions.Add(new HarvestAction(_farmerActionsController, ParentCell, harvestActionSprite,
                    Harvest));
            }

            return selectedActions;
        }

        public void Harvest()
        {
            // assumption that only plants are harvestable
            _signalBus.Fire(new HarvestedSignal(this));

            ParentCell.Clear();
        }

        protected virtual void OnTimerTickedEventHandler(float timePassed)
        {
            _timePassed += timePassed;

            if (_timePassed >= Data.growthTime)
            {
                _signalBus.Fire(new PlantGrownSignal(this));
                OnPlantGrownEvent?.Invoke();
                
                Grew = true;
                // set model on ground
                plantModel.transform.DOMove(ParentCell.CellPosition, 1);

                _timer.OnTickedEvent -= OnTimerTickedEventHandler;
            }
        }

        private void OnDestroy()
        {
            _timer.OnTickedEvent -= OnTimerTickedEventHandler;
        }
    }
}