using UnityEngine;
using System;
using static Tile;
using System.Collections.Generic;
using UnityEngine.UIElements;

// Convert Network Objects to store grid visuals here

public class TileVisualsManager : MonoBehaviour
{
    //[Networked, Capacity(400)] public NetworkArray<VisualTileData> gridTileVisuals => default;
    public Dictionary<Vector2Int, TileVisual> tileVisualMap = new Dictionary<Vector2Int, TileVisual>();
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform gridVisualsNavMesh;

    public static TileVisualsManager Instance { get; private set; }
    public void Awake()
    {
        // Only assign singleton once
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate CollectibleManager detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


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
    public void SetInitialGridTileTypes(Vector2Int gridPosition)
    {
        TileType tileType = TileType.Wood;
        int x = gridPosition.x;
        int y = gridPosition.y;

        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        int DEFAULT_RANGE = 2;

        Vector2Int centre = new Vector2Int((width - 1) / 2, (height - 1) / 2);

        if (tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            if (gridPosition.x == centre.x && y == centre.y)
            {
                tileType = TileType.Centre;
                BuildingManager.Instance.SpawnBuilding(new Vector2Int(x, y), 0, 0);
            }
            else if (x == 0 || x == width - 1 || gridPosition.y == 0 || y == height - 1)
            {
                tileType = TileType.Edge;
            }
            else if (GridManager.Instance.IsTileInRange(new Vector2Int(x, y), centre, DEFAULT_RANGE))
            {
                tileType = TileType.Default;
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

        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid) continue;
            if (setInitialTypes)
            {
                SetInitialGridTileTypes(tile.gridPosition);
            }
        }

    }

}
