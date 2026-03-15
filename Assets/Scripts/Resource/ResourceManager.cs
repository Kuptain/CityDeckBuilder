using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    #region singleton
    public static ResourceManager instance;
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
    public static UnityEvent  OnHousingChange = new UnityEvent();
    #endregion
    Dictionary<ResourceType, int> ressources = new Dictionary<ResourceType, int>();

    public int housingBaseValue;
    public int housing;
    public int food;
    public int population;

    private void Start()
    {
        //TurnManager.OnEndTurn.AddListener(LooseAllRessources);
        OnHousingChange.AddListener(SetUpHousing);
        SetUpHousing();
        HUD.Instance.text_FoodCount.text = food.ToString();
    }
    void SetUpHousing()
    {
        Debug.Log("ResourceManager: SetUpHousing()");
        housing = housingBaseValue;
        HUD.Instance.text_HousingCount.text = housing.ToString();
    }
    public void AddHousing(int value)
    {
        housing += value;
        HUD.Instance.text_HousingCount.text = housing.ToString();
        TurnManager.Instance.CheckLosingCondition(false);
    }

    public void ChangeFood(int amount )
    {
        food += amount;
        HUD.Instance.text_FoodCount.text = food.ToString();
        TurnManager.Instance.CheckLosingCondition(false);
    }
    public void SetFood(int amount)
    {
        food = amount;
        HUD.Instance.text_FoodCount.text = food.ToString();
        TurnManager.Instance.CheckLosingCondition(false);
    }
    public void GetRessources(ResourceType type, int amount)
    {
        if (!ressources.ContainsKey(type))
        {
            AddNewRessource(type);
        }
        ressources[type] += amount;

    }
    public void GetRessources(List<ResourceCost> _ressourceCosts)
    {
        for (int i = _ressourceCosts.Count - 1; i >= 0; i--)
        {
            ResourceType type = _ressourceCosts[i].resource;
            int amount = _ressourceCosts[i].amount;

            GetRessources(type, amount);
        }

    }

    public void RemoveRessources(List<ResourceCost> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            if (ressources.ContainsKey(_ressources[i].resource))
            {
                ressources[_ressources[i].resource] -= 1;
            }
        }
    }

    public void SpendRessources(List<ResourceCost> _ressources)
    {
        for (int i = _ressources.Count - 1; i >= 0; i--)
        {
            if (ressources.ContainsKey(_ressources[i].resource))
            {
                ressources[_ressources[i].resource] -= _ressources[i].amount;
                OnRessourceSpend.Invoke(_ressources[i].resource, _ressources[i].amount);
            }
        }
    }

    void LooseAllRessources()
    {
        foreach(var key in ressources.Keys.ToList())
        {
            ressources[key] = 0;
        }
    }


    public bool IHaveEnoughRessources(List<ResourceCost> cost)
    {
        Dictionary<ResourceType, int> sumOfCost = new Dictionary<ResourceType, int>();
        for (int i = 0; i < cost.Count; i++)
        {
            sumOfCost.TryAdd(cost[i].resource, 0);
            sumOfCost[cost[i].resource] += cost[i].amount;
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

    public bool TryToSpendRessource(List<ResourceCost> cost)
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

    public int GetRessourceCount(ResourceType type)
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

