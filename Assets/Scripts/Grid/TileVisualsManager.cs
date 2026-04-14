using UnityEngine;
using static Tile;
using System.Collections.Generic;

[System.Serializable]
public class TileVisualType
{
    public TileType type;
    public Color color;
    [Tooltip("How frequent should this type spawn")]
    public int weight;
    public List<GameObject> visualVariants;
}
public class TileVisualsManager : MonoBehaviour
{
    public Dictionary<Vector2Int, TileVisual> tileVisualMap = new Dictionary<Vector2Int, TileVisual>();
    public int tileVisibleRange = 3;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform gridVisualsNavMesh;
    [SerializeField] private BuildingData centreBuilding;
    [SerializeField] private BuildingData blueprintBuilding;
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

    public TileVisual GetVisualTilelData(Vector2Int gridPosition)
    {
        if (!tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            Debug.LogError("Failed to get GridTileVisual at gridPosition: " + gridPosition);
        }
        return visual;
    }
    void SetInitialUnexploredDirectionOutlines()
    {
        // Gets all directions and stores them in a list
        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid || !tile.isExplored) continue;

            Vector2Int gridPosition = tile.gridPosition;
            GetVisualTilelData(gridPosition).CheckDirectionalOutlines();
        }
    }
    public void HandleOnUpdateTileVisual(Vector2Int gridPosition, TileType _tileType)
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
    public void InstantiateGridTileVisualFromData(bool setInitialTypes = false)
    {
        for (int i = 0; i < GridManager.Instance.gridArray.Length; i++)
        {
            var tile = GridManager.Instance.gridArray[i];
            if (!tile.isValid) continue;
            Quaternion hexRotation = Quaternion.Euler(new Vector3(0, tile.rotationIndex * 60f, 0));
            GameObject spawnedTile = Instantiate(_tilePrefab, GridManager.Instance.GridToWorldPosition(tile.gridPosition), Quaternion.identity);
 
            TileVisual gridTileVisual = spawnedTile.GetComponent<TileVisual>();

            InteractionManager.OnPickUpCard.AddListener(gridTileVisual.EnableHighlight);
            InteractionManager.OnReleaseCard.AddListener(gridTileVisual.DisableHighlight);

            tileVisualMap[tile.gridPosition] = gridTileVisual;

            gridTileVisual.directionOutlines_parent.transform.parent = null;
            spawnedTile.transform.rotation = hexRotation;
            spawnedTile.name = $"Tile {tile.gridPosition.x} {tile.gridPosition.y}";
            spawnedTile.transform.parent = gridVisualsNavMesh;

            gridTileVisual?.Init(tile.gridPosition);
        }

        NavMeshRuntimeBaker.Instance.BakeNavMesh();

        if (setInitialTypes)
        {
            SetInitialGridTileType();
            SetInitialTileExploredState();
            SetInitialUnexploredDirectionOutlines();
            SetInitialBlueprintSpawns();
            SetInitialTileVisibleState();
        }
    }
    private void SetInitialBlueprintSpawns()
    {
        int amountOfBuildings = 30;


        // 1. Copy all unexplored tiles to a list
        List<Tile> unexploredTiles = new List<Tile>();
        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid || tile.isExplored) continue;
            unexploredTiles.Add(tile);
        }
        int spawnCount = Mathf.Min(amountOfBuildings, unexploredTiles.Count);

        // 2. For each entry, spawn a building and remove from list.
        for (int i = 0; i < spawnCount; i++)
        {
            int randomTile = Random.Range(0, unexploredTiles.Count);
            Vector2Int tileGridPosition = unexploredTiles[randomTile].gridPosition;
            BuildingManager.Instance.SpawnBuilding(new Vector2Int(tileGridPosition.x, tileGridPosition.y), blueprintBuilding, true);
            unexploredTiles.Remove(unexploredTiles[randomTile]);
        }
    }

    private void SetInitialTileExploredState()
    {
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        int DEFAULT_RANGE = 3;

        Vector2Int centre = new Vector2Int((width - 1) / 2, (height - 1) / 2);

        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid) continue;

            Vector2Int gridPosition = tile.gridPosition;
            int x = gridPosition.x;
            int y = gridPosition.y;

            if (GridManager.Instance.IsTileInRange(new Vector2Int(x, y), centre, DEFAULT_RANGE))
            {
                tile.SetExploredState(true);
            }
            else
            {
                tile.SetExploredState(false);
            }
        }
    }
    private void SetInitialTileVisibleState()
    {
        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid) continue;

            Vector2Int gridPosition = tile.gridPosition;

            if (tile.isExplored || GridManager.Instance.IsExploredTileInRange(gridPosition, tileVisibleRange))
            {
                tile.SetVisibleState(true);
            }
            else
            {
                tile.SetVisibleState(false);
            }
        }
    }
    private TileVisualType GetRandomVariant(List<TileVisualType> variants) // ChatGPT
    {
        float totalWeight = 0f;

        foreach (var v in variants)
            totalWeight += v.weight;

        float randomPoint = Random.Range(0f, totalWeight);

        float current = 0f;

        foreach (var v in variants)
        {
            current += v.weight;

            if (randomPoint <= current)
                return v;
        }

        return null; // fallback (shouldn't happen)
    }
    private void SetInitialGridTileType()
    {
        TileType tileType = TileType.Default;
        int width = GridManager.Instance.width;
        int height = GridManager.Instance.height;

        foreach (var tile in GridManager.Instance.gridArray)
        {
            if (!tile.isValid) continue;
            tileType = TileType.Default;
            Vector2Int gridPosition = tile.gridPosition;
            int x = gridPosition.x;
            int y = gridPosition.y;
            Vector2Int centre = new Vector2Int((width - 1) / 2, (height - 1) / 2);

            if (tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
            {
                if (x == centre.x && y == centre.y)
                {
                    tileType = TileType.Centre;
                    Instantiate(cameraController, GridManager.Instance.GridToWorldPosition(gridPosition), Quaternion.identity);

                    BuildingObject _centreBuilding = BuildingManager.Instance.SpawnBuilding(new Vector2Int(x, y), centreBuilding, true);
                    _centreBuilding.FinishConstruction();
                    IconAnimationManager.OnCentrebuilding.Invoke(_centreBuilding);
                }
                else if (x == 0 || x == width - 1 || gridPosition.y == 0 || y == height - 1)
                {
                    tileType = TileType.Edge;
                }
                else
                {
                    tileType = GetRandomVariant(visual.tileVisualTypes).type;
                }

                HandleOnUpdateTileVisual(gridPosition, tileType);
            }
            else
            {
                Debug.LogWarning($"GridTileVisual is null for tile at gridPosition {gridPosition}");
            }
        }

    }



}
