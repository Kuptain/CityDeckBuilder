using UnityEngine;
using System.Collections.Generic;
using static Tile;
using static Unity.Collections.Unicode;

public class BuildingManager : MonoBehaviour
{
    public List<BuildingData> buildingsData;

    [HideInInspector] public int selectedBuildingID = 0;

    public static BuildingManager Instance { get; private set; }


    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate BuildingsDatabase detected.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    public void ChangeSelectedBuildingID(int _buildingID)
    {
        selectedBuildingID = _buildingID;
    }
    public void SpawnBuilding(Vector2Int gridPosition, int buildingID, int playerID = -1)
    {
        GridManager gridManager = GridManager.Instance;
        if(GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
        {
            if (tile.buildingID == 0 && !gridManager.spawnedBuildings.TryGetValue(gridManager.GetIndex(gridPosition.x, gridPosition.y), out GameObject building))
            {
                Tile currentTile = gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)];
                GameObject spawnedBuilding = Instantiate(buildingsData[buildingID].prefab, gridManager.GridToWorldPosition(gridPosition), Quaternion.identity);

                currentTile.buildingID = (byte)buildingID;
                gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)] = currentTile;
                gridManager.spawnedBuildings.Add(gridManager.GetIndex(gridPosition.x, gridPosition.y), spawnedBuilding);


                foreach (Tile _tile in GridManager.Instance.GetTilesInRange(gridPosition, 0))
                {
                    Tile tileInRange = TileVisualsManager.Instance.GetVisualTilelData(_tile.gridPosition);
                    if (tileInRange.tileType != TileType.Edge && tileInRange.tileType != TileType.Centre)
                    {
                        TileVisualsManager.Instance.HandleOnUpdateTileVisual(tileInRange.gridPosition, TileType.Default);
                    }
                    else
                    {
                        Debug.LogWarning($"No visual found for tile at {gridPosition}");
                    }
                }
            }
            else
            {
                Debug.Log("buildingID == " + gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)].buildingID);
            }
        }

     
    }
}
