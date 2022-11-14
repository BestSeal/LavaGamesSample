using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sources.GUI
{
    public class GUIResourcesCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text counter;
        [SerializeField] private Image counterIcon;

        public GUIResourcesCounter Initialize(Sprite icon)
        {
            counterIcon.sprite = icon;

            return this;
        }

        public GUIResourcesCounter UpdateCounter(float counterData)
        {
            counter.SetText(counterData.ToString(CultureInfo.InvariantCulture));
            
            return this;
        }
    }
}