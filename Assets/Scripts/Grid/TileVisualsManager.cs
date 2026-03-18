using UnityEngine;
using System;
using static Tile;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Convert Network Objects to store grid visuals here

public class TileVisualsManager : MonoBehaviour
{
    public Dictionary<Vector2Int, TileVisual> tileVisualMap = new Dictionary<Vector2Int, TileVisual>();
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform gridVisualsNavMesh;
    [SerializeField] private BuildingData centreBuilding;
    [SerializeField] private GameObject cameraController;

    #region Singleton
    public static TileVisualsManager Instance { get; private set; }
    public void Awake()
    {
        // Only assign singleton once
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate TileVisualsManager detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    #endregion

    public Tile GetVisualTilelData(Vector2Int _gridPosition)
    {
        Tile _visual = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(_gridPosition.x, _gridPosition.y)];
        return _visual;
    }

    public void HandleOnUpdateTileVisual(Vector2Int gridPosition, TileType _tileType, byte _randomVariantID = 0)
    {
        if (tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            int x = gridPosition.x;
            int y = gridPosition.y;

            if (GridManager.Instance.TryGetTile(x, y, out Tile tile))
            {
                tile.tileType = _tileType;
                int index = GridManager.Instance.GetIndex(x, y);
                GridManager.Instance.gridArray[index] = tile;

                visual.UpdateTileTypeVisual();
            }
        }
        else
        {
            Debug.LogError("Failed to get GridTileVisual at gridPosition: " + gridPosition);
        }
    }
    public void SetDefaultTileExploredState(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;

        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        int DEFAULT_RANGE = 3;

        Vector2Int centre = new Vector2Int((width - 1) / 2, (height - 1) / 2);

        if (GridManager.Instance.TryGetTile(x, y, out Tile tile))
        {
            if (GridManager.Instance.IsTileInRange(new Vector2Int(x, y), centre, DEFAULT_RANGE))
            {
                tile.SetExplored(true);
            }
            else
            {
                tile.SetExplored(false);
            }
            int index = GridManager.Instance.GetIndex(x, y);
            GridManager.Instance.gridArray[index] = tile;
        }
        else
        {
            Debug.LogWarning($"GridTile is null for tile at gridPosition {gridPosition}");
        }

    }
    public void SetDefaultTileVisibleState(Vector2Int gridPosition)
    {
        int x = gridPosition.x;
        int y = gridPosition.y;

        int DEFAULT_RANGE = 2;

        if (GridManager.Instance.TryGetTile(x, y, out Tile tile))
        {
            if (tile.isExplored)
            {
                tile.SetVisible(true);
            }
            else if(GridManager.Instance.IsExploredTileInRange(gridPosition, DEFAULT_RANGE))
            {
                tile.SetVisible(true);
            }
            else
            {
                tile.SetVisible(false);
            }
            int index = GridManager.Instance.GetIndex(x, y);
            GridManager.Instance.gridArray[index] = tile;
        }
        else
        {
            Debug.LogWarning($"GridTile is null for tile at gridPosition {gridPosition}");
        }
    }
    public void SetInitialGridTileType(Vector2Int gridPosition)
    {
        TileType tileType = TileType.Default;
        int x = gridPosition.x;
        int y = gridPosition.y;

        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        Vector2Int centre = new Vector2Int((width - 1) / 2, (height - 1) / 2);

        if (tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            if (x == centre.x && y == centre.y)
            {
                tileType = TileType.Centre;

                BuildingManager.Instance.SpawnBuilding(new Vector2Int(x, y), centreBuilding).FinishConstruction();
                Instantiate(cameraController, GridManager.Instance.GridToWorldPosition(gridPosition), Quaternion.identity);

            }
            else if (x == 0 || x == width - 1 || gridPosition.y == 0 || y == height - 1)
            {
                tileType = TileType.Edge;
            }

            HandleOnUpdateTileVisual(gridPosition, tileType);
        }
        else
        {
            Debug.LogWarning($"GridTileVisual is null for tile at gridPosition {gridPosition}");
        }
    }

    public void InstantiateGridTileVisualFromData(bool setInitialTypes = false)
    {
        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid) continue;
            Quaternion hexRotation = Quaternion.Euler(new Vector3(0, tile.rotationIndex * 60f, 0));
            GameObject spawnedTile = Instantiate(_tilePrefab, GridManager.Instance.GridToWorldPosition(tile.gridPosition), hexRotation);
            spawnedTile.name = $"Tile {tile.gridPosition.x} {tile.gridPosition.y}";
            spawnedTile.transform.parent = gridVisualsNavMesh;


            TileVisual gridTileVisual = spawnedTile.GetComponent<TileVisual>();
            tileVisualMap[tile.gridPosition] = gridTileVisual;

            gridTileVisual?.Init(tile.gridPosition);
        }

        NavMeshRuntimeBaker.Instance.BakeNavMesh();

        if (setInitialTypes)
        {
            foreach (var tile in GridManager.Instance.gridArray)
            {
                if (!tile.isValid) continue;
                SetInitialGridTileType(tile.gridPosition);
            }
            foreach (var tile in GridManager.Instance.gridArray)
            {
                if (!tile.isValid) continue;
                SetDefaultTileExploredState(tile.gridPosition);
            }
            foreach (var tile in GridManager.Instance.gridArray)
            {
                if (!tile.isValid) continue;
                SetDefaultTileVisibleState(tile.gridPosition);
            }
        }


    }

}
