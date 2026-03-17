using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float moveSmoothTime = 0.1f;

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float minZoom = 20f;
    public float maxZoom = 80f;

    [Header("References")]
    public Transform cameraTransform;

    private Vector2 moveInput;
    private float zoomInput;

    private Vector3 velocity;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<float>();
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    void HandleMovement()
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 targetPosition = transform.position + direction * moveSpeed * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            moveSmoothTime
        );
    }

    [SerializeField] private LayerMask groundMask; // assign in inspector

    private Vector3 offsetVelocity = Vector3.zero; // keep this in your class

    void HandleZoom()
    {
        if (Mathf.Abs(zoomInput) < 0.01f) return;

        // 1. Invert scroll: scroll up = zoom in
        float zoomDelta = -zoomInput * zoomSpeed * Time.deltaTime;

        // 2. Raycast from mouse to get the world point under cursor BEFORE zoom
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            return;

        Vector3 targetPoint = hit.point;

        // 3. Apply zoom along camera local offset
        Vector3 localPos = cameraTransform.localPosition;
        Vector3 dir = localPos.normalized;
        float distance = localPos.magnitude;

        float newDistance = Mathf.Clamp(distance + zoomDelta, minZoom, maxZoom);
        float appliedDelta = newDistance - distance;
        if (Mathf.Abs(appliedDelta) < 0.001f)
            return; // at min/max, stop

        cameraTransform.localPosition = dir * newDistance;

        // 4. Move CameraController only if zooming in
        if (appliedDelta < 0f) // zoom in
        {
            // Raycast again to see where the mouse points after zoom
            Ray newRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(newRay, out RaycastHit newHit, 1000f, groundMask))
            {
                Vector3 offset = targetPoint - newHit.point;
                offset.y = 0f; // only horizontal movement
                transform.position += offset;
            }
        }
    }
}