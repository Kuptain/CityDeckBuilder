using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    public Canvas canvas;
    [Header("General UI")]
    public TMPro.TMP_Text text_TurnCount;
    public TMPro.TMP_Text text_PopulationCount;
    public TMPro.TMP_Text text_PopulationPerTurn;
    public TMPro.TMP_Text text_HousingCount;
    public TMPro.TMP_Text text_FoodCount;
    public TMPro.TMP_Text text_Deck;
    public TMPro.TMP_Text text_Production;
    public Transform panelBuildingButtons;
    public Transform panelGameLost;
    public RectTransform deckButtonTransform;
    [Header("UI State Panels")]
    public Transform panelPause;
    public Transform panelProductionOverview;
    public Transform panelDeckOverview;

    private int hoverintUI;

  
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
    public void EnterUI()
    {
        hoverintUI += 1;
    }
    public void ExitUI()
    {
        hoverintUI -= 1;
    }
    public bool IsHoveringUI()
    {
        if (hoverintUI > 0)
        {
            return true;
        }
        return false;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void TogglePause(bool state)
    {
        panelPause.gameObject.SetActive(state);
    }
    public void TogglePause()
    {
        panelPause.gameObject.SetActive(!panelPause.gameObject.activeSelf);
    }
}
