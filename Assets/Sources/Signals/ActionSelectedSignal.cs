using Sources.Farmer.Actions;

namespace Sources.Signals
{
    public class ActionSelectedSignal
    {
        public BaseAction Action { get; }

        public ActionSelectedSignal(BaseAction action)
        {
            Action = action;
        }
    }
}