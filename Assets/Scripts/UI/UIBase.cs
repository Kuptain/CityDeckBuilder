using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    protected bool isHovered;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HUD.Instance.EnterUI();
        HandlePointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HUD.Instance.ExitUI();
        HandlePointerExit(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        HandlePointerDown(eventData);
    }

    // These are what child classes override
    protected virtual void HandlePointerEnter(PointerEventData eventData) { }
    protected virtual void HandlePointerExit(PointerEventData eventData) { }
    protected virtual void HandlePointerDown(PointerEventData eventData) { }

    public bool IsHovered() => isHovered;
}