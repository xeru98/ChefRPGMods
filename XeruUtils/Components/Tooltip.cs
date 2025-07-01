using UnityEngine;
using UnityEngine.EventSystems;

namespace XeruUtils.Components
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject modOptionWidget;
        public string tooltipText;


        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}