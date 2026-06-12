using UnityEngine;
using UnityEngine.UIElements;

public class Selectable : PointerManipulator
{
    EventCallback<PointerDownEvent> onPointerDown;
    EventCallback<PointerUpEvent> onPointerUp;
    EventCallback<PointerUpEvent> onCancle;
    bool isPointerOverMe;

    public Selectable(EventCallback<PointerDownEvent> _onPointerDown, EventCallback<PointerUpEvent> _onPointerUp, EventCallback<PointerUpEvent> _onCancle)
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        onPointerDown = _onPointerDown;
        onPointerUp = _onPointerUp;
        onCancle = _onCancle;
        isPointerOverMe = false;
    }


    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.RegisterCallback<PointerOutEvent>(OnPointerExit);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.UnregisterCallback<PointerOutEvent>(OnPointerExit);
    }

    protected void OnPointerEnter(PointerEnterEvent e)
    {
        isPointerOverMe = true;
    }

    protected void OnPointerExit(PointerOutEvent e)
    {
        isPointerOverMe = false;
    }



    protected void OnPointerDown(PointerDownEvent e)
    {

        if (CanStartManipulation(e))
        {
            onPointerDown.Invoke(e);
            e.StopPropagation();
        }
    }

    protected void OnPointerUp(PointerUpEvent e)
    {
        if (CanStopManipulation(e))
        {
            if (isPointerOverMe)
            {
                onPointerUp.Invoke(e);
            }
            else
            {
                onCancle.Invoke(e);
            }
            e.StopPropagation();
        }
    }
}
