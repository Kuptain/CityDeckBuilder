using System.Collections.Generic;
using UnityEngine;

public class BuildingObject : MonoBehaviour, Iinteractable
{
    [HideInInspector] public BuildingData data;
    [HideInInspector] public int housingIncrease;
    [HideInInspector] public int foodIncrease;
    bool usedAbility;
    [HideInInspector] public Tile tile;

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
        if (!data.endlessUses && usedAbility)
        {
            Debug.LogError(data.name + "ability was already used");
            return;
        }
        Debug.Log("click");
        data.OnClick.Invoke();
        usedAbility = true;
    }
    public void Drag(Card card)
    {
        if (!data.endlessUses && usedAbility)
        {
            Debug.LogError(data.name + "ability was already used");
            return;
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

    public void Build(Tile _tile)
    {
        data.OnBuild.Invoke(_tile);
        tile = _tile;
    }

    public void increaseHousing()
    {
        data.IncreaseHousingPerNeighbour(tile);
        ResourceManager.instance.housing += housingIncrease;
    }

    public void increaseFood()
    {
        data.IncreaseFoodPerFreeNeighbour(tile);
        ResourceManager.instance.food += foodIncrease;
    }

    public void IncreaseFoodPerFreeNeighbour(Tile tile)
    {
        List<Tile> neighbours = GridManager.Instance.GetTilesInRange(tile.gridPosition, 1);
        int sum = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].currentBuilding == null)
            {
                sum += 1;
            }
        }
        tile.currentBuilding.foodIncrease = sum;
    }

    public void IncreaseHousingPerNeighbour(Tile tile)
    {
        List<Tile> neighbours = GridManager.Instance.GetTilesInRange(tile.gridPosition, 1);
        int sum = 0;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours[i].currentBuilding.data.ID == 2)
            {
                sum += 1;
            }
        }
        tile.currentBuilding.housingIncrease = sum;
    }
}
