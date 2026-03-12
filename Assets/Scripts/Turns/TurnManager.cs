using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public static UnityEvent OnStartTurn = new UnityEvent();
    public static UnityEvent OnEndTurn = new UnityEvent();
    public static UnityEvent OnStartCheckingLosingCondition = new UnityEvent();

    [SerializeField] int startingTurnCount;
    [SerializeField] int startingPopulation;
    [SerializeField] int populationPerTurn;

    private int turnCount;
    private int populationCount;

    private void Start()
    {
        OnEndTurn.AddListener(EndTurn);
        OnStartTurn.AddListener(StartTurn);

        AddPopulation(startingPopulation);
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
            AddPopulation(populationPerTurn);
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
    }

    void StartTurn() 
    {

    }
    void AddPopulation(int count)
    {
        // Add new villager
        populationCount += count;
        HUD.Instance.text_PopulationCount.text = populationCount.ToString();
        HUD.Instance.text_PopulationPerTurn.text = "+" + populationPerTurn.ToString();
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
        populationPerTurn += 1;
    }
    void CheckLosingCondition(bool isEndOfSeason)
    {
        OnStartCheckingLosingCondition.Invoke();
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
