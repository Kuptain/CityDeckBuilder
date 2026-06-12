using UnityEngine;
using UnityEngine.Events;

public class SelectionArrow : MonoBehaviour
{
    public static UnityEvent<Vector2> OnActivate = new UnityEvent<Vector2>();
    public static UnityEvent onDeactivate = new UnityEvent();

    [SerializeField] RectTransform targetPoint;
    [SerializeField] RectTransform trail;
    private Canvas canvas;

    private void Start()
    {
        canvas = HUD.Instance.canvas;
        OnActivate.AddListener(Activate);
        onDeactivate.AddListener(SetDeactive);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        MoveArrow();
    }
    void Activate(Vector2 origin)
    {
        gameObject.SetActive(true);
        transform.position = origin;

    }

    void SetDeactive()
    {
        gameObject.SetActive(false);
    }

    void MoveArrow()
    {
        targetPoint.position = Inputmanager.mousePosition;

        Vector2 dir = Inputmanager.mousePosition - (Vector2)trail.position;
        float distance = dir.magnitude;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        trail.rotation = Quaternion.Euler(0, 0, angle);

        // Convert pixel distance to canvas units
        float canvasDistance = distance / canvas.scaleFactor;

        trail.sizeDelta = new Vector2(canvasDistance, trail.sizeDelta.y);
    }
}
