using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionManager : Manager
{
    public RessourceCard activeCard;
    public static UnityEvent<RessourceCard> OnPickUpCard = new UnityEvent<RessourceCard>();
    public static UnityEvent<RessourceCard> OnHoldCard = new UnityEvent<RessourceCard>();
    public static UnityEvent<RessourceCard> OnReleaseCard = new UnityEvent<RessourceCard>();
    BuildingObject currentHoverBuilding;
    Tile currentHoverTile;
    bool isHoldingCard;

    public enum BuildingOutlineStates { Idle, Hover, Draggable, Clickable, Preview}
    private void Start()
    {
        OnPickUpCard.AddListener(PickUpCard);
        OnReleaseCard.AddListener(ReleaseCard);
        Inputmanager.OnInteract.AddListener(TryToInteract);
        currentHoverTile = GridManager.Instance.gridNullTile;
    }
    private void Update()
    {
        //HoverBuilding();
        HoverTile();
    }

    public void PickUpCard(RessourceCard card)
    {
        isHoldingCard = true;
        activeCard = card;
    }

    void TryToInteract()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        BuildingObject building;
        if (SearchForBuilding(out building,true))
        {
            building.Click();
        }
    }
    void HoverBuilding()
    {
        BuildingOutlineStates state = BuildingOutlineStates.Hover;
        if (isHoldingCard)
        {
            state = BuildingOutlineStates.Draggable; // also needs to find all current draggable buildings
            //return;
        }
        BuildingObject building;
        if (SearchForBuilding(out building))
        {
            if (currentHoverBuilding != null && currentHoverBuilding != building)
            {
                currentHoverBuilding.StopHover();
            }
            currentHoverBuilding = building;
            currentHoverBuilding.StartHover(state);
        }
        else if (currentHoverBuilding != null)
        {
            currentHoverBuilding.StopHover();
        }
    }
    BuildingObject lastHoveredBuilding;

    void HoverTile()
    {
        if (HUD.Instance.IsHoveringUI()) return;

        Tile tile;
        if (SearchForTile(out tile))
        {
            if (currentHoverTile.gridPosition != GridManager.Instance.gridNullTile.gridPosition && currentHoverTile.gridPosition != tile.gridPosition)
            {
                StopHoverTile();
            }
            currentHoverTile = tile;
            currentHoverTile.StartHover();
            if(tile.currentBuilding != null && tile.isExplored)
            {
                lastHoveredBuilding = tile.currentBuilding;
                UI_HoverTooltip.Instance.SelectBuilding(tile.currentBuilding);
            }
        }
        else if (currentHoverTile.gridPosition != GridManager.Instance.gridNullTile.gridPosition)
        {
            StopHoverTile();
        }
    }
    void StopHoverTile()
    {
        currentHoverTile.StopHover();
        if (lastHoveredBuilding != null
            && UI_HoverTooltip.Instance.TryHideTooltip(lastHoveredBuilding.GetInstanceID()))
        {
            lastHoveredBuilding = null;
        }
    }

    public void ReleaseCard(RessourceCard card)
    {
        isHoldingCard = false;
        BuildingObject building;
        if (ExploreTile(card))
        {
            // Explore Feedback
            return;
        }

        else if (SearchForBuilding(out building, true))
        {
            if (GridManager.Instance.TryGetTile(building.GetTile().gridPosition, out Tile newTile) && newTile.isExplored)
            {
                building.PlayCardOnThis(activeCard);
            }
        }

    }
    bool ExploreTile(RessourceCard card)
    {
        var raycastHit = GridManager.Instance.GroundRaycast();
        Vector2Int gridPosition = GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition);

        if (raycastHit.isGround)
        {
            if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
            {
                if (!tile.isExplored && tile.isVisible && tile.isExplorable && CardManager.instance.HasCardResource(card, ResourceType.person))
                {
                    tile.SetExploredState(true, true, true);
                    CardManager.instance.DiscardCard(activeCard, true);

                    return true;
                }
            }
        }
        return false;
    }
    bool SearchForTile(out Tile tile, bool sentdebugMessage = false)
    {
        var raycastHit = GridManager.Instance.GroundRaycast();
        if (!raycastHit.isGround)
        {
            tile = new Tile();
            return false;
        }
        var gridPosition = GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition);

        if (raycastHit.isGround && GridManager.Instance.TryGetTile(gridPosition, out tile))
        {
            return true;
        }
        else if (sentdebugMessage)
        {
            SendLog("no tile hit at:" + GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition));
        }
        tile = new Tile();
        return false;
    }
    bool SearchForBuilding(out BuildingObject building,bool sentdebugMessage = false)
    {
        var raycastHit = GridManager.Instance.GroundRaycast();

        if (raycastHit.isGround)
        {
            if (TryToGetBuilding(raycastHit.hitPosition, out building))
            {
                if (sentdebugMessage)
                    SendLog("building found :" + building.data + " at :" + building.GetTile().gridPosition);
                return true;
            }
            else if (sentdebugMessage)
            {
                SendLog("no building found at :" + GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition));
            }
        }

        building = null;
        return false;
    }

    bool TryToGetBuilding(Vector3 pos, out BuildingObject building)
    {
        Vector2Int gridPos = GridManager.Instance.WorldToGridPosition(pos);
        Tile tile;
        GridManager.Instance.TryGetTile(gridPos.x, gridPos.y, out tile);
        building = tile.currentBuilding;
        return building != null;
    }
}
