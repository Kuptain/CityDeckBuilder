using UnityEngine;

[System.Serializable]
public class Handslot
{
    public bool empty = true;
    public ResourceCard currentCard;

    public Handslot()
    {
        TurnManager.OnEndTurn.AddListener(OnEndOfTurn);
    }


    public void Discard(bool wasPlayed)
    {
        CardManager.OnDiscard.Invoke(currentCard, wasPlayed);
        currentCard = null;
        empty = true;
    }

    public void DrawCard(ResourceCard card,int index)
    {
        currentCard = card;
        empty = false;
        CardManager.OnDraw.Invoke(card,index);
    }

    public void OnEndOfTurn()
    {
        if (currentCard != null)
        {
            currentCard.EndOfTurnInHand();
        }
    }
}
