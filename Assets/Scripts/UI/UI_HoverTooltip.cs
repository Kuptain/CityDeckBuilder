using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using System.Resources;

public class UI_HoverTooltip : MonoBehaviour
{
    public static UI_HoverTooltip Instance;

    [SerializeField] private RectTransform backgroundRectTransform;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject resourceIconSlotContainer;
    [SerializeField] private Vector2 padding = new Vector2(8, 8);
    [SerializeField] private ResourceDatabase resourceDatabase;

    private List<string> _currentLines;
    private List<ResourceCost> _currentResourceCosts;
    private RectTransform _canvasRectTransform;
    private ResourceSlotUI[] _iconSlots;
    private int _tooltipActiveState = 0;

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
        /*
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            Mouse.current.position.ReadValue(),
            null,
            out anchoredPos);
        transform.localPosition = anchoredPos;
        */

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


    public void ShowTooltip(List<string> lines, List<ResourceCost> resourceTypes, Vector3 position)
    {
        transform.position = position;
        backgroundRectTransform.gameObject.SetActive(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }
        _currentLines = lines;
        _currentResourceCosts = resourceTypes;
        _tooltipActiveState = 1;
        UpdateTooltip();
    }
    public void ShowTooltip(List<string> lines, Vector3 position)
    {
        transform.position = position;
        backgroundRectTransform.gameObject.SetActive(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }
        _currentLines = lines;
        _currentResourceCosts = new List<ResourceCost>();
        _tooltipActiveState = 1;
        UpdateTooltip();
    }

    public void UpdateTooltip()
    {
        if (_tooltipActiveState != 1) return;

        tooltipText.text = string.Join("\n", _currentLines);

        // Ensure TMP layout is updated
        tooltipText.ForceMeshUpdate();

        // Get preferred height only
        float textHeight = tooltipText.preferredHeight;

        Vector2 size = backgroundRectTransform.sizeDelta;

        // Only modify height
        size.y = textHeight + padding.y;

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

    public void HideTooltip()
    {
        backgroundRectTransform.gameObject.SetActive(false);
        _tooltipActiveState = 0;
    }
}
