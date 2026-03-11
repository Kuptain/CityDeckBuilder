using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TMPro.TMP_Text turnText;
    public Transform panelBuildingButtons;
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
}
