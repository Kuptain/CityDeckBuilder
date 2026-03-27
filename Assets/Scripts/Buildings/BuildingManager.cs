using UnityEngine;
using System.Collections.Generic;
using static Tile;
using static Unity.Collections.Unicode;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class BuildingManager : Manager
{
    public BuildingDataContainer container;
    [SerializeField] GameObject buildingButtonPrefab;
    [SerializeField] List<BuildingData> unlockedBuildings;
    [SerializeField] List<BuildingData> lockedBuildings;
    [SerializeField] float previewBuildingSnapStrength = 0.5f;
    public BuildingData selectedBuilding { get; set; }
    public Dictionary<int, BuildingObject> spawnedBuildings = new Dictionary<int, BuildingObject>(); // To save progress later
    public Material outlineHover;
    public Material outlineDragCard;
    public Material outlineClickable;

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
        buildingsPanel = HUD.Instance.panelBuildingButtons;
        foreach (var building in unlockedBuildings)
        {
            GameObject buttonGO = Instantiate(buildingButtonPrefab, buildingsPanel);
            BuildingButton button = buttonGO.GetComponent<BuildingButton>();
            button.ChangeBuildingData(building);
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
                Destroy(previewBuilding);

                if (raycastHit.isGround)
                {
                    SpawnBuilding(GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition), selectedBuilding);
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
        selectedBuilding = building;

        var raycastHit = GridManager.Instance.GroundRaycast();
        previewBuilding = Instantiate(building.prefab, raycastHit.hitPosition, Quaternion.identity);
        previewBuilding.GetComponent<BuildingObject>().data = building;
        previewBuilding.transform.GetChild(0).gameObject.SetActive(false);
        previewBuilding.transform.GetChild(1).gameObject.SetActive(true);
    }

    public BuildingObject SpawnBuilding(Vector2Int gridPosition, BuildingData buildingToSpawn)
    {
        GridManager gridManager = GridManager.Instance;

        if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
        {
            if (tile.currentBuilding == null)
            {

                Tile currentTile = gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)];

                GameObject spawnedBuilding = Instantiate(buildingToSpawn.prefab, gridManager.GridToWorldPosition(gridPosition), Quaternion.identity);
                spawnedBuilding.transform.GetChild(0).gameObject.SetActive(true);

                var tileVisual = TileVisualsManager.Instance.GetVisualTilelData(currentTile.gridPosition);
                tileVisual.buildingObject = spawnedBuilding;

                //spawnedBuilding.transform.GetChild(1).gameObject.SetActive(false);
                var buildingObject = spawnedBuilding.GetComponent<BuildingObject>();
                //buildingObject.EnableOutline(); // Test outline

                currentTile.currentBuilding = spawnedBuilding.GetComponent<BuildingObject>();
                currentTile.currentBuilding.BuildingSetup(buildingToSpawn,currentTile);



                gridManager.gridArray[gridManager.GetIndex(gridPosition.x, gridPosition.y)] = currentTile;
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
        }

        return null;
    }



}
