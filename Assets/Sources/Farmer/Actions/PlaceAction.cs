using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sources.DataHolders;
using UnityEngine;

namespace Sources.Farmer.Actions
{
    public class PlaceAction : BaseAction
    {
        private FarmerActionsController _actionsController;
        private FieldSelectableObject _placeObject;
        private FieldCell _placeCell;

        public PlaceAction(FarmerActionsController actionsController,
            FieldSelectableObject placeObject, FieldCell placeCell, Sprite actionSprite, Action finishCallback = null) :
            base(actionSprite, finishCallback)
        {
            _actionsController = actionsController;
            _placeObject = placeObject;
            _placeCell = placeCell;
        }

        public override async UniTask PerformAction(CancellationToken cancellationToken)
        {
            await _actionsController.SetMoveDestination(_placeCell.CellPosition, cancellationToken);
            
            await _actionsController.PlantAction(cancellationToken);

            _placeCell.Fill(_placeObject);
            _finishCallback?.Invoke();
        }
    }
}