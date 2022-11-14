using Sources.DataHolders;

namespace Sources.Signals
{
    public class PlantGrownSignal
    {
        public Plant GrownPlant { get; }

        public PlantGrownSignal(Plant grownPlant)
        {
            GrownPlant = grownPlant;
        }
    }
}