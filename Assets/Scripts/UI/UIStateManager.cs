using UnityEngine;
using UnityEngine.Events;

public class UIStateManager : MonoBehaviour
{
    #region events
    public static UnityEvent<UIStates> SwitchState = new UnityEvent<UIStates>();
    #endregion
    public enum UIStates { Playing, Pause, ProductionOverview }
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
                HUD.Instance.panelPause.gameObject.SetActive(false);
                HUD.Instance.panelCardOverview.gameObject.SetActive(false);
                break;
            case UIStates.Pause:
                HUD.Instance.panelPause.gameObject.SetActive(true);
                HUD.Instance.panelCardOverview.gameObject.SetActive(false);
                break;
            case UIStates.ProductionOverview:
                HUD.Instance.panelCardOverview.gameObject.SetActive(true);
                HUD.Instance.panelPause.gameObject.SetActive(false);
                break;
        }
    }
}
