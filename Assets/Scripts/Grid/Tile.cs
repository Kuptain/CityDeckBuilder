using UnityEngine;

public struct Tile
{
    public enum TileType { Default, Centre, Edge, Wood, Stone, Water, Start, Building }

    public TileType tileType;
    public Vector2Int gridPosition;
    public bool isOffset;
    public bool isValid;
    public byte rotationIndex;
    public byte offsetColorID;
    public BuildingData currentBuilding;

    public void Init(Vector2Int _gridPosition, byte _offsetColorID, bool _isOffset = false,
        TileType _tileType = TileType.Default, byte _randomValue = 0, bool _isValid = true, byte _rotationIndex = 0)
    {
        gridPosition = _gridPosition;
        isOffset = _isOffset;
        tileType = _tileType;
        offsetColorID = _offsetColorID;
        isValid = _isValid;
        rotationIndex = _rotationIndex;
    }
}
