using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class ResourceCard_Data : ScriptableObject
{
    public Sprite sprite;
    public string cardName;
    public string cardDescription;
    public List<ResourceCost> ressources;
    [Header("decay")]
    public int decay;
    public ResourceCard_Data decayTarget;
    [Header("food")]
    public int FoodAmount;

    public ResourceCard_Data(ResourceCard_Data card)
    {
        sprite = card.sprite;
        ressources = new List<ResourceCost>(card.ressources);
    }

    public bool Contains(List<ResourceCost> cost)
    {
        bool costSatisfied = false;
        foreach (ResourceCost x in cost)
        {
            costSatisfied = false;
            foreach (ResourceCost y in ressources)
            {
                if (x.resource == y.resource)
                {
                    if (y.amount > 0)
                    {
                        costSatisfied = true;
                    }
                    break;
                }
            }
            if (costSatisfied == false)
            {
                return false;
            }
        }
        return true;
    }

    public bool TryToPayFor(List<ResourceCost> cost)
    {
        bool costSatisfied = false;
        for (int i = cost.Count-1; i >=0; i--)
        {
            foreach (ResourceCost y in ressources)
            {
                if (cost[i].resource == y.resource)
                {
                    if (y.amount > 0)
                    {
                        costSatisfied = true;
                    }
                    break;
                }
            }

        }
        return costSatisfied;
    }
}