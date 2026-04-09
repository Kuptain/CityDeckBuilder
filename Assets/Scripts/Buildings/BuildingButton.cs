using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : UIBase
{
    public static event System.Action<BuildingData> OnPressedBuildingUI;

    [SerializeField] private Image icon;
    [SerializeField] private UI_HoverTooltip.Pivot pivot;
    private BuildingData buildingData;

    public void ChangeIcon(Sprite sprite)
    {
        icon.sprite = sprite;
    }

    public void ChangeBuildingData(BuildingData data)
    {
        buildingData = data;
        ChangeIcon(buildingData.uiIcon);
    }

    public void ChangeBuildingID(BuildingData building)
    {
        OnPressedBuildingUI?.Invoke(building); // Calls all listeners
    }

    protected override void HandlePointerEnter(PointerEventData eventData)
    {
        if (ResourceManager.instance == null) return;

        UI_HoverTooltip.Instance.ShowTooltip(buildingData.buildingName, buildingData.buildingDescription, buildingData.GetBaseCost(), transform.position, pivot, GetInstanceID());
    }

    protected override void HandlePointerExit(PointerEventData eventData)
    {
        UI_HoverTooltip.Instance.TryHideTooltip(GetInstanceID());
    }

    protected override void HandlePointerDown(PointerEventData eventData)
    {
        ChangeBuildingID(buildingData);
        UI_HoverTooltip.Instance.TryHideTooltip(GetInstanceID());

    }
}
