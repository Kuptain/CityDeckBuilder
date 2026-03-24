using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionManager : Manager
{
    public Card activeCard;
    public static UnityEvent<Card> OnPickUpCard = new UnityEvent<Card>();
    public static UnityEvent<Card> OnHoldCard = new UnityEvent<Card>();
    public static UnityEvent<Card> OnReleaseCard = new UnityEvent<Card>();
    BuildingObject currentHoverBuilding;
    bool isHoldingCard;

    public enum BuildingOutlineStates { Idle, Hover, Draggable, Clickable}
    private void Start()
    {
        OnPickUpCard.AddListener(PickUpCard);
        OnReleaseCard.AddListener(ReleaseCard);
        Inputmanager.OnInteract.AddListener(TryToInteract);
    }
    private void Update()
    {
        HoverInteract();
    }

    public void PickUpCard(Card card)
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
    void HoverInteract()
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


    public void ReleaseCard(Card card)
    {
        isHoldingCard = false;
        BuildingObject building;
        if (IsUnexplored())
        {
            // Any card works for now
            return;
        }
        else if (SearchForBuilding(out building,true))
        {
            building.PlayCardOnThis(activeCard);
        }

    }
    bool IsUnexplored()
    {
        var raycastHit = BuildingManager.Instance.GroundRaycast();
        Vector2Int gridPosition = GridManager.Instance.WorldToGridPosition(raycastHit.hitPosition);

        if (raycastHit.isGround)
        {
            if (GridManager.Instance.TryGetTile(gridPosition.x, gridPosition.y, out Tile tile))
            {
                if (!tile.isExplored)
                {
                    tile.SetExplored(true, true);
                    return true;
                }
            }
        }
        return false;
    }

    bool SearchForBuilding(out BuildingObject building,bool sentdebugMessage = false)
    {
       
        Ray ray = Camera.main.ScreenPointToRay(Inputmanager.mousePosition);
        RaycastHit hit;
        int mask = LayerMask.GetMask("Ground");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            if (TryToGetBuilding(hit.point, out building))
            {
                if(sentdebugMessage)
                SendLog("building found :" + building.data + " at :"+ building.GetTile().gridPosition);
                return true;
            }
            else if (sentdebugMessage)
            {
                SendLog("no building found at :" + GridManager.Instance.WorldToGridPosition(hit.point));
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
