using UnityEngine;

public class EndOfTurnButton : MonoBehaviour
{
    public void EndTurn()
    {
        TurnManager.OnEndTurn.Invoke();
    }
}
