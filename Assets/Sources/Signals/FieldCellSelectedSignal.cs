using System.Collections.Generic;
using Sources.Farmer.Actions;

namespace Sources.Signals
{
    public class FieldCellSelectedSignal
    {
        public List<BaseAction> Actions { get; }
        
        public FieldCell TargetCell { get; }

        public FieldCellSelectedSignal(List<BaseAction> actions, FieldCell targetCell)
        {
            Actions = actions;
            TargetCell = targetCell;
        }
    }
}