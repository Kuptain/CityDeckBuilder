using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterCard : MonoBehaviour, ICard
{
    public Character target;
    [SerializeField] UIDocument uiDocument;

    
    public void SetupCard(Character character)
    {
        target = character;
        uiDocument.rootVisualElement.Q<VisualElement>("Card").dataSource = this;
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


    #endregion
}
