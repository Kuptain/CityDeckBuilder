using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static UnityEvent OnStartTurn = new UnityEvent();
    public static UnityEvent OnEndTurn = new UnityEvent();
    public static UnityEvent OnStartCheckingLosingCondition = new UnityEvent();
    public static UnityEvent OnPopulationIncreased = new UnityEvent();

    [SerializeField] int startingTurnCount;
    [SerializeField] int populactionIncreaseModifier;
    [SerializeField] int populationPerYear;
    [SerializeField] GameObject npcPrefab;

    private int turnCount;
    private int populationCount;
    public static TurnManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate TurnManager detected.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        OnEndTurn.AddListener(EndTurn);
        OnStartTurn.AddListener(StartTurn);

        StartCoroutine(DelayInitialPopulation());
        UpdateTurnCount(startingTurnCount);

        StartCoroutine(DelayStartTurn());

    }
    private void OnDestroy()
    {
        OnEndTurn.RemoveListener(EndTurn);
        OnStartTurn.RemoveListener(StartTurn);
    }
    public int GetCurrentTurn()
    {
        return turnCount;
    }

    void EndTurn()
    {
        if (turnCount > 1)
        {
            UpdateTurnCount(turnCount - 1);

            if (turnCount == 1)
            {
                OnStartCheckingLosingCondition.Invoke();
            }
            CheckLosingCondition(false);
            StartCoroutine(DelayStartTurn());
         
        }
        else
        {
            EndOfSeason();
        }
    }
    
    IEnumerator DelayStartTurn()
    {
        yield return null;
        OnStartTurn.Invoke();
        CheckLosingCondition(false);
    }
    IEnumerator DelayInitialPopulation()
    {
        yield return null;
        AddPopulation(populationPerYear);
    }
    void StartTurn() 
    {

    }
    void AddPopulation(int amount)
    {
        // Add new villager
        for (int i = 0; i < amount; i++)
        {
            populationCount += 1;
            OnPopulationIncreased.Invoke();
        }

        HUD.Instance.text_PopulationCount.text = populationCount.ToString();
        HUD.Instance.text_PopulationPerTurn.text = "+" + populationPerYear.ToString();
    }
    public void SpawnNPC(Vector3 spawnPosition)
    {
        GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
        npc.GetComponent<NPC>().originPosition = spawnPosition;
    }
    void UpdateTurnCount(int count)
    {
        turnCount = count;
        HUD.Instance.text_TurnCount.text = turnCount.ToString();
    }
    void EndOfSeason()
    {
        Debug.Log("TurnManager: EndOfSeason()");
        UpdateTurnCount(startingTurnCount);
        CheckLosingCondition(true);
        AddPopulation(populactionIncreaseModifier);
    }
    public void CheckLosingCondition(bool isEndOfSeason)
    {
        
        bool lostGame = false;

        // Check if housing is equal to or higher than population
        if (populationCount > ResourceManager.instance.housing)
        {
            HUD.Instance.text_HousingCount.color = Color.red;
            lostGame = true;

        }
        else
        {
            HUD.Instance.text_HousingCount.color = Color.white;
        }
        if (populationCount > ResourceManager.instance.food)
        {
            HUD.Instance.text_FoodCount.color = Color.red;
            lostGame = true;
        }
        else
        {
            HUD.Instance.text_FoodCount.color = Color.white;
        }

        if (isEndOfSeason)
        {
            if (lostGame)
            {
                Debug.Log("TurnManager: GAME LOST");
                HUD.Instance.panelGameLost.gameObject.SetActive(true);
            }
            else
            {
                ResourceManager.instance.SetFood(0);
                UpdateTurnCount(startingTurnCount);
                StartCoroutine(DelayStartTurn());
            }
        }

    }
}
