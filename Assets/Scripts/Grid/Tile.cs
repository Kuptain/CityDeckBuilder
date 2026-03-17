using UnityEngine;

public struct Tile
{
    public enum TileType { Default, Centre, Edge, Forest, Mountain, Water }

    public TileType tileType;
    public Vector2Int gridPosition;
    public bool isOffset;
    public bool isValid;

    public bool isExplored;
    public bool isVisible;

    public byte rotationIndex;
    public byte offsetColorID;
    public BuildingObject currentBuilding;

    public void SetExplored(bool state)
    {
        isExplored = state;
        if (TileVisualsManager.Instance.tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            visual.SetExploredVisual(state);
        }
    }
    public void SetVisible(bool state)
    {
        isVisible = state;
        if (TileVisualsManager.Instance.tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            visual.gameObject.SetActive(state);
        }
        // Enable/Disable visuals
    }
    public void Init(Vector2Int _gridPosition, byte _offsetColorID, bool _isOffset = false,
        TileType _tileType = TileType.Default, bool _isValid = true, byte _rotationIndex = 0)
    {
        gridPosition = _gridPosition;
        isOffset = _isOffset;
        tileType = _tileType;
        offsetColorID = _offsetColorID;
        isValid = _isValid;
        rotationIndex = _rotationIndex;
    }
}
