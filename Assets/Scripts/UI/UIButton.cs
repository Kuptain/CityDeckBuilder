using System;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public UIStateManager.UIStates state;
    public void SwitchUIState()
    {
        UIStateManager.SwitchState.Invoke(state);
    }
}
