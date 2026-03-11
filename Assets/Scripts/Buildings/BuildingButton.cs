using Mono.Cecil;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public static event System.Action<BuildingData> OnPressedBuildingUI;

    [SerializeField] private Image icon;
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
            $"<b>{buildingData.buildingName}</b>",
            buildingData.buildingDescription
            //$"",
            //$"Cost:"
        };

        // // ####### Hover tooltip information, wip #######
        
        List<ResourceCost> resourceTypes = new List<ResourceCost>();
        for (int i = 0; i < buildingData.resourceCosts.Count; i++)
        {
            resourceTypes.Add(buildingData.resourceCosts[i]);
        }

        
        TooltipUI.Instance.ShowTooltip(info, resourceTypes, transform.position);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ####### Hover tooltip information, wip #######
        // #######
        TooltipUI.Instance.HideTooltip(); 
    }
}
