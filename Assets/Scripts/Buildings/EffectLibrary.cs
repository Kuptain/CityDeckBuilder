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
        building.AddProductionCardsEffect();
        CardManager.OnProductiionToDeck.AddListener(building.AddProductionCardsEffect);
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

    public void discoverTiles(BuildingObject building, Card card)
    {
        
    }
    public void discoverMoreTiles(BuildingObject building, Card card)
    {

    }
}
