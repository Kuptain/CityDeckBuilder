using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    [HideInInspector] public GameObject terrainObject;
    [ReadOnly] public Vector2Int gridPosition;
    public TerrainData terrain;

    public GameObject fogOfWar_dense;
    public GameObject fogOfWar_visible;
    public GameObject outlineHover;
    public GameObject outlinePlayable;
    public GameObject outlineExplorable;
    public GameObject outlineEffect;
    public GameObject directionOutlines_parent;
    public GameObject notSafeHighlight;
    private bool isExplored;

    [SerializeField] private List<TileDirectionOutline> directionOutlines;
    [SerializeField] private Color _unexploredColor;
    [SerializeField] private MeshRenderer _renderer;

    public void Init(Vector2Int _gridPosition)
    {
        gridPosition = _gridPosition;
        //UpdateTileTypeVisual();
        DisableHighlight(null);
    }
    public void EnableHighlight(ICard card)
    {
        if (GridManager.Instance.TryGetTile(gridPosition, out Tile tile))
        {
            if (card.GetType() == CardType.Resource)
            {
                ResourceCard ressource = (ResourceCard)card;
                if (tile.currentBuilding != null && tile.isExplored)
                {
                    var buildingResourceCosts = tile.currentBuilding.GetCostsStillOpen();
                    if (ressource.CheckMatchingResources(buildingResourceCosts))
                    {
                        outlinePlayable.SetActive(true);
                        return;
                    }

                    if (!tile.currentBuilding.isConstructing && tile.currentBuilding.TryToGetBuildingEffect(BuildingEffect.triggerType.onCharacter, out BuildingEffect effect)
                     && ressource.data.TryToPayFor(effect.EffectCost) && !tile.currentBuilding.IsOnCooldown())
                    {
                        outlineEffect.SetActive(true);
                        return;
                    }
                }
            }
            if (card.GetType() == CardType.Resource)
            {
                if (!tile.isExplored && tile.isVisible && tile.isExplorable)
                {
                    outlineExplorable.SetActive(true);
                    return;
                }
            }

        }
    }

    public void DisableHighlight(ICard card)
    {
        outlinePlayable.SetActive(false);
        outlineEffect.SetActive(false);
        outlineExplorable.SetActive(false);
    }

    public void SetExploredVisual(bool newIsExplored, bool isVisible, bool isSafe)
    {
        isExplored = newIsExplored;

        fogOfWar_dense.SetActive(!isVisible);
        fogOfWar_visible.SetActive(!newIsExplored);
        notSafeHighlight.SetActive(!isSafe);
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
        }
    }

    public void CheckDirectionalOutlines()
    {
        // Gets all directions and stores them in a list
        List<GridManager.TileDirection> tileDirections = new List<GridManager.TileDirection>((GridManager.TileDirection[])System.Enum.GetValues(typeof(GridManager.TileDirection)));
        Tile tileNeighbor;
        if (GridManager.Instance.TryGetTile(gridPosition, out Tile myTile))
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
                        if (GridManager.Instance.TryGetTile(tileNeighbor.gridPosition, out Tile neighbour))
                        {
                            neighbour.isExplorable = true;
                            GridManager.Instance.ApplyTileChanges(neighbour);

                        }
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
        foreach (var d in directionOutlines)
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
    private Dictionary<SpriteRenderer, float> originalAlphas = new();
    public void UpdateTileTypeVisual()
    {
        Tile tile = GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)];
        int randomInt = 0;
        if (terrain != null)
            currentTileVisualType = terrain.visualType;

        if (currentTileVisualType.visualVariants.Count > 0)
        {
            if (terrainObject != null)
                Destroy(terrainObject);

            randomInt = Random.Range(0, currentTileVisualType.visualVariants.Count);
            terrainObject = Instantiate(currentTileVisualType.visualVariants[randomInt], transform);
        }
        if (terrainObject != null)
        {
            SpriteRenderer[] renderers = terrainObject.GetComponentsInChildren<SpriteRenderer>(true);


            foreach (SpriteRenderer renderer in renderers)
            {
                if (!originalAlphas.ContainsKey(renderer))
                {
                    originalAlphas[renderer] = renderer.color.a;
                }

                Color color = renderer.color;

                if (isExplored)
                {
                    color.a = originalAlphas[renderer] * 1f;
                }
                else if(tile.isExplorable)
                {
                    color.a = originalAlphas[renderer] * 0.4f;
                }
                else
                {
                    color.a = originalAlphas[renderer] * 0.12f;
                }

                renderer.color = color;
            }
        }
        _renderer.transform.position = transform.position + new Vector3(0, currentTileVisualType.tileOffsetY, 0);
        if (tile.tileType == Tile.TileType.Lake)
        {
            _renderer.enabled = false;
        }
        else
        {
            _renderer.enabled = true;
        }
        SetOffsetColor(TerrainManager.GetColor(tile.tileType));
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
