using UnityEngine;

public class FogManager : MonoBehaviour
{
    private void Start()
    {
        TurnManager.OnEndOfSeason.AddListener(FogMovesIn);
        
    }

    void FogMovesIn()
    {
        for(int i = 0; i < GridManager.Instance.gridArray.Length; i++)
        {
            GridManager.Instance.gridArray[i].SetExploredState(GridManager.Instance.gridArray[i].isSafe,true,true);
        }
    }
}
