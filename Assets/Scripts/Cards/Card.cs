using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card
{

    public Card_Data data;
    BuildingObject originBuilding;
    public int roundsInHand;
    public int rank;

    public Card(Card_Data _data)
    {
        data = _data;
    }

    public void Upgrade()
    {
        rank += 1;
    }

    public List<ResourceCost> GetCurrentResources()
    {
        return data.ressources;
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

}
