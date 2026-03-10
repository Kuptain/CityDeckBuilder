using UnityEngine;
using UnityEngine.InputSystem;

public class Inputmanager : MonoBehaviour
{
    public InputActionMap inputMap;
    public static Vector2 mousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        mousePosition = Mouse.current.position.value;
    }

    public void SubscibeInputs()
    {
        
    }
    public void UnSubscibeInputs()
    {

    }
}
