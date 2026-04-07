using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectLibrary", menuName = "Scriptable Objects/EffectLibrary")]
public class EffectLibrary : ScriptableObject
{ 
    public void AddListenerSpawnNPC(BuildingObject building, Card card)
    {
        Vector3 spawnPosition = GridManager.Instance.GridToWorldPosition(building.GetTile().gridPosition);
        TurnManager.OnPopulationIncreased.AddListener(() => TurnManager.Instance.SpawnNPC(spawnPosition));
    }
    public void AddCards(BuildingObject building, Card card)
    {
        List<Card_Data> cardsToAdd = building.GetCurrentCards();
        CardManager.instance.AddCardsToProduction(cardsToAdd);
        building.VisualProductionCards();
        CardManager.OnProductiionToDeck.AddListener(building.VisualProductionCards);
    }

    public void Test(BuildingObject building, Card card)
    {
        Debug.Log("test");
    }

    public void StockRessource(BuildingObject building, Card card)
    {
        building.AddCardToStock(card);
    }

    public void createCard(BuildingObject building, Card card)
    {
        building.TryToCraft();
    }

    public static void AddHousing(BuildingObject building, Card card)
    {
        int housing = building.data.GetRankData(building.GetRank()).housingIncrease;
        if(building.housingValue == null)
        {
            building.housingValue = new HousingValue(building);
            ResourceManager.instance.housingValues.Add(building.housingValue);
        }
        int housingChange = housing - building.housingValue.currentValue;
        building.housingValue.currentValue = housing;
        ResourceManager.OnHousingChange.Invoke(housingChange);
    }

    public void discoverTiles(BuildingObject building, Card card)
    {
        discoverneighbours(building, 1);
    }
    public void discoverMoreTiles(BuildingObject building, Card card)
    {
        discoverneighbours(building, 2);
    }

    private static void discoverneighbours(BuildingObject building, int range)
    {
        List<Tile> neighbours = GridManager.Instance.GetTilesInRange(building.GetTile().gridPosition, range);
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].SetExploredState(true, true,true);
        }
    }


    public void AddFood(BuildingObject building, Card card)
    {
        ResourceManager.instance.ChangeFood(card.GetCurrentFood());
    }
}
