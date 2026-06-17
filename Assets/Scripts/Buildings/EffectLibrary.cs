using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectLibrary", menuName = "Scriptable Objects/EffectLibrary")]
public class EffectLibrary : ScriptableObject
{
    public void AddListenerSpawnNPC(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        Vector3 spawnPosition = GridManager.Instance.GridToWorldPosition(building.GetTile().gridPosition);
        TurnManager.OnPopulationIncreased.AddListener(() => TurnManager.Instance.SpawnNPC(spawnPosition));
    }
   
    public void Test(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        Debug.Log("test");
    }

    public void StockRessource(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        building.AddCardToStock(card);
    }

    public void createTemporaryCard(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        for (int i = 0; i < origin.temporaryCards.Count; i++)
        {
            CardManager.instance.CreateRessourceCard(origin.temporaryCards[i]);
        }
    }



    public static void AddHousing(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        int housing = building.data.GetRankData(building.GetRank()).housingIncrease;
        if (building.housingValue == null)
        {
            building.housingValue = new HousingValue(building);
            ResourceManager.instance.housingValues.Add(building.housingValue);
        }
        int housingChange = housing - building.housingValue.currentValue;
        building.housingValue.currentValue = housing;
        ResourceManager.OnHousingChange.Invoke(housingChange);
    }

    public void discoverTiles(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        discoverneighbours(building, 2);
    }
    public void discoverMoreTiles(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        discoverneighbours(building, 3);
    }

    private static void discoverneighbours(BuildingObject building, int range)
    {
        List<Tile> neighbours = GridManager.Instance.GetTilesInRange(building.GetTile().gridPosition, range);
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].SetSafeState(true);
        }
        neighbours = GridManager.Instance.GetTilesInRange(building.GetTile().gridPosition, range);
        for (int i = 0; i < neighbours.Count; i++)
        {
            neighbours[i].SetExploredState(true, true, true);
        }
    }


    public void AddFood(BuildingObject building, ResourceCard card, BuildingEffect origin)
    {
        ResourceManager.instance.ChangeFood(card.GetCurrentFood());
    }
}
