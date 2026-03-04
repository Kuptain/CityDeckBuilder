using UnityEngine;
using UnityEngine.UI;

public class Handslot : MonoBehaviour
{
    public Card card;
    public Image image;
    public bool selected;
    RectTransform rect;

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
        Vector2 targetPosition = Input.mousePosition;
        rect.position += Vector3.Lerp(rect.position, targetPosition, CardManager.instance.cardSpeed* Time.deltaTime);
    }

    #region selection
    public void TryToSelect()
    {
        if (selected)
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
        RessourceManager.instance.GetRessources(card.ressource, card.amount);
        selected = true;
        image.color = Color.white;
    }

    public void Deselect()
    {
        RessourceManager.instance.TryToSpendRessource(card.ressource, card.amount);
        selected = false;
        image.color = Color.gray7;
    }
    #endregion
}
