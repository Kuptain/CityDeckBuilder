using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectLibrary", menuName = "Scriptable Objects/EffectLibrary")]
public class EffectLibrary : ScriptableObject
{
    public void AddListenerSpawnNPC(BuildingObject building, ICard card, BuildingEffect origin)
    {
        Vector3 spawnPosition = GridManager.Instance.GridToWorldPosition(building.GetTile().gridPosition);
        TurnManager.OnPopulationIncreased.AddListener(() => TurnManager.Instance.SpawnNPC(spawnPosition));
    }
   
    public void Test(BuildingObject building, ICard card, BuildingEffect origin)
    {
        Debug.Log("test");
    }

    public void StockRessource(BuildingObject building, ICard card, BuildingEffect origin)
    {
        building.AddCardToStock((ResourceCard) card);
    }

    public void createTemporaryCard(BuildingObject building, ICard card, BuildingEffect origin)
    {
        for (int i = 0; i < origin.temporaryCards.Count; i++)
        {
            CardManager.instance.CreateRessourceCard(origin.temporaryCards[i]);
        }
    }

    public void createTemporaryCard(CharacterCard card, TerrainEffect origin)
    {
        for (int i = 0; i < origin.temporaryCards.Count; i++)
        {
            CardManager.instance.CreateRessourceCard(origin.temporaryCards[i]);
        }
    }

    public static void AddHousing(BuildingObject building, ICard card, BuildingEffect origin)
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

    public void discoverTiles(BuildingObject building, ICard card, BuildingEffect origin)
    {
        discoverneighbours(building, 2);
    }
    public void discoverMoreTiles(BuildingObject building, ICard card, BuildingEffect origin)
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


    public void AddFood(BuildingObject building, ICard card, BuildingEffect origin)
    {
        //Implement new food system
        ResourceCard r_card = (ResourceCard)card;
        ResourceManager.instance.ChangeFood(r_card.GetCurrentFood());
    }
}
