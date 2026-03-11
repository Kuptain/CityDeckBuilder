using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static UnityEvent OnStartTurn = new UnityEvent();
    public static UnityEvent OnEndTurn = new UnityEvent();

    [SerializeField] 
    int currentTurn;

    public void GetCurrentTurn()
    {

    }

    public void EndTurn()
    {
        OnEndTurn.Invoke();
    }

    public void StarTurn() 
    {
        OnStartTurn.Invoke();
        currentTurn += 1;
    }

}
