using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InteractionManager : MonoBehaviour
{
    public Card activeCard;
    public static UnityEvent<Card> OnPickUpCard = new UnityEvent<Card>();
    public static UnityEvent<Card> OnHoldCard = new UnityEvent<Card>();
    public static UnityEvent<Card> OnReleaseCard = new UnityEvent<Card>();
    BuildingObject currentHoverBuilding;

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
        activeCard = card;
    }

    void TryToInteract()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        BuildingObject building;
        if (SearchForBuilding(out building))
        {
            building.Click();
        }
    }
    void HoverInteract()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        BuildingObject building;
        if (SearchForBuilding(out building))
        {
            if (currentHoverBuilding != null && currentHoverBuilding != building)
            {
                currentHoverBuilding.StopHover();
            }
            currentHoverBuilding = building;
            currentHoverBuilding.StartHover();
        }
        else if (currentHoverBuilding != null)
        {
            currentHoverBuilding.StopHover();
        }
    }


    public void ReleaseCard(Card card)
    {
        BuildingObject building;
        if (SearchForBuilding(out building))
        {
            building.Drag(activeCard);
        }
    }

    bool SearchForBuilding(out BuildingObject building)
    {
        Ray ray = Camera.main.ScreenPointToRay(Inputmanager.mousePosition);
        RaycastHit hit;
        int mask = LayerMask.GetMask("Ground");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            if (TryToGetBuilding(hit.point, out building))
            {
                return true;
            }
        }
        building = null;
        return false;
    }

    bool TryToGetBuilding(Vector3 pos, out BuildingObject building)
    {
        Vector2Int gridPos = GridManager.Instance.WorldToGridPosition(pos);
        Debug.Log(gridPos);
        Tile tile;
        GridManager.Instance.TryGetTile(gridPos.x, gridPos.y, out tile);
        building = tile.currentBuilding;
        return building != null;
    }
}
