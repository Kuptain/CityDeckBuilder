using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : MonoBehaviour
{
    #region singleton
    public static CardManager instance;


   
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    #region events
    public static UnityEvent<Card> OnDraw = new UnityEvent<Card>();
    public static UnityEvent<Card> OnDiscard = new UnityEvent<Card>();
    #endregion


    private void Start()
    {

        TurnManager.OnEndTurn.AddListener(EndTurn);
    }
    [Header("variables")]
    public int HandSize;
    public float cardSpeed = 5;
    [Header("listen")]
    public List<Card> deck = new List<Card>(10);
    public List<Card> hand;
    public List<Card> discardedCards;


    public void EndTurn()
    {
        DiscardHand();
        DrawCards(HandSize);
    }


    public void ShuffleDeck()
    {
        List<Card> newDeck = new List<Card>();
        while ( deck.Count > 0)
        {
            int RandomIndex = Random.Range(0, deck.Count);
            newDeck.Add(deck[RandomIndex]);
            deck.RemoveAt(RandomIndex);
        }
        deck = newDeck;
    }

    public void DrawCards(int count)
    {
        for(int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard(int index = 0)
    {

        if (deck.Count > 0)
        {
            index = Mathf.Clamp(index, 0, deck.Count - 1);
            hand.Add(deck[index]);
            OnDraw.Invoke(deck[index]);
            deck.RemoveAt(index);
        }
        else
        {
            ReshuffleDiscard();
            if (deck.Count > 0)
            {
                index = Mathf.Clamp(index, 0, deck.Count - 1);
                hand.Add(deck[index]);
                deck.RemoveAt(index);
            }
            else
            {
                //add reaction to nno cards beeing drawn
                Debug.Log("No cards in the Deckk to Draw");
            }
        }
    }

    public void DiscardCard(int index = 0)
    {
        if(index<hand.Count && index >= 0)
        {
            discardedCards.Add(hand[index]);
            OnDiscard.Invoke(hand[index]);
            hand.RemoveAt(index);
        }
    }
    public void DiscardCard(Card card)
    {
        if (hand.Contains(card))
        {
            discardedCards.Add(card);
            OnDiscard.Invoke(card);
            hand.Remove(card);
        }
    }

    public void DiscardHand()
    {
        discardedCards.AddRange(hand);
        foreach(Card c in hand)
        {
            OnDiscard.Invoke(c);
        }
        hand.Clear();
    }

    public void ReshuffleDiscard()
    {
        deck.AddRange(discardedCards);
        ShuffleDeck();
        discardedCards.Clear();
    }

    #region test functions
    [ContextMenu("shuffle Deck")]
    public void Test_Shuffle()
    {
        ShuffleDeck();
    }

    [ContextMenu("resshuffle Discard")]
    public void Test_Reshuffle()
    {
        ReshuffleDiscard();
    }

    [ContextMenu("draw Hand")]
    public void Test_DrawHand()
    {
        DrawCards(HandSize);
    }

    [ContextMenu("draw Card")]
    public void Test_Draw()
    {
        DrawCard();
    }
    [ContextMenu("discard Card")]
    public void Discard()
    {
        DiscardCard(0);
    }
    [ContextMenu("discard Hand")]
    public void Test_DiscardHand()
    {
        DiscardHand();
    }
    #endregion
}
