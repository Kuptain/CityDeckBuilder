using UnityEngine;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    [HideInInspector] public BuildingData data;
    bool usedAbility;

    private void Start()
    {
        TurnManager.OnEndTurn.AddListener(EndOfTurn);
    }

    public void Hover()
    {
        ShowHighlight();
    }

    public void Click()
    {
        if (usedAbility)
        {
            Debug.LogError(data.name + "ability was already used");
        }
        Debug.Log("click");
        data.OnClick.Invoke();
        usedAbility = true;
    }
    public void Drag(Card card)
    {
        if (usedAbility)
        {
            Debug.LogError(data.name + "ability was already used");
        }
        if (card.Contains(data.EffectCost))
        {
            data.OnDrag.Invoke((Card)card);
            usedAbility = true;
        }
    }

    public void ShowHighlight()
    {
        Debug.Log("highlight");
    }

    void EndOfTurn()
    {
        usedAbility = false;
        data.OnEndOfTurn.Invoke();
    }

    public void Build(Tile tile)
    {
        data.OnBuild.Invoke(tile);
    }
}
