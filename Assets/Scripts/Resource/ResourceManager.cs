using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : Manager
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
        SendLog("SetUpHousing()");
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


}

