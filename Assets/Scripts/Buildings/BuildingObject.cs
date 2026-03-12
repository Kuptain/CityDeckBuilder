using UnityEngine;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    [HideInInspector] public BuildingData data;

    public void Hover()
    {
        ShowHighlight();
    }

    public void Click()
    {
        Debug.Log("click");
        data.OnClick.Invoke();
    }
    public void Drag(Card card)
    {
        Debug.Log(card.GetType());
        data.OnDrag.Invoke((Card)card);
    }

    public void ShowHighlight()
    {
        Debug.Log("highlight");
    }

}
