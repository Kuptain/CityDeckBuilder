using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Mono.Cecil;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    public byte ID;
    public string buildingName;
    public string buildingDescription;
    public List<Card> cardsToAdd;
    public int housingIncrease;

    public GameObject prefab;
    public Sprite uiIcon;
    public GameObject uiButton;
    public List<ResourceCost> resourceCosts;

    [Header("Effects")]
    public bool endlessUses;
    public List<ResourceCost> EffectCost = new List<ResourceCost>();
    public UnityEvent<Tile> OnBuild = new UnityEvent<Tile>();
    public UnityEvent<Card> OnDrag = new UnityEvent<Card>();
    public UnityEvent OnClick = new UnityEvent();
    public UnityEvent OnEndOfTurn = new UnityEvent();


    public void DuplicateCard(Card card)
    {
        if (card.Contains(EffectCost))
        {
            Card duplication = card;
            List<Card> cardsToAdd = new List<Card>() { duplication };
            CardManager.instance.AddCardsToDiscard(cardsToAdd);
            CardManager.instance.DiscardCard(card);
        }
    }

    public void AddRessources(Card card)
    {
        ResourceManager.instance.GetRessources(card.ressources);
        CardManager.instance.DiscardCard(card);
    }

    public void SellCard(Card card)
    {
        if (card.Contains(EffectCost))
        {
            CardManager.instance.RemoveCardFromHand(card);
            ResourceManager.instance.GetRessources(ResourceType.gold, 1);
        }
    }

    public void AddCards()
    {
        CardManager.instance.AddCardsToDiscard(cardsToAdd);
    }
    public void changeFoodPerNeighbour(Tile tile)
    {
        tile.currentBuilding.increaseFood();
    }
  
    public void changeHousingPerNeighbour(Tile tile)
    {
        ResourceManager.OnHousingChange.AddListener(tile.currentBuilding.increaseHousing);
        ResourceManager.OnHousingChange.Invoke();
    }
    public void changeHousing()
    {
        ResourceManager.OnHousingChange.AddListener(increaseHousing);
        ResourceManager.OnHousingChange.Invoke();
    }
    void increaseHousing()
    {
        ResourceManager.instance.housing += housingIncrease;
    }

    public void SellFood()
    {
        ResourceManager.instance.ChangeFood(-1);
        ResourceManager.instance.GetRessources(ResourceType.gold, 1);
    }

    public void CreateFood(Card card)
    {
        CardManager.instance.RemoveCardFromHand(card);
        ResourceManager.instance.ChangeFood(3);
    }

    public void BuyStone()
    {
        if (ResourceManager.instance.TryToSpendRessource(EffectCost))
        {
            ResourceManager.instance.GetRessources(ResourceType.stone, 1);
        }
    }
}

