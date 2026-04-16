using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IconTooltip : UIBase
{
    public string tooltipName;
    public string tooltiDescription;
    public UI_HoverTooltip.Pivot pivot;

    protected override void HandlePointerEnter(PointerEventData eventData)
    {
        if (ResourceManager.instance == null) return;

        UI_HoverTooltip.Instance.ShowTooltip(tooltipName, tooltiDescription, new List<ResourceCost>(), transform.position, pivot, GetInstanceID());
    }

    protected override void HandlePointerExit(PointerEventData eventData)
    {
        UI_HoverTooltip.Instance.TryHideTooltip(GetInstanceID());
    }
}
