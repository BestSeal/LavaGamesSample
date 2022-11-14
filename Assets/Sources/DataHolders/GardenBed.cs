using System.Collections.Generic;
using System.Linq;
using Sources.Farmer.Actions;

namespace Sources.DataHolders
{
    /// <summary>
    /// Garden bed for planting plants
    /// </summary>
    public class GardenBed : FieldSelectableObject
    {
        public override List<BaseAction> GetActionsOnSelected()
        {
            if (PlacedObject != null)
            {
                return PlacedObject.GetActionsOnSelected();
            }

            if (selectedActions.Count == 0)
            {
                selectedActions.AddRange(objectData.possibleObjectsToPlaceOn
                    .Select(x => new PlaceAction(_farmerActionsController, x, ParentCell, x.ObjectData.guiImage)));
            }

            return selectedActions;
        }
    }
}