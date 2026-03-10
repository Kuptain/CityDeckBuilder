using UnityEngine;
using UnityEngine.UI;

public class Handslot : MonoBehaviour
{
    public Card card;
    public Image image;
    RectTransform rect;
    bool selected;
    Vector2 startPosition;

    private void Start()
    {
        rect = image.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (selected)
        {

           Move();
        }
        else 
        {
            Moveback();
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
    void Moveback()
    {
        Vector2 targetPosition = Vector2.zero;
        rect.localPosition = Vector3.Lerp(rect.localPosition, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);
        if(Vector2.Distance(rect.localPosition,targetPosition) < 5)
        {
            rect.localPosition = targetPosition;
            startPosition = Vector2.zero;
        }
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
        
        selected = true;
        image.color = Color.white;
        startPosition = rect.position;
    }

    public void Deselect()
    {
        selected = false;
        image.color = Color.gray7;
        if(rect.localPosition.y > 50)
        {
            PlayCard();
        }
    }

    void PlayCard()
    {
        RessourceManager.instance.GetRessources(card.ressources);
        CardManager.instance.DiscardCard(card);
    }
    #endregion
}
