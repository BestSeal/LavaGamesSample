using System;
using System.Collections.Generic;
using Sources.Farmer;
using Sources.Farmer.Actions;
using Sources.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Sources.DataHolders
{
    public abstract class FieldSelectableObject : MonoBehaviour, IPointerClickHandler, IFillable<FieldSelectableObject>
    {
        public event Action<FieldSelectableObject> ObjectClickedEvent;

        public PlaceableObjectData ObjectData => objectData;

        public bool IsFilled => PlacedObject != null;

        public FieldSelectableObject PlacedObject { get; protected set; }

        public FieldCell ParentCell { get; set; } = null;

        public abstract List<BaseAction> GetActionsOnSelected();

        [SerializeField] protected PlaceableObjectData objectData;
        protected SignalBus _signalBus;

        protected FarmerActionsController _farmerActionsController;
        protected List<BaseAction> selectedActions = new List<BaseAction>(1);

        [Inject]
        protected virtual void Initialize(SignalBus signalBus, FarmerActionsController farmerActionsController)
        {
            _signalBus = signalBus;
            _farmerActionsController = farmerActionsController;
        }

        public void OnPointerClick(PointerEventData _) => ObjectClickedEvent?.Invoke(this);

        public FieldSelectableObject Place(Transform parent, FieldCell cell)
        {
            transform.SetParent(parent);
            transform.position = cell.CellPosition;
            ParentCell = cell;

            return this;
        }

        public void Fill(FieldSelectableObject placeable)
        {
            PlacedObject = placeable;
        }

        public virtual void Clear()
        {
            if (PlacedObject != null)
            {
                PlacedObject.Clear();
            }

            // here must be used pooling despawn instead of destroy
            Destroy(gameObject);
        }
    }
}