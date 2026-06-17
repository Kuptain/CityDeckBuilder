using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class CharacterCard : MonoBehaviour, ICard
{
    public Character target;
    [SerializeField] UIDocument uiDocument;

    public void SetupCard(Character character)
    {
        Selectable select = new Selectable(onPointerDown, onPointerUp, onPointerCancle);
        target = character;
        uiDocument.rootVisualElement.Q<VisualElement>("Card").dataSource = this;
        uiDocument.rootVisualElement.AddManipulator(select);
    }
    
    public void Discard(bool wasPlayed)
    {
        CardManager.OnDiscard.Invoke(this, wasPlayed);
        Destroy(gameObject);
    }

    CardType ICard.GetType()
    {
        return CardType.Character;
    }

    void onPointerDown(PointerDownEvent e)
    {
       
        InteractionManager.OnPickUpCard.Invoke(this);
        SelectionArrow.OnActivate.Invoke(Camera.main.WorldToScreenPoint(transform.position));
    }

    void onPointerUp(PointerUpEvent e)
    {
        InteractionManager.OnHoldCard.Invoke(this);
        SelectionArrow.onDeactivate.Invoke();
    }

    void onPointerCancle(PointerUpEvent e)
    {
        InteractionManager.OnReleaseCard.Invoke(this);
        SelectionArrow.onDeactivate.Invoke();
    }

    #region debug
    [ContextMenu("test Setup")]
    public void testSetup()
    {
        SetupCard(new Character("Philip", "Hildebrandt", new Characterlibrary.Colorit(), 3));
    }

  
    #endregion
}
