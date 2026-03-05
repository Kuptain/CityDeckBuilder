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
    public static UnityEvent<RessourceType, int> OnRessourceSpend = new UnityEvent<RessourceType, int>();
    public static UnityEvent<RessourceType, int> OnRessourceReceived = new UnityEvent<RessourceType, int>();
    public static UnityEvent<RessourceType> OnRessourceNotDiscovered = new UnityEvent<RessourceType>();
    public static UnityEvent<RessourceType> OnNotEnoughRessources = new UnityEvent<RessourceType>();
    public static UnityEvent<RessourceType> OneNewRessource = new UnityEvent<RessourceType>();
    public static UnityEvent OnRessourceschanged = new UnityEvent();
    #endregion
    Dictionary<RessourceType, int> ressources = new Dictionary<RessourceType, int>();

    public void GetRessources(RessourceType type, int amount)
    {
        if (!ressources.ContainsKey(type))
        {
            AddNewRessource(type);
        }
        ressources[type] += amount;

    }
    public void GetRessources(List<RessourceType> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            GetRessources(_ressources[i], 1);
        }

    }

    public void RemoveRessources(List<RessourceType> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            if (ressources.ContainsKey(_ressources[i]))
            {
                ressources[_ressources[i]] -= 1;
            }
        }
    }

    public void SpendRessources(List<RessourceType> _ressources)
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

    public bool IHaveEnoughRessources(List<RessourceType> cost)
    {
        Dictionary<RessourceType, int> sumOfCost = new Dictionary<RessourceType, int>();
        for (int i = 0; i < cost.Count; i++)
        {
            sumOfCost.TryAdd(cost[i], 0);
            sumOfCost[cost[i]] += 1;
        }
        foreach (RessourceType key in sumOfCost.Keys)
        {
            if (!ressources.ContainsKey(key) || sumOfCost[key] > ressources[key])
            {
                return false;
            }
        }
        return true;
    }

    public bool TryToSpendRessource(List<RessourceType> cost)
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

    public int getRessourceCount(RessourceType type)
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
    void AddNewRessource(RessourceType type)
    {
        ressources.Add(type, 0);
        OneNewRessource.Invoke(type);
    }


}

