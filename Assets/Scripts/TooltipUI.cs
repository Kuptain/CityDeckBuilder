using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Mono.Cecil;
using System.Resources;
using UnityEditor.EditorTools;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public RectTransform backgroundRectTransform;
    public TextMeshProUGUI tooltipText;
    public GameObject resourceIconSlotContainer;
    public Vector2 padding = new Vector2(8, 8);

    private RectTransform _canvasRectTransform;
    private RessourceSlotUI[] _iconSlots;
    int _tooltipActiveState = 0;

    private void Awake()
    {
        Instance = this;
        _canvasRectTransform = transform.root.GetComponent<RectTransform>();
        HideTooltip();
        
        _iconSlots = resourceIconSlotContainer.GetComponentsInChildren<RessourceSlotUI>(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }
        
        StartCoroutine(SubscribeToUpdateTooltip());
    }
    IEnumerator SubscribeToUpdateTooltip()
    {
        while (RessourceManager.instance == null)
        {
            yield return null;
        }
        //RessourceManager.instance.OnResourcesUpdated += UpdateTooltip; // add to RessourceManager
    }
    private void OnDestroy()
    {
        if (RessourceManager.instance != null)
        {
            //RessourceManager.instance.OnResourcesUpdated -= UpdateTooltip;
        }
    }

    void Update()
    {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform,
            Input.mousePosition,
            null,
            out anchoredPos);
        transform.localPosition = anchoredPos;

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
    List<string> _currentLines;
    List<RessourceType> _currentResourceTypes;

    public void ShowTooltip(List<string> lines, List<RessourceType> resourceTypes)
    {
        backgroundRectTransform.gameObject.SetActive(true);
        foreach (var slot in _iconSlots)
        {
            slot.gameObject.SetActive(false);
        }
        _currentLines = lines;
        _currentResourceTypes = resourceTypes;
        _tooltipActiveState = 1;
        UpdateTooltip();
    }
    public void UpdateTooltip()
    {
        if (_tooltipActiveState != 1) return; // 1 = Tooltip is active
        // Join lines with newline
        tooltipText.text = string.Join("\n", _currentLines);
        Vector2 textSize = tooltipText.GetPreferredValues(tooltipText.text);
        backgroundRectTransform.sizeDelta = textSize + padding;

        int lineCount = tooltipText.textInfo.lineCount;
        // Falls Zeilenanzahl nicht verfügbar, TextInfo updaten
        if (lineCount == 0)
        {
            tooltipText.ForceMeshUpdate();
            lineCount = tooltipText.textInfo.lineCount;
        }
        // Berechne neuen Spacing-Wert
        float spacingPerLine = 5f;
        float baseSpacing = 10f;
        //float newSpacing = baseSpacing + (lineCount - 1) * spacingPerLine;
        float textHeight = tooltipText.preferredHeight;
        float newSpacing = textHeight * 0.5f;

        backgroundRectTransform.GetComponent<VerticalLayoutGroup>().spacing = newSpacing;

        /*
        // Set resource icons
        List<bool> hasResourcesList = RessourceManager.instance.HasResourcesAsList(RoomManager.Instance.localPlayer.playerID, _currentResourceTypes);
        for (int i = 0; i < _currentResourceTypes.Count; i++)
        {
            RessourceSlotUI slot = _iconSlots[i];
            slot.gameObject.SetActive(true);
            slot.SetIcon(_currentResourceTypes[i]);
            slot.SetGreyActive(!hasResourcesList[i]);
        }
        */
        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRectTransform);
    }


    public void HideTooltip()
    {
        backgroundRectTransform.gameObject.SetActive(false);
        _tooltipActiveState = 0;
    }
}
