using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Inputmanager : MonoBehaviour
{
    static InputMap map;
    public static Vector2 mousePosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Enable();
       

    }

    private void Enable()
    {
        if (map == null)
        {
            map = new InputMap();
        }
        map.Enable();
        SubscibeInputs();
    }
    private void Disable()
    {
       
        map.Disable();
        UnSubscibeInputs();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        mousePosition = Mouse.current.position.value;
    }

    public void SubscibeInputs()
    {
        map.Player.interact.performed += ctx => OnInteract.Invoke();
    }
    public void UnSubscibeInputs()
    {
        map.Player.interact.performed -= ctx => OnInteract.Invoke();
    }

    public static UnityEvent OnInteract = new UnityEvent();

}
