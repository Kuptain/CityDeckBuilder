using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourceCard : ICard
{
    public Card_Data data;
    BuildingObject originBuilding;
    public int roundsInHand;
    public int rank;
    public bool temporary;

    public ResourceCard(Card_Data _data)
    {
        data = _data;
    }

    public ResourceCard(ResourceCard copy)
    {
        data = copy.data;
        originBuilding = copy.originBuilding;
        rank = copy.rank;
        roundsInHand = copy.roundsInHand;
    }

    public void Upgrade()
    {
        rank += 1;
    }

    public List<ResourceCost> GetCurrentResources()
    {
        return data.ressources;
    }

    public int GetDecayCount()
    {
        return data.decay - roundsInHand;
    }

    public bool TryToPayFor(ref List<ResourceCost> cost)
    {
        bool returnBool = false;
        List<ResourceCost> currentResources = GetCurrentResources();
        for (int i = cost.Count-1; i >=0; i--)
        {
            for (int j = 0; j < currentResources.Count; j++)
            {
                if (cost[i].resource == currentResources[j].resource)
                {
                    int newCost = cost[i].amount - currentResources[j].amount;
                    if (newCost > 0)
                    {
                        cost[i] = new ResourceCost(cost[i].resource,newCost);
                    }
                    else
                    {
                        cost.RemoveAt(i);
                    }
                    returnBool = true;
                    break;
                }
            }
        }

        return returnBool;
    }
    public bool CheckMatchingResources(List<ResourceCost> cost)
    {
        bool returnBool = false;
        if(cost == null)
        {
            return false;
        }
        List<ResourceCost> currentResources = GetCurrentResources();
        for (int i = cost.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < currentResources.Count; j++)
            {
                if (cost[i].resource == currentResources[j].resource)
                {
                    returnBool = true;
                    break;
                }
            }
        }

        return returnBool;
    }
    public bool HasDesiredRessources(List<ResourceCost> cost)
    {
        List<ResourceCost> currentResources = GetCurrentResources();
        for (int i = cost.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < currentResources.Count; j++)
            {
                if (cost[i].resource == currentResources[j].resource)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int GetCurrentFood()
    {
        return data.FoodAmount;
    }

    public void EndOfTurnInHand()
    {
        roundsInHand += 1;
        if (data.decay != 0 && roundsInHand >= data.decay)
        {
            data = data.decayTarget;
            CardManager.OnCardDecayed.Invoke(this);
        }
    }

    CardType ICard.GetType()
    {
        return CardType.Resource;
    }
}
