using UnityEngine;

public struct Tile
{
    public enum TileType { Default, Centre, Edge, Forest, Mountain, Lake }

    [ReadOnly] public TileType tileType;
    [ReadOnly] public Vector2Int gridPosition;
    [ReadOnly] public bool isOffset;
    [ReadOnly] public bool isValid;

    [ReadOnly] public bool isExplored;
    [ReadOnly] public bool isExplorable;
    [ReadOnly] public bool isVisible;

    [ReadOnly] public byte rotationIndex;
    [ReadOnly] public byte offsetColorID;
    [ReadOnly] public BuildingObject currentBuilding;

    public void SetExploredState(bool state, bool enableInvisibleTiles = false, bool checkDirectionalOutlines = false)
    {
        isExplored = state;
        if (state == true)
        {
            SetVisibleState(true);
            if (currentBuilding != null)
            {
                currentBuilding.constructionUI.ToggleVisible(true);
            }
        }
        if (enableInvisibleTiles)
        {
            var tilesInRange = GridManager.Instance.GetTilesInRange(gridPosition, TileVisualsManager.Instance.tileVisibleRange);
            foreach(Tile tile in tilesInRange)
            {
                tile.SetVisibleState(true);
            }
        }

        // Apply this tile back to the gridArray
        GridManager.Instance.ApplyTileChanges(this);


        if (TileVisualsManager.Instance.tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            visual.SetExploredVisual(state, true);
            if (checkDirectionalOutlines)
            {
                visual.CheckDirectionalOutlines();

                // Also do this check for neighbors, so previous outlines get disabled
                var tilesInRange = GridManager.Instance.GetTilesInRange(gridPosition, 1);
                foreach (Tile tile in tilesInRange)
                {
                    if (TileVisualsManager.Instance.tileVisualMap.TryGetValue(tile.gridPosition, out TileVisual neighborVisual))
                    {
                        neighborVisual.CheckDirectionalOutlines();
                    }
                }
            }
        }
    }
    public void SetVisibleState(bool state)
    {
        isVisible = state;
        if (TileVisualsManager.Instance.tileVisualMap.TryGetValue(gridPosition, out TileVisual visual))
        {
            //visual.gameObject.SetActive(state);
            visual.SetExploredVisual(isExplored, state);
        }

        // Apply this tile back to the gridArray
        int index = GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y);
        GridManager.Instance.gridArray[index] = this;
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

    public void StartHover()
    {
        TileVisualsManager.Instance.GetVisualTilelData(gridPosition).outlineHover.SetActive(true);
    }
    public void StopHover()
    {
        TileVisualsManager.Instance.GetVisualTilelData(gridPosition).outlineHover.SetActive(false);
    }
}
