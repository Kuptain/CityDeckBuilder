using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static UnityEvent OnStartTurn = new UnityEvent();
    public static UnityEvent OnEndTurn = new UnityEvent();


    public void EndTurn()
    {
        OnEndTurn.Invoke();
    }

    public void StarTurn() 
    {
        OnStartTurn.Invoke();
    }

}
