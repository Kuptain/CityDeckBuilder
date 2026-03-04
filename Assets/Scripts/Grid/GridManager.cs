using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using static Tile;

public class GridManager : MonoBehaviour
{
    public int width, height;
    [SerializeField] private float _tileSize;
    [SerializeField] private Transform _cam;
    private const float TILE_SIZE_MULTIPLIER = 0.87f;
    public enum TileDirection { top, rightTop, rightBottom, bottom, leftBottom, leftTop }
    public Tile[] gridArray;

    public Tile gridNullTile { get; set; }

    public static GridManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GridManager detected.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        gridArray = new Tile[width * height];
        GenerateGridData();
    }

    public int GetIndex(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            //throw new IndexOutOfRangeException();
            Debug.Log("GridManager.GetIndex() : Out of Index, escaping to: " + width + height * width + 1 + ", gridArray Length: " + gridArray.Length);
            return - 1;
        }
        return x + y * width;
    }

    public bool TryGetTile(int x, int y, out Tile tile)
    {
        tile = default;

        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;

        tile = gridArray[x + y * width];
        return true;
    }

    // https://www.redblobgames.com/grids/hexagons/
    Vector3Int OffsetToCube(int x, int y) // Chat GPT helped
    {
        int realX = -x; // Flip X to match increasing left
        int z = y - (realX + (realX & 1)) / 2;
        int yCube = -realX - z;
        return new Vector3Int(realX, yCube, z);
    }
    Vector2Int CubeToOffset(Vector3Int cube)
    {
        int col = -cube.x; // Flip back X if OffsetToCube flipped it
        int row = cube.z + ((cube.x + (cube.x & 1)) / 2);
        return new Vector2Int(col, row);
    }
    public bool IsTileInRange(Vector2Int fromOffset, Vector2Int toOffset, int range) // Chat GPT helped
    {
        Vector3Int fromCube = OffsetToCube(fromOffset.x, fromOffset.y);
        Vector3Int toCube = OffsetToCube(toOffset.x, toOffset.y);
        return HexDistance(fromCube, toCube) <= range;
    }
    int HexDistance(Vector3Int a, Vector3Int b) // Chat GPT helped
    {
        return Mathf.Max(
            Mathf.Abs(a.x - b.x),
            Mathf.Abs(a.y - b.y),
            Mathf.Abs(a.z - b.z)
        );
    }

    public List<Tile> GetTilesInRange(Vector2Int center, int range)
    {
        List<Tile> result = new List<Tile>();
        Vector3Int centerCube = OffsetToCube(center.x, center.y);

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
            {
                int dz = -dx - dy;
                Vector3Int cube = new Vector3Int(centerCube.x + dx, centerCube.y + dy, centerCube.z + dz);
                Vector2Int offset = CubeToOffset(cube);

                if (IsValidGridPosition(offset))
                {
                    Tile tile = gridArray[GetIndex(offset.x, offset.y)];
                    result.Add(tile);
                }
            }
        }

        return result;
    }
    public Tile ReturnDirectionalTile(Vector2 gridPosition, TileDirection direction, int distance) // Chat GPT helped
    {
        Vector2Int origin = new Vector2Int((int)gridPosition.x, (int)gridPosition.y);

        if (!IsValidGridPosition(origin)) return gridNullTile;

        bool isOffset = gridArray[GetIndex(origin.x, origin.y)].isOffset;

        // Get directional offset based on direction, distance, and offset
        Vector2Int offset = GetDirectionalOffset(direction, distance, isOffset);
        Vector2Int targetPos = origin + offset;

        if (!IsValidGridPosition(targetPos))
        {
            Debug.LogWarning("ReturnDirectionalTile: target position is out of bounds.");
            return gridNullTile;
        }

        Tile originTile = gridArray[GetIndex(origin.x, origin.y)];
        Tile targetTile = gridArray[GetIndex(targetPos.x, targetPos.y)];

        if (originTile.gridPosition == targetTile.gridPosition && distance > 0)
        {
            Debug.LogError("ReturnDirectionalTile: Target tile equals origin tile!");
        }

        return targetTile;
    }
    private Vector2Int GetDirectionalOffset(TileDirection dir, int distance, bool isOffset) // Chat GPT helped
    {
        // Even-q flat-topped offsets based on your custom layout
        // X increases to the left, even columns are offset upward

        switch (dir)
        {
            case TileDirection.top:
                return new Vector2Int(0, distance);
            case TileDirection.bottom:
                return new Vector2Int(0, -distance);
            case TileDirection.rightTop:
                return new Vector2Int(distance, isOffset ? 0 : distance);
            case TileDirection.rightBottom:
                return new Vector2Int(distance, isOffset ? -distance : 0);
            case TileDirection.leftTop:
                return new Vector2Int(-distance, isOffset ? 0 : distance);
            case TileDirection.leftBottom:
                return new Vector2Int(-distance, isOffset ? -distance : 0);
            default:
                return Vector2Int.zero;
        }
    }
    private bool IsValidGridPosition(Vector2Int pos) // Chat GPT
    {
        return pos.x >= 0 && pos.y >= 0 &&
               pos.x < width && pos.y < height;
    }
    public Tile FindPlayerStartingTile(int _playerID)
    {
        Tile targetTile = new Tile();
        Tile centreTile = gridArray[GetIndex((width - 1) / 2, (height - 1) / 2)];

        switch (_playerID)
        {
            case 1:
                targetTile = ReturnDirectionalTile(centreTile.gridPosition, TileDirection.top, 2);
                break;
            case 2:
                targetTile = ReturnDirectionalTile(centreTile.gridPosition, TileDirection.bottom, 2);
                break;
        }
        return targetTile;
    }
    public Vector3 GridToWorldPosition(Vector2 gridPosition)
    {
        bool isOffset = gridArray[GetIndex((int)gridPosition.x, (int)gridPosition.y)].isOffset;
        float offsetHeight = isOffset ? -_tileSize / 2f : 0;

        Vector3 worldPos = new Vector3(gridPosition.x * _tileSize * TILE_SIZE_MULTIPLIER, 0, gridPosition.y * _tileSize + offsetHeight);
        return worldPos;
    }
    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int gridX = Mathf.RoundToInt(
            worldPosition.x / (_tileSize * TILE_SIZE_MULTIPLIER)
        );

        // Determine if this column is offset
        bool isOffset = gridArray[GetIndex(gridX, 0)].isOffset;
        float offsetHeight = isOffset ? -_tileSize / 2f : 0f;

        int gridY = Mathf.RoundToInt(
            (worldPosition.z - offsetHeight) / _tileSize
        );

        return new Vector2Int(gridX, gridY);
    }
    public Vector3 SnapToGrid(Vector3 worldPosition, float snapStrength) // Chat GPT
    {
        snapStrength = Mathf.Clamp01(snapStrength);

        // 1. Get grid position
        Vector2Int gridPos = WorldToGridPosition(worldPosition);

        // 2. Get tile center in world space
        Vector3 tileCenter = GridToWorldPosition(gridPos);

        // 3. Measure distance
        float distance = Vector2.Distance(new Vector2(worldPosition.x, worldPosition.z), new Vector2(tileCenter.x, tileCenter.z));

        // 4. Define max snap distance (half tile size works well)
        float maxSnapDistance = _tileSize * 0.5f;

        float allowedDistance = maxSnapDistance * snapStrength;

        if (distance <= allowedDistance)
        {
            return tileCenter;
        }

        return worldPosition;
    }
    private (int colorID, bool isOffset) GetColorAndOffset(int x, int y)
    {
        if ((x % 2 == 0 && y % 3 == 0) || (x % 2 != 0 && (y - 2) % 3 == 0))
            return (1, x % 2 != 0);

        if ((x % 2 != 0 && y % 3 == 0) || (x % 2 == 0 && (y - 1) % 3 == 0))
            return (2, x % 2 != 0);

        if ((x % 2 != 0 && (y - 1) % 3 == 0) || (x % 2 == 0 && (y - 2) % 3 == 0))
            return (3, x % 2 != 0);

        return (0, false);
    }

    public void GenerateGridData()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile visualData = new Tile();
                Vector2Int gridPos = new Vector2Int(x, y);
                (int colorID, bool isOffset) = GetColorAndOffset(x, y);
                TileType tileType = TileType.Wood;
                byte angleIndex = (byte)UnityEngine.Random.Range(0, 6); // 0 to 5

                visualData.Init(gridPos, (byte)colorID, isOffset, tileType, 0, true, angleIndex);
                gridArray[GetIndex(x, y)] = visualData;
            }
        }
        TileVisualsManager.Instance.InstantiateGridTileVisualFromData(true);
    }
}