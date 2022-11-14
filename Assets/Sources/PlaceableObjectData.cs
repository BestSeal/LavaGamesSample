using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sources.DataHolders;
using UnityEngine;

namespace Sources
{
    [CreateAssetMenu(fileName = nameof(PlaceableObjectData), menuName = "LavaSO/PlaceableObjectData")]
    public class PlaceableObjectData : SerializedScriptableObject
    {
        /// <summary>
        /// Plant image
        /// </summary>
        [Header("Plant image")] public Sprite guiImage;

        /// <summary>
        /// For localization purpose it should be a localization constant
        /// </summary>
        [Header("For localization purpose it should be a localization constant")]
        public string nameConstant;

        /// <summary>
        /// List of possible objects to place on
        /// </summary>
        [Header("List of possible objects to place on")]
        public List<FieldSelectableObject> possibleObjectsToPlaceOn;
    }
}