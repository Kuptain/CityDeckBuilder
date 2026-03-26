using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
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
    public void OnPointerDown(PointerEventData eventData)
    {
        ChangeBuildingID(buildingData);
    }
    public void ChangeBuildingID(BuildingData building)
    {
        OnPressedBuildingUI?.Invoke(building); // Calls all listeners
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ResourceManager.instance == null) return;

        List<string> info = new List<string>
        {
            $"<b>{buildingData.buildingName}</b>", $"",
            buildingData.buildingDescription
        };

        UI_HoverTooltip.Instance.ShowTooltip(info, buildingData.GetBaseCost(), transform.position, pivot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI_HoverTooltip.Instance.HideTooltip();
    }
}
