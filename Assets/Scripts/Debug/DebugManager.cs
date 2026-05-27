using UnityEngine;

public class DebugManager : Manager
{
    private void Start()
    {
        Inputmanager.OnInteract.AddListener(GetTileDebugRay);
    }

    void GetTileDebugRay()
    {
        if (debugMode)
        {
            var raycastHit = GridManager.Instance.GroundRaycast();
            Vector2Int gridPosition = GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition);

            if (raycastHit.isGround)
            {
                if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
                {
                    DebugTileInfo(tile);
                }
            }
        }
    }

    public void DebugTileInfo(Tile tile)
    {
        SendLog("Tile at position: " + tile.gridPosition +"; Building: "+ tile.currentBuilding + "; is explored: " + tile.isExplored + "; is safe: " + tile.isSafe);
    }
}
