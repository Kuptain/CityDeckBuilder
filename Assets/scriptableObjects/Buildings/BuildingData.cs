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
    public List<ResourceCost> EffectCost = new List<ResourceCost>();
    public UnityEvent OnBuild = new UnityEvent();
    public UnityEvent<Card> OnDrag = new UnityEvent<Card>();
    public UnityEvent OnClick = new UnityEvent();
    public UnityEvent OnEndOfRound = new UnityEvent();


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

    public void RemoveCard(Card card)
    {
       
        ResourceCost _cost = new ResourceCost();
        _cost.amount = 1;
        _cost.resource = ResourceType.gold;
        List<ResourceCost> cost = new List<ResourceCost>() {_cost };
        if (card.Contains(EffectCost) && ResourceManager.instance.IHaveEnoughRessources(cost));
        {
            CardManager.instance.RemoveCardFromHand(card);
        }
    }

    public void AddCards()
    {
        CardManager.instance.AddCardsToDiscard(cardsToAdd);
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
    public void IncreaseHousingValue(int i )
    {
        housingIncrease += i;
        ResourceManager.OnHousingChange.Invoke();
    }

    public void SellFood()
    {
        Debug.Log("Sellfood");
        ResourceManager.instance.ChangeFood(-1);
        ResourceManager.instance.GetRessources(ResourceType.gold, 1);
    }

    public void CreateFood(Card card)
    {
        CardManager.instance.RemoveCardFromHand(card);
        ResourceManager.instance.ChangeFood(3);
    }
}

