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
            dataBase = database;
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
    public static UnityEvent<int> OnHousingChange = new UnityEvent<int>();
    #endregion
    Dictionary<ResourceType, int> ressources = new Dictionary<ResourceType, int>();

    [SerializeField] ResourceDatabase database;
    public static ResourceDatabase dataBase;
    public int housingBaseValue;
    [ReadOnly] public int housing;
    public int food;
    public int population;
    public List<HousingValue> housingValues = new List<HousingValue>();

    private void Start()
    {
        //TurnManager.OnEndTurn.AddListener(LooseAllRessources);
        SetFood(food);
        CalculateHousing(0);
        OnHousingChange.AddListener(CalculateHousing);
    }
    public void CalculateHousing(int housingIncrease)
    {
        housing = housingBaseValue;
        for (int i = 0; i < housingValues.Count; i++)
        {
            housing += housingValues[i].currentValue;
        }
        ShowHousingText(housingIncrease);
        void ShowHousingText(int increase)
        {
            //change text of HousingCount
            HUD.Instance.text_HousingCount.text = housing.ToString();
            //add feedback here to show how much the housing increased
        }
        TurnManager.Instance.CheckLosingCondition(false);
    }

    public void ChangeFood(int amount)
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

public class HousingValue
{
    private BuildingObject source;
    public int currentValue;

    public HousingValue(BuildingObject _source)
    {
        source = _source;
    }

    public BuildingObject GetSource()
    {
        return source;
    }
}