using Sources.DataHolders;

namespace Sources.Signals
{
    public class HarvestedSignal
    {
        public Plant Plant { get; }

        public HarvestedSignal(Plant plant)
        {
            Plant = plant;
        }
    }
}