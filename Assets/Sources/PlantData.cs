using Sources.DataHolders;
using UnityEngine;

namespace Sources
{
    [CreateAssetMenu(fileName = nameof(Plant), menuName = "LavaSO/Plant")]
    public class PlantData : PlaceableObjectData
    {
        /// <summary>
        /// Plant growth time
        /// </summary>
        [Header("Plant growth time")]
        public float growthTime;
        
        /// <summary>
        /// Exp reward after harvesting
        /// </summary>
        [Header("Exp reward after harvesting")]
        public int harvestExpReward;
        
        /// <summary>
        /// Is this plant harvestable
        /// </summary>
        [Header("Is this plant harvestable")]
        public bool harvestable = true;
    }
}