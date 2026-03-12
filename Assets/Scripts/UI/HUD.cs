using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public TMPro.TMP_Text text_TurnCount;
    public TMPro.TMP_Text text_PopulationCount;
    public TMPro.TMP_Text text_PopulationPerTurn;
    public TMPro.TMP_Text text_HousingCount;
    public TMPro.TMP_Text text_FoodCount;
    public TMPro.TMP_Text text_Deck;
    public TMPro.TMP_Text text_Discard;
    public Transform panelBuildingButtons;
    public Transform panelGameLost;
    public static HUD Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate HUD detected.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
