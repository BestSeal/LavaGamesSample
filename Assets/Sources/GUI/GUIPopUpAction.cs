using System;
using Sources.Farmer.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sources.GUI
{
    public class GUIPopUpAction : MonoBehaviour, IPointerClickHandler
    {
        public event Action<BaseAction> ActionClickedEvent;
        
        [SerializeField] private Image actionImage;
        
        public BaseAction Action { get; private set; }

        public void SetupData(BaseAction action)
        {
            Action = action;
            actionImage.sprite = action.ActionSprite;
        }

        public void OnPointerClick(PointerEventData eventData) => ActionClickedEvent?.Invoke(Action);
    }
}