using UnityEngine;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    public BuildingData data;
    
    public void Hover()
    {
        ShowHighlight();
    }

    public void Click()
    {

        data.OnClick.Invoke();
    }
    public void Drag(Card card)
    {
        data.OnDrag.Invoke(card);
    }

    public void ShowHighlight()
    {
        Debug.Log("highlight");
    }

}
