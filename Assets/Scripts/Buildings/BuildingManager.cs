using UnityEngine;
using System.Collections.Generic;
using static Tile;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BuildingManager : Manager
{
    [SerializeField] GameObject lockedBuildingParent;
    [SerializeField] GameObject buildingParent;
    public GameObject buildingConstructionUIPrefab;
    [SerializeField] GameObject buildingButtonPrefab;
    public BuildingData centreBuilding;
    public BuildingData blueprintBuilding;
    public ResourceCost blueprintCost;
    [SerializeField] bool unlockAllBuildings; // DEBUG
    [SerializeField] List<BuildingData> unlockedBuildings;
    public List<BuildingData> lockedBuildings;
    //[SerializeField] float previewBuildingSnapStrength = 0.5f;

    public BuildingData selectedBuilding { get; set; }
    public Dictionary<int, BuildingObject> spawnedBuildings = new Dictionary<int, BuildingObject>(); // To save progress later

    public Material outlineHover;
    public Material outlineDragCard;
    public Material outlineClickable;
    public Material matPreviewBuilding;

    private GameObject previewBuilding;
    private Transform buildingsPanel;

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
    private void Start()
    {
        SetupInitialUnlockedBuildings();
    }

    void SetupInitialUnlockedBuildings()
    {
        buildingsPanel = HUD.Instance.panelBuildingButtons;
        foreach (var building in unlockedBuildings)
        {
            SpawnBuildingButton(building);
        }
        if (unlockAllBuildings)
        {
            foreach (var building in lockedBuildings)
            {
                SpawnBuildingButton(building);
            }
        }
    }

    public bool UnlockBuilding(BuildingData building)
    {
        Debug.Log("1) UnlockBuilding: " + building.buildingName);
        if (!unlockedBuildings.Contains(building) && lockedBuildings.Contains(building))
        {
            Debug.Log("2) UnlockBuilding: " + building.buildingName);
            SpawnBuildingButton(building);
            unlockedBuildings.Add(building);
            lockedBuildings.Remove(building);

            return true;
        }
        return false;
    }

    void SpawnBuildingButton(BuildingData building)
    {
        GameObject buttonGO = Instantiate(buildingButtonPrefab, buildingsPanel);
        BuildingButton button = buttonGO.GetComponent<BuildingButton>();
        button.ChangeBuildingData(building);
    }

    private void Update()
    {
        MouseInputRaycast();
    }

    void MouseInputRaycast()
    {
        if (Mouse.current != null)
        {
            var raycastHit = GridManager.Instance.GroundRaycast();

            if (previewBuilding != null)
            {
                if (raycastHit.isGround)
                {
                    //previewBuilding.transform.position = Vector3.Lerp(previewBuilding.transform.position, GridManager.Instance.SnapToGrid(raycastHit.hitPosition, previewBuildingSnapStrength), 0.6f);
                    previewBuilding.transform.position = Vector3.Lerp(previewBuilding.transform.position, raycastHit.hitTransform.position, 0.6f);
                }
                else
                {
                    previewBuilding.transform.position = Vector3.Lerp(previewBuilding.transform.position, raycastHit.hitPosition, 0.6f);
                }

            }

            if (Mouse.current.leftButton.wasReleasedThisFrame && previewBuilding != null)
            {
                if (raycastHit.isGround)
                {
                    SpawnBuilding(GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition), selectedBuilding, false, false);
                }
                else
                {
                    Destroy(previewBuilding);
                }
            }
        }
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
        HUD.Instance.EnterUI();
        selectedBuilding = building;

        var raycastHit = GridManager.Instance.GroundRaycast();
        previewBuilding = Instantiate(building.prefab, raycastHit.hitPosition, Quaternion.identity,buildingParent.transform);
        previewBuilding.GetComponent<BuildingObject>().BuildingPreviewSetup(building);
        //ToggleBuildingPreview(previewBuilding, true);
    }

    private void ToggleBuildingPreview(GameObject building, bool state)
    {
        if (building.TryGetComponent<BuildingObject>(out var buildingObject))
        {
            if (state)
            {
                buildingObject.EnablePreviewMaterials();
            }
            else
            {
                buildingObject.EnableOriginMaterials();
            }
        }
    }

    public BuildingObject SpawnBuilding(Vector2Int gridPosition, BuildingData buildingToSpawn, bool ignoreRestrains, bool isLocked)
    {
        GridManager gridManager = GridManager.Instance;

        if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
        {
            if (tile.currentBuilding == null && (ignoreRestrains || (!ignoreRestrains && tile.isExplored)))
            {
                
                BuildingObject buildingObject;

                if (previewBuilding == null)
                {
                    Vector3 worldPos = gridManager.GridToWorldPosition(gridPosition);
                    previewBuilding = Instantiate(buildingToSpawn.prefab, worldPos, Quaternion.identity,lockedBuildingParent.transform);

                    buildingObject = previewBuilding.GetComponent<BuildingObject>();
                    buildingObject.BuildingPreviewSetup(buildingToSpawn);
                }
                else
                {
                    HUD.Instance.ExitUI();
                    buildingObject = previewBuilding.GetComponent<BuildingObject>();
                }

                TileVisual tileVisual = TileVisualsManager.Instance.GetVisualTilelData(tile.gridPosition);
                tileVisual.buildingObject = previewBuilding;

                //InteractionManager.OnPickUpCard.AddListener(tileVisual.EnableHighlight);
                //InteractionManager.OnReleaseCard.AddListener(tileVisual.DisableHighlight);

                tile.currentBuilding = previewBuilding.GetComponent<BuildingObject>();
                tile.currentBuilding.BuildingSetup(buildingToSpawn, tile, isLocked);

                previewBuilding = null;

                // Apply Grid Array
                gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)] = tile;
                spawnedBuildings.Add(gridManager.GetIndex(gridPosition.x, gridPosition.y), buildingObject);


                foreach (Tile _tile in GridManager.Instance.GetTilesInRange(gridPosition, 0))
                {
                    //Tile tileInRange = TileVisualsManager.Instance.GetVisualTilelData(_tile.gridPosition);
                    GridManager.Instance.TryGetTile(_tile.gridPosition, out Tile tileInRange);
                    
                    if (tileInRange.tileType != TileType.Edge && tileInRange.tileType != TileType.Centre)
                    {
                        TileVisualsManager.Instance.HandleOnUpdateTileVisual(tileInRange.gridPosition, TileType.Default);
                    }
                    else
                    {
                        Debug.LogWarning($"No visual found for tile at {gridPosition}, tileType: " + tile.tileType);
                    }
                }
                return buildingObject; 
            }
            else
            {
                if (previewBuilding != null)
                {
                    Destroy(previewBuilding);
                    previewBuilding = null;
                    HUD.Instance.ExitUI();
                }

                return null;
            }
        }

        return null;
    }

    public void DestroyBuilding(Vector2Int gridPosition)
    {
        if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
        {
            if (tile.currentBuilding != null)
            {
                tile.currentBuilding =null;

                GridManager.Instance.gridArray[GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y)] = tile;
                spawnedBuildings.Remove(GridManager.Instance.GetIndex(gridPosition.x, gridPosition.y));
            }


        }
    }

}
