using System;
using Sources.DataHolders;
using Sources.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Sources
{
    public class FieldCell : IFillable<FieldSelectableObject>
    {
        public event Action<FieldSelectableObject, FieldCell> ObjectClickedEvent;
        public event Action<FieldCell> EmptyCellClickedEvent;

        public Vector3 CellPosition { get; private set; }
        public FieldSelectableObject PlacedItem { get; private set; }
        public bool IsFilled => PlacedItem != null;

        private FarmingFieldView _field;
        
        /// <summary>
        /// Is is better to implement factories with pooling, but for simplicity container is used here
        /// </summary>
        protected DiContainer _container;

        public FieldCell(Vector3 cellPosition, FarmingFieldView field, DiContainer container)
        {
            CellPosition = cellPosition;
            _field = field;
            _container = container;
        }

        public void Fill(FieldSelectableObject placeable)
        {
            if (IsFilled)
            {
                PlacedItem.Fill(PlacedItem = _container.InstantiatePrefab(placeable).GetComponent<FieldSelectableObject>()
                    .Place(_field.gameObject.transform, this));
                PlacedItem.ObjectClickedEvent += OnPlaceableObjectClickedEvent;

                return;
            }
            
            PlacedItem = _container.InstantiatePrefab(placeable).GetComponent<FieldSelectableObject>()
                .Place(_field.gameObject.transform, this);
            PlacedItem.ObjectClickedEvent += OnPlaceableObjectClickedEvent;
        }

        private void OnPlaceableObjectClickedEvent(FieldSelectableObject data) => ObjectClickedEvent?.Invoke(data, this);

        public void OnCellClicked(PointerEventData eventData)
        {
            if (IsFilled)
            {
                PlacedItem.OnPointerClick(eventData);
            }else
            {
                EmptyCellClickedEvent?.Invoke(this);
            }
        }

        public void Clear()
        {
            PlacedItem.Clear();
            PlacedItem = null;
        }
    }
}