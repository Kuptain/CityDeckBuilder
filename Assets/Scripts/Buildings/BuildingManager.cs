using UnityEngine;
using System.Collections.Generic;
using static Tile;
using static Unity.Collections.Unicode;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    private GameObject previewBuilding;

    [HideInInspector] public BuildingData selectedBuilding;
    public Dictionary<int, GameObject> spawnedBuildings = new Dictionary<int, GameObject>(); // To save progress later
    public static BuildingManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate BuildingsDatabase detected.");
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }
    private void Update()
    {
        MouseInputRaycast();
    }

    void MouseInputRaycast()
    {
        if (Mouse.current != null)
        {
            var raycastHit = GroundRaycast();

            if (previewBuilding != null)
            {
                if (raycastHit.isGround)
                {
                    previewBuilding.transform.position = Vector3.Lerp(previewBuilding.transform.position, GridManager.Instance.SnapToGrid(raycastHit.hitPosition, 0.55f), 0.6f);
                }
                else
                {
                    previewBuilding.transform.position = Vector3.Lerp(previewBuilding.transform.position, raycastHit.hitPosition, 0.6f);
                }
               
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && previewBuilding != null)
            {
                Destroy(previewBuilding);

                if (raycastHit.isGround)
                {
                    if ((selectedBuilding.resourceCosts.Count > 0 && RessourceManager.instance.TryToSpendRessource(selectedBuilding.resourceCosts[0], 1))
                        || selectedBuilding.resourceCosts.Count == 0)
                    {
                        SpawnBuilding(GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition), selectedBuilding);
                    }
                }
            }

        }
    }
    private (Vector3 hitPosition, bool isGround) GroundRaycast()
    {
        Vector3 hitPosition = new Vector3();
        bool isGround = false;

        int layer_maskGround = LayerMask.GetMask("Ground");

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        // Try ground raycast first
        if (Physics.Raycast(ray, out hit, 1000f, layer_maskGround))
        {
            if (hit.transform.GetComponent<Ground>() != null)
            {
                hitPosition = hit.point;
                isGround = true;
            }
        }
        else
        {
            // Fallback if no ground hit
            Plane fallbackPlane = new Plane(Vector3.up, Camera.main.transform.position + Camera.main.transform.forward * 60f);

            if (fallbackPlane.Raycast(ray, out float distance))
            {
                hitPosition = ray.GetPoint(distance);
            }
        }

        return (hitPosition, isGround);
    }
    private void OnEnable()
    {
        BuildingButton.OnPressedBuildingUI += SpawnBuildingPreview;
    }

    private void OnDisable()
    {
        BuildingButton.OnPressedBuildingUI -= SpawnBuildingPreview;
    }
    private void SpawnBuildingPreview(BuildingData building)
    {
        selectedBuilding = building;

        var raycastHit = GroundRaycast();
        previewBuilding = Instantiate(building.prefab, raycastHit.hitPosition, Quaternion.identity);
        previewBuilding.transform.GetChild(0).gameObject.SetActive(false);
        previewBuilding.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SpawnBuilding(Vector2Int gridPosition, BuildingData buildingToSpawn)
    {
        GridManager gridManager = GridManager.Instance;
        if(GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
        {
            if (tile.currentBuilding == null)
            {
                Tile currentTile = gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)];

                GameObject spawnedBuilding = Instantiate(buildingToSpawn.prefab, gridManager.GridToWorldPosition(gridPosition), Quaternion.identity);
                spawnedBuilding.transform.GetChild(0).gameObject.SetActive(true);
                spawnedBuilding.transform.GetChild(1).gameObject.SetActive(false);

                currentTile.currentBuilding = buildingToSpawn;
                gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)] = currentTile;
                spawnedBuildings.Add(gridManager.GetIndex(gridPosition.x, gridPosition.y), spawnedBuilding);

                foreach (Tile _tile in GridManager.Instance.GetTilesInRange(gridPosition, 0))
                {
                    Tile tileInRange = TileVisualsManager.Instance.GetVisualTilelData(_tile.gridPosition);
                    if (tileInRange.tileType != TileType.Edge && tileInRange.tileType != TileType.Centre)
                    {
                        TileVisualsManager.Instance.HandleOnUpdateTileVisual(tileInRange.gridPosition, TileType.Default);
                    }
                    else
                    {
                        Debug.LogWarning($"No visual found for tile at {gridPosition}");
                    }
                }
            }
        }

     
    }
}
