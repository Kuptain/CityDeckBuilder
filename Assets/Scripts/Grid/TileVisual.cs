using System.Collections.Generic;
using UnityEngine;
using static Tile;

public class TileVisual : MonoBehaviour
{
    public List<GameObject> visualsWood, visualsStone;
    public GameObject fogOfWar;
    public GameObject buildingObject;
    public Vector2Int gridPosition;

    [SerializeField] private Color _baseColor1, _baseColor2, _baseColor3, _centreColor, _edgeColor, _unexploredColor;
    [SerializeField] private MeshRenderer _renderer;

    public void Init(Vector2Int _gridPosition)
    {
        gridPosition = _gridPosition;
        UpdateTileTypeVisual();
    }
    public void SetExploredVisual(bool isExplored)
    {
        fogOfWar.SetActive(!isExplored);
        if (buildingObject != null )
        {
            buildingObject.SetActive(isExplored);
        }

        if (isExplored)
        {
            SetOffsetColor();
        }
        else
        {
            _renderer.material.SetColor("_BaseColor", _unexploredColor);
        }
    }
    private void SetOffsetColor()
    {
        Tile tile = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)];

        switch (tile.offsetColorID)
        {
            case 1:
                _renderer.material.SetColor("_BaseColor", _baseColor1);
                break;
            case 2:
                _renderer.material.SetColor("_BaseColor", _baseColor2);
                break;
            case 3:
                _renderer.material.SetColor("_BaseColor", _baseColor3);
                break;
        }
    }
    public void UpdateTileTypeVisual()
    {
        Tile tile = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)];

        SetOffsetColor();
        foreach (var visual in visualsWood)
        {
            visual.SetActive(false);
        }
        foreach (var visual in visualsStone)
        {
            visual.SetActive(false);
        }

        switch (tile.tileType)
        {
            case TileType.Forest:
                break;
            case TileType.Mountain:
                break;
            case TileType.Centre:
                _renderer.material.SetColor("_BaseColor", _edgeColor);
                break;
            case TileType.Edge:
                _renderer.material.SetColor("_BaseColor", _edgeColor);
                break;

        }
    }
}
