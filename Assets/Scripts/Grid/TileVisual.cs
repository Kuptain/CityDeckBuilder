using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.UIElements.ToolbarMenu;

[System.Serializable]
class TileDirectionOutline
{
    public GameObject outline;
    public GridManager.TileDirection direction;
}
public class TileVisual : MonoBehaviour
{
    [HideInInspector] public GameObject buildingObject;
    [HideInInspector] public TileVisualType currentTileVisualType;
    [ReadOnly] public Vector2Int gridPosition;

    public List<TileVisualType> tileVisualTypes;
    public GameObject fogOfWar_dense;
    public GameObject fogOfWar_visible;
    public GameObject outlineHover;
    public GameObject outlinePlayable;
    public GameObject outlineEffect;
    public GameObject directionOutlines_parent;

    [SerializeField] private List<TileDirectionOutline> directionOutlines;
    [SerializeField] private Color _unexploredColor;
    [SerializeField] private MeshRenderer _renderer;

    public void Init(Vector2Int _gridPosition)
    {
        gridPosition = _gridPosition;
        UpdateTileTypeVisual();
        DisableHighlight(null);
    }
    public void EnableHighlight(Card card)
    {
        if (GridManager.Instance.TryGetTile(gridPosition, out Tile tile))
        {
            if (!tile.isExplored && tile.isVisible)
            {
                outlinePlayable.SetActive(true);
                return;
            }

            if (tile.currentBuilding != null)
            {
                var buildingResourceCosts = tile.currentBuilding.GetCostsStillOpen();
                if (card.CheckMatchingResources(buildingResourceCosts))
                {
                    outlinePlayable.SetActive(true);
                    return;
                }

                if (tile.currentBuilding.TryToGetBuildingEffect(BuildingEffect.triggerType.onCard, out BuildingEffect effect)
                 && card.data.TryToPayFor(effect.EffectCost))
                {
                    outlineEffect.SetActive(true);
                    return;
                }
            }
        }
    }

    public void DisableHighlight(Card card)
    {
        outlinePlayable.SetActive(false);
        outlineEffect.SetActive(false);
    }

    public void SetExploredVisual(bool isExplored, bool isVisible)
    {
        fogOfWar_dense.SetActive(!isVisible);
        fogOfWar_visible.SetActive(!isExplored);
        if (buildingObject != null)
        {
            buildingObject.SetActive(isVisible);
        }

        if (isVisible)
        {
            UpdateTileTypeVisual();
        }
        else
        {
            SetOffsetColor(_unexploredColor);
            foreach (var visualVariant in currentTileVisualType.visualVariants) // Disable all variants
            {
                visualVariant.SetActive(false);
            }
        }
    }
    public void CheckDirectionalOutlines()
    {
        // Gets all directions and stores them in a list
        List<GridManager.TileDirection> tileDirections = new List<GridManager.TileDirection>((GridManager.TileDirection[])System.Enum.GetValues(typeof(GridManager.TileDirection)));
        Tile tileNeighbor;
        if(GridManager.Instance.TryGetTile(gridPosition, out Tile myTile))
        {
            if (!myTile.isValid) return;

            foreach (var direction in tileDirections)
            {
                if (myTile.isExplored)
                {
                    tileNeighbor = GridManager.Instance.ReturnDirectionalTile(gridPosition, direction, 1);
                    if (!tileNeighbor.isExplored)
                    {
                        ToggleDirectionOutline(direction, true);
                    }
                    else
                    {
                        ToggleDirectionOutline(direction, false);
                    }
                }
                else
                {
                    ToggleDirectionOutline(direction, false);
                }
            }
        }
    }
    void ToggleDirectionOutline(GridManager.TileDirection direction, bool state)
    {
        foreach(var d in directionOutlines)
        {
            if (d.direction == direction)
            {
                d.outline.SetActive(state);
            }
        }
    }
    private void SetTileColor(Color color)
    {
        _renderer.material.SetColor("_BaseColor", color);
    }
    public void UpdateTileTypeVisual()
    {
        Tile tile = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)];
        int randomInt = 0;

        foreach (var visualType in tileVisualTypes)
        {
            foreach (var visualVariant in visualType.visualVariants) // Disable all variants first
            {
                visualVariant.SetActive(false);
            }
            if (tile.tileType == visualType.type)
            {
                currentTileVisualType = visualType;
                randomInt = Random.Range(0, visualType.visualVariants.Count);
                visualType.visualVariants[randomInt].SetActive(true);
                SetOffsetColor(visualType.color);
            }
        }
    }
    private void SetOffsetColor(Color color)
    {
        Tile tile = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)];
        float colorShade_2 = 1.03f;
        float colorShade_3 = 0.97f;

        switch (tile.offsetColorID)
        {
            case 1:
                SetTileColor(color);
                break;
            case 2:
                SetTileColor(new Color(color.r * colorShade_2, color.g * colorShade_2, color.b * colorShade_2));
                break;
            case 3:
                SetTileColor(new Color(color.r * colorShade_3, color.g * colorShade_3, color.b * colorShade_3));
                break;
        }
    }
}
