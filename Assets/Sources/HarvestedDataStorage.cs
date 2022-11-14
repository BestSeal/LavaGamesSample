using System;
using System.Collections.Generic;
using Sources.Signals;
using Zenject;

namespace Sources
{
    public class HarvestedDataStorage
    {
        public static string ExpConstant = "ExpConstant";
        
        public event Action<int> ExpValueChangedEvent; 
        public event Action<(string, int)> HarvestedItemsChangedEvent; 

        /// <summary>
        /// Earned experience
        /// </summary>
        public int ExpEarned { get; private set; }

        /// <summary>
        /// string - items localization strings, int - harvested count
        /// </summary>
        public Dictionary<string, int> HarvestedItems { get; private set; }

        private SignalBus _signalBus;

        public HarvestedDataStorage(SignalBus signalBus)
        {
            _signalBus = signalBus;
            HarvestedItems = new Dictionary<string, int>(10);
            
            _signalBus.Subscribe<HarvestedSignal>(OnHarvestedSignalHandler);
            _signalBus.Subscribe<PlantGrownSignal>(OnPlantGrownSignalHandler);
        }

        private void OnPlantGrownSignalHandler(PlantGrownSignal signal)
        {
            if (signal?.GrownPlant != null)
            {
                ExpEarned += signal.GrownPlant.Data.harvestExpReward;
                ExpValueChangedEvent?.Invoke(ExpEarned);
            }
        }

        private void OnHarvestedSignalHandler(HarvestedSignal signal)
        {
            if (signal?.Plant != null)
            {
                var constantStr = signal.Plant.ObjectData.nameConstant;
                if (HarvestedItems.TryGetValue(constantStr, out int count))
                {
                    HarvestedItems[constantStr] = count + 1;
                    HarvestedItemsChangedEvent?.Invoke((constantStr, count));
                }
                else
                {
                    HarvestedItems[constantStr] = 1;
                    HarvestedItemsChangedEvent?.Invoke((constantStr, 1));
                }
            }
        }
    }
}