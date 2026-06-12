using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public void CreateVisuals()
    {

    }

    #region debug
    [ContextMenu("test Setup")]
    public void testSetup()
    {
        SetupCard(new Character("Philip", "Hildebrandt", new Characterlibrary.Colorit(), 3));
    }

    public void Discard()
    {
        Destroy(gameObject);
    }

    CardType ICard.GetType()
    {
        return CardType.Character;
    }

    void onPointerDown(PointerDownEvent e)
    {
        Debug.Log("Character selected :" + target.FullName);
        InteractionManager.OnPickUpCard.Invoke(this);
    }

    void onPointerUp(PointerUpEvent e)
    {
        Debug.Log("Character deselected :" + target.FullName);
        InteractionManager.OnHoldCard.Invoke(this);
    }

    void onPointerCancle(PointerUpEvent e)
    {
        Debug.Log("Character used:" + target.FullName);

        InteractionManager.OnReleaseCard.Invoke(this);
    }

    #endregion
}
