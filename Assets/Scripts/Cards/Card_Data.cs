using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card_Data : ScriptableObject
{
    public Sprite sprite;
    public string cardName;
    public string cardDescription;
    public List<ResourceCost> ressources;

    public Card_Data(Card_Data card)
    {
        sprite = card.sprite;
        ressources = new List<ResourceCost>(card.ressources);
    }

    public bool Contains(List<ResourceCost> cost)
    {
        bool costSatisfied = false;
        foreach(ResourceCost x in cost)
        {
            costSatisfied = false;
            foreach(ResourceCost y in ressources)
            {
                if(x.resource == y.resource)
                {
                    if (x.amount <= y.amount)
                    {
                        costSatisfied = true;
                    }
                    break;
                }
            }
            if(costSatisfied == false)
            {
                return false;
            }
        }
        return true;
    }
}