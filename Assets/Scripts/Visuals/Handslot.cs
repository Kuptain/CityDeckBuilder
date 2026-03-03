using UnityEngine;
using UnityEngine.UI;

public class Handslot : MonoBehaviour
{
    public Card card;
    public Image image;

    public void Setup(Card _card)
    {
        if (_card != null) 
        {
            image.sprite = _card.sprite;
            card = _card;
        }
        else
        {
            card = null;
            image.sprite = null;
        }
    }

    public void Select()
    {

    }
}
