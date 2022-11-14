using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Sources.GUI
{
    public class GUIResourcesHolder : MonoBehaviour
    {
        [SerializeField] private GUIResourcesCounter expCounter;
        [SerializeField] private GUIResourcesCounter counterPrefab;
        [SerializeField] private List<PlaceableObjectData> countersTemplates;
        
        private Dictionary<string, GUIResourcesCounter> _counters = new Dictionary<string, GUIResourcesCounter>();

        private HarvestedDataStorage _harvestedDataStorage;

        [Inject]
        private void Initialize(HarvestedDataStorage harvestedDataStorage)
        {
            _harvestedDataStorage = harvestedDataStorage;
        }

        private void Awake()
        {
            _harvestedDataStorage.ExpValueChangedEvent += OnExpValueChangedEventHandler;
            _harvestedDataStorage.HarvestedItemsChangedEvent += OnHarvestedItemsChangedEventHandler;

            foreach (var countersTemplate in countersTemplates)
            {
                var counter = Instantiate(counterPrefab, transform)
                    .Initialize(countersTemplate.guiImage)
                    .UpdateCounter(0);
                _counters.Add(countersTemplate.nameConstant, counter);
            }
            
            _counters.Add(HarvestedDataStorage.ExpConstant, expCounter.UpdateCounter(0));
        }

        private void OnHarvestedItemsChangedEventHandler((string counterKey, int counterValue) counterUpdateData) 
            => UpdateCounter(counterUpdateData.counterKey, counterUpdateData.counterValue);

        private void OnExpValueChangedEventHandler(int expValue) => UpdateCounter(HarvestedDataStorage.ExpConstant, expValue);

        private void UpdateCounter(string key, int value)
        {
            if (_counters.TryGetValue(key, out var counter))
            {
                counter.UpdateCounter(value);
            }
        }
        
        private void OnDestroy()
        {
            _harvestedDataStorage.ExpValueChangedEvent -= OnExpValueChangedEventHandler;
            _harvestedDataStorage.HarvestedItemsChangedEvent -= OnHarvestedItemsChangedEventHandler;
        }
    }
}