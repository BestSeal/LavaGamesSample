using System.Collections.Generic;
using Sources.Farmer.Actions;
using Sources.Signals;
using UnityEngine;
using Zenject;

namespace Sources.GUI
{
    [RequireComponent(typeof(Canvas))]
    public class GUIPopUpsHolder : MonoBehaviour
    {
        [SerializeField] private GUIPopUpAction guiPopUpActionPrefab;
        [SerializeField] private Transform planeRoot;
        [SerializeField] private Vector3 popUpOffset = new Vector3(0, 3, 0);

        private SignalBus _signalBus;
        private List<GUIPopUpAction> actions = new List<GUIPopUpAction>(10);
        
        [Inject]
        private void Initialize(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Start()
        {
            _signalBus.Subscribe<FieldCellSelectedSignal>(OnFieldCellSelectedSignal);
            planeRoot.gameObject.SetActive(false);
        }

        private void OnFieldCellSelectedSignal(FieldCellSelectedSignal signal)
        {
            if(signal == null) return;

            PopulateActions(signal.Actions);
            
            transform.position = signal.TargetCell.CellPosition + popUpOffset;
        }

        public void PopulateActions(List<BaseAction> actionsData)
        {
            if (actionsData == null || actionsData.Count == 0)
            {
                planeRoot.gameObject.SetActive(false);
                return;
            }
            
            planeRoot.gameObject.SetActive(true);
            
            // deactivate previous actions
            foreach (var action in actions)
            {
                action.gameObject.SetActive(false);
            }

            for (int i = 0; i < actionsData.Count; i++)
            {
                GUIPopUpAction action;
                
                if (i < actions.Count)
                {
                    action = actions[i];
                }
                else
                {
                    action = Instantiate(guiPopUpActionPrefab, planeRoot);
                    actions.Add(action);
                }
                
                action.SetupData(actionsData[i]);
                action.ActionClickedEvent += OnActionClickedEventHandler;
                action.gameObject.SetActive(true);
            }
        }

        private void OnActionClickedEventHandler(BaseAction action)
        {
            planeRoot.gameObject.SetActive(false);
            _signalBus.Fire(new ActionSelectedSignal(action));
        }

        private void OnDestroy()
        {
            foreach (var action in actions)
            {
                action.ActionClickedEvent -= OnActionClickedEventHandler;
            }
            
            _signalBus.TryUnsubscribe<FieldCellSelectedSignal>(OnFieldCellSelectedSignal);
        }
    }
}