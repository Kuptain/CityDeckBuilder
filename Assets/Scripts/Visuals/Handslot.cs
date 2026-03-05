using UnityEngine;
using UnityEngine.UI;

public class Handslot : MonoBehaviour
{
    public Card card;
    public Image image;
    RectTransform rect;
    bool selected;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (selected)
        {

           Move();
        }
    }


    public void Setup(Card _card)
    {
        if (_card != null)
        {
            image.sprite = _card.sprite;
            image.color = Color.gray7;
            card = _card;
        }
        else
        {
            card = null;
            image.sprite = null;
        }
    }
    void Move()
    {
        Vector2 targetPosition = Inputmanager.mousePosition;
        rect.position = Vector3.Lerp(rect.position, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);
    }

    #region selection
    public void TryToSelect()
    {
        if (card.selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }


    public void Select()
    {
        RessourceManager.instance.GetRessources(card.ressources);
        selected = true;
        image.color = Color.white;
    }

    public void Deselect()
    {
        RessourceManager.instance.RemoveRessources(card.ressources);
        selected = false;
        image.color = Color.gray7;
    }
    #endregion
}
