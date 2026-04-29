using UnityEngine;

public interface Iinteractable 
{
    public void StartHover(InteractionManager.BuildingOutlineStates state);
    public void StopHover();
    public void Click();
    public void PlayCardOnThis(RessourceCard card);
}
