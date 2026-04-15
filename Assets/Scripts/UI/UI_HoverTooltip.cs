using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;

public class UI_HoverTooltip : UIBase
{
    public static UI_HoverTooltip Instance;
    public enum Pivot
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject resourceIconSlotContainer;
    [SerializeField] private GameObject buildingSettings;
    [SerializeField] private GameObject recipeButtonContainer;
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private GameObject UpgradeButton;
    [SerializeField] private GameObject CancleButton;
    [SerializeField] private float padding = 150f;
    [SerializeField] private float paddingResouceIcons = 50f;
    [SerializeField] private Vector2 pivotTopLeft;
    [SerializeField] private Vector2 pivotBottomLeft;
    [SerializeField] private Vector2 pivotTopRight;
    [SerializeField] private Vector2 pivotBottomRight;
    [SerializeField] private ResourceDatabase resourceDatabase;

    private List<string> _currentLines;
    private List<ResourceCost> _currentResourceCosts;
    private List<GameObject> recipesButtons = new List<GameObject>();
    private RectTransform _canvasRectTransform;
    private ResourceSlotUI[] _iconSlots;
    private int _tooltipActiveState = 0;
    private int tooltipID;
    BuildingObject selectedBuilding;

    public enum TooltipTypes { Building, Resource }

    private void Awake()
    {
        Instance = this;
        _canvasRectTransform = transform.parent.GetComponent<RectTransform>();
        HideTooltip();

        _iconSlots = resourceIconSlotContainer.GetComponentsInChildren<ResourceSlotUI>(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }

    }

    void Update()
    {

        switch (_tooltipActiveState)
        {
            case 0: // Nothing
                break;

            case 1: // Update Tooltip
                break;

            case 2: // Update Tooltip
                HideTooltip();
                break;
        }
    }

    public Vector2 PivotToVector2(Pivot dir)
    {
        return dir switch
        {
            Pivot.TopLeft => pivotTopLeft,
            Pivot.TopRight => pivotTopRight,
            Pivot.BottomLeft => pivotBottomLeft,
            Pivot.BottomRight => pivotBottomRight,
            _ => Vector2.zero
        };
    }

    public void SelectBuilding(BuildingObject building)
    {
        BuildingObject.OpenEffect effect;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(building.transform.position);
        ShowTooltip(building.data.buildingName, building.data.buildingDescription, building.data.GetBaseCost(), screenPos, UI_HoverTooltip.Pivot.TopLeft, building.GetInstanceID(), true);
        selectedBuilding = building;
        UpgradeButton.SetActive(building.HasUpgrade());
        CancleButton.SetActive(building.HasOpenEffect(out effect));
        List<CraftRecipe> recipes;

        if (effect.type != BuildingObject.OpenEffect.Type.construction)
        {
            for (int i = 0; i < recipesButtons.Count; i++)
            {
                recipesButtons[i].SetActive(false);
            }
            if (building.TryToGetCraftingRecipes(out recipes))
            {
                for (int i = 0; i < recipes.Count; i++)
                {

                    CreateRecipeButton(recipes[i], i);

                }
            }
        }
    }

    void CreateRecipeButton(CraftRecipe recipe, int index)
    {
        GameObject button;
        Sprite icon = recipe.cardsToCreate[0].sprite;
        if (index < recipesButtons.Count)
        {
            button = recipesButtons[index];
            button.SetActive(true);
        }
        else
        {
            button = Instantiate(recipeButtonPrefab, recipeButtonContainer.transform);
            recipesButtons.Add(button);
        }
        button.GetComponentInChildren<Image>().sprite = icon;
        button.GetComponent<UIRecipeButton>().recipe = recipe;
    }


    public void ShowTooltip(string name, string description, List<ResourceCost> resourceTypes, Vector3 position, Pivot pivot, int id, bool hasBuildingSettings = false)
    {
        if (tooltipID > 0) return;
        tooltipID = id;
        List<string> lines = new List<string>
        {
            $"<b>{name}</b>", $"",
            description
        };

        transform.position = position + (Vector3)PivotToVector2(pivot);
        backgroundRectTransform.gameObject.SetActive(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }
        _currentLines = lines;
        _currentResourceCosts = resourceTypes;
        _tooltipActiveState = 1;

        buildingSettings.SetActive(hasBuildingSettings);

        UpdateTooltip();
    }

    public void UpdateTooltip()
    {
        if (_tooltipActiveState != 1) return;

        tooltipText.text = string.Join("\n", _currentLines);

        // Ensure TMP layout is updated
        tooltipText.ForceMeshUpdate();

        float textHeight = tooltipText.preferredHeight;

        Vector2 size = backgroundRectTransform.sizeDelta;

        if (_currentResourceCosts.Count > 0)
        {
            size.y = textHeight + padding + paddingResouceIcons;
        }
        else
        {
            size.y = textHeight + padding;
        }

        backgroundRectTransform.sizeDelta = size;

        int lineCount = tooltipText.textInfo.lineCount;

        float baseSpacing = 5f;
        float spacingPerLine = 5f;
        float newSpacing = baseSpacing + Mathf.Max(0, lineCount - 1) * spacingPerLine;

        VerticalLayoutGroup layout = backgroundRectTransform.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.spacing = newSpacing;
        }

        // Set resource icons
        //List<bool> hasResourcesList = ResourceManager.instance.HasResourcesAsList(RoomManager.Instance.localPlayer.playerID, _currentResourceTypes);
        for (int i = 0; i < _currentResourceCosts.Count; i++)
        {
            ResourceSlotUI slot = _iconSlots[i];
            slot.gameObject.SetActive(true);
            slot.SetIcon(resourceDatabase.GetIcon(_currentResourceCosts[i].resource));
            slot.text.text = _currentResourceCosts[i].amount.ToString();
            //slot.SetGreyActive(!hasResourcesList[i]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
    }

    public void HideTooltip(int id = 0)
    {
        if (id == 0 || id == tooltipID)
        {
            tooltipID = 0;
            backgroundRectTransform.gameObject.SetActive(false);
            _tooltipActiveState = 0;
        }
    }

    private IEnumerator DelayHideToolip(float delay, int id)
    {
        yield return new WaitForSeconds(delay);

        HideTooltip(id);
    }

    public bool TryHideTooltip(int id = 0)
    {
        if (!IsMouseOver())
        {
            HideTooltip(id);
            return true;
        }
        return false;
    }

    public bool IsMouseOver()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = mousePos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform.IsChildOf(transform))
                return true;
        }

        return false;
    }

    protected override void HandlePointerEnter(PointerEventData eventData)
    {
        // nothin yet
    }

    protected override void HandlePointerExit(PointerEventData eventData)
    {
        TryHideTooltip();
    }

    #region buttons
    public void StartRecipe(CraftRecipe recipe)
    {
        selectedBuilding.Craft(recipe);
        CancleButton.SetActive(selectedBuilding.HasOpenEffect());
    }

    public void StartUpgrade()
    {
        selectedBuilding.StartUpgrade();
        CancleButton.SetActive(selectedBuilding.HasOpenEffect());
    }

    public void CancleEffect()
    {
        Debug.Log("cancle button");
        selectedBuilding.CancleOpenEffect();
        CancleButton.SetActive(selectedBuilding.HasOpenEffect());
    }
    #endregion
}
