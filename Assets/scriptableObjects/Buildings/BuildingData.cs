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


}

