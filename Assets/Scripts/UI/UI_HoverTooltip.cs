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
    [SerializeField] private float padding = 150f;
    [SerializeField] private float paddingResouceIcons = 50f;
    [SerializeField] private Vector2 pivotTopLeft;
    [SerializeField] private Vector2 pivotBottomLeft;
    [SerializeField] private Vector2 pivotTopRight;
    [SerializeField] private Vector2 pivotBottomRight;
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
    public void ShowTooltip(string name, string description, List<ResourceCost> resourceTypes, Vector3 position, Pivot pivot)
    {
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

    public void HideTooltip()
    {
        backgroundRectTransform.gameObject.SetActive(false);
        _tooltipActiveState = 0;
    }
}
