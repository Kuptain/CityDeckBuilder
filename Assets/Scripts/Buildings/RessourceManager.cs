using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RessourceManager : MonoBehaviour
{
    #region singleton
    public static RessourceManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    #region events
    public static UnityEvent<ResourceType, int> OnRessourceSpend = new UnityEvent<ResourceType, int>();
    public static UnityEvent<ResourceType, int> OnRessourceReceived = new UnityEvent<ResourceType, int>();
    public static UnityEvent<ResourceType> OnRessourceNotDiscovered = new UnityEvent<ResourceType>();
    public static UnityEvent<ResourceType> OnNotEnoughRessources = new UnityEvent<ResourceType>();
    public static UnityEvent<ResourceType> OneNewRessource = new UnityEvent<ResourceType>();
    public static UnityEvent OnRessourceschanged = new UnityEvent();
    #endregion
    Dictionary<ResourceType, int> ressources = new Dictionary<ResourceType, int>();

    public void GetRessources(ResourceType type, int amount)
    {
        if (!ressources.ContainsKey(type))
        {
            AddNewRessource(type);
        }
        ressources[type] += amount;

    }
    public void GetRessources(List<ResourceType> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            GetRessources(_ressources[i], 1);
        }

    }

    public void RemoveRessources(List<ResourceType> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            if (ressources.ContainsKey(_ressources[i]))
            {
                ressources[_ressources[i]] -= 1;
            }
        }
    }

    public void SpendRessources(List<ResourceType> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            if (ressources.ContainsKey(_ressources[i]))
            {
                ressources[_ressources[i]] -= 1;
                OnRessourceSpend.Invoke(_ressources[i],1);
            }
        }
    }

    public bool IHaveEnoughRessources(List<ResourceType> cost)
    {
        Dictionary<ResourceType, int> sumOfCost = new Dictionary<ResourceType, int>();
        for (int i = 0; i < cost.Count; i++)
        {
            sumOfCost.TryAdd(cost[i], 0);
            sumOfCost[cost[i]] += 1;
        }
        foreach (ResourceType key in sumOfCost.Keys)
        {
            if (!ressources.ContainsKey(key) || sumOfCost[key] > ressources[key])
            {
                return false;
            }
        }
        return true;
    }

    public bool TryToSpendRessource(List<ResourceType> cost)
    {

        if (IHaveEnoughRessources(cost))
        {
            SpendRessources(cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public int getRessourceCount(ResourceType type)
    {
        if (!ressources.ContainsKey(type))
        {
            return 0;
        }
        else
        {
            return ressources[type];
        }
    }
    void AddNewRessource(ResourceType type)
    {
        ressources.Add(type, 0);
        OneNewRessource.Invoke(type);
    }


}

