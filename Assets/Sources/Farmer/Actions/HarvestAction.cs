using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Sources.Farmer.Actions
{
    public class HarvestAction : BaseAction
    {
        private FarmerActionsController _actionsController;
        private FieldCell _placeCell;

        public HarvestAction(FarmerActionsController actionsController, FieldCell placeCell, Sprite actionSprite,
            Action finishCallback = null) : base(actionSprite, finishCallback)
        {
            _actionsController = actionsController;
            _placeCell = placeCell;
            _finishCallback = finishCallback;
        }

        public override async UniTask PerformAction(CancellationToken cancellationToken)
        {
            await _actionsController.SetMoveDestination(_placeCell.CellPosition, cancellationToken);

            await _actionsController.PlantAction(cancellationToken);

            _finishCallback?.Invoke();
        }
    }
}