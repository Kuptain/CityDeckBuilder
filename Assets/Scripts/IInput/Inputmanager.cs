using UnityEngine;
using UnityEngine.InputSystem;

public class Inputmanager : MonoBehaviour
{
    public InputActionMap inputMap;
    public static Vector2 mousePosition;
    public static Vector3 MouseWorldPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mousePosition = Mouse.current.position.value;
        MouseWorldPosition = Camera.main.cameraToWorldMatrix * mousePosition;
    }

    public void SubscibeInputs()
    {
        
    }
    public void UnSubscibeInputs()
    {

    }
}
