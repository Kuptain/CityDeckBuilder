using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipName;
    public string tooltiDescription;
    public UI_HoverTooltip.Pivot pivot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ResourceManager.instance == null) return;

        List<string> info = new List<string>
        {
            $"<b>{tooltipName}</b>", $"",
            tooltiDescription
        };

        UI_HoverTooltip.Instance.ShowTooltip(info, new List<ResourceCost>(), transform.position, pivot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_HoverTooltip.Instance.HideTooltip();
    }
}
