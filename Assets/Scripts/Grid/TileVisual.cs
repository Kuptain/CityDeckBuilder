using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.UIElements.ToolbarMenu;

public class TileVisual : MonoBehaviour
{
    [HideInInspector] public GameObject buildingObject;
    [HideInInspector] public TileVisualType currentTileVisualType;

    public List<TileVisualType> tileVisualTypes;
    public GameObject fogOfWar;
    public Vector2Int gridPosition;
    [SerializeField] private Color _unexploredColor;
    [SerializeField] private MeshRenderer _renderer;

    public void Init(Vector2Int _gridPosition)
    {
        gridPosition = _gridPosition;
        UpdateTileTypeVisual();
    }
    public void SetExploredVisual(bool isExplored)
    {
        fogOfWar.SetActive(!isExplored);
        if (buildingObject != null)
        {
            buildingObject.SetActive(isExplored);
        }

        if (isExplored)
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

    private void SetTileColor(Color color)
    {
        _renderer.material.SetColor("_BaseColor", color);
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
}
