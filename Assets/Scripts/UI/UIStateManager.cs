using UnityEngine;
using UnityEngine.Events;

public class UIStateManager : MonoBehaviour
{
    #region events
    public static UnityEvent<UIStates> SwitchState = new UnityEvent<UIStates>();
    #endregion
    public enum UIStates { Playing, Pause, ProductionOverview, DeckOverview }
    UIStates currentState;
    void Start()
    {
        SwitchState.AddListener(OnSwitchState);
    }
    private void OnDestroy()
    {
        SwitchState.RemoveListener(OnSwitchState);
    }
    void OnSwitchState(UIStates newState)
    {
        currentState = newState;
        UpdateUI();
    }
    void UpdateUI()
    {
        switch (currentState)
        {
            case UIStates.Playing:
                DisableAllPanels();
                break;
            case UIStates.Pause:
                DisableAllPanels();
                HUD.Instance.panelPause.gameObject.SetActive(true);
                break;
            case UIStates.ProductionOverview:
                DisableAllPanels();
                HUD.Instance.panelProductionOverview.gameObject.SetActive(true);
                break;
            case UIStates.DeckOverview:
                DisableAllPanels();
                HUD.Instance.panelDeckOverview.gameObject.SetActive(true);
                break;
        }
    }
    void DisableAllPanels()
    {
        HUD.Instance.panelPause.gameObject.SetActive(false);
        HUD.Instance.panelProductionOverview.gameObject.SetActive(false);
        HUD.Instance.panelDeckOverview.gameObject.SetActive(false);
    }
}
