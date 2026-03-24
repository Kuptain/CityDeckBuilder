using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardManager : Manager
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

    [Header("variables")]
    public int HandSize;
    public float cardSpeed = 5;
    [Header("Cards")]
    public List<Card> deck = new List<Card>(10);
    public List<Handslot> hand;
    public List<Card> productionDeck;
    private void Start()
    {
        TurnManager.OnEndTurn.AddListener(EndTurn);
        TurnManager.OnStartTurn.AddListener(StartTurn);
    }
    private void OnDestroy()
    {
        TurnManager.OnEndTurn.RemoveListener(EndTurn);
        TurnManager.OnStartTurn.RemoveListener(StartTurn);
    }
    private void Update()
    {
        UpdateHUD(); // Probably move somewhere else, only when the values are actually changed
        
    }
    private void UpdateHUD()
    {
        HUD.Instance.text_Deck.text = deck.Count.ToString();
        HUD.Instance.text_Production.text = productionDeck.Count.ToString();
    }
    public void StartTurn()
    {
        SendLog("Start Of Turn");

        for(int i = 0; i < hand.Count; i++)
        {
            if(hand[i].empty)
            {
                DrawCard(i);
            }
        }

        //DrawCards(HandSize - hand.Count); // Refill back to hand size
    }
    public void EndTurn()
    {
        SendLog("End Of Turn");
        //DiscardHand();
    }

    public void AddCardsToDeck(List<Card_Data> cards_Data)
    {
        for (int i = 0; i < cards_Data.Count; i++)
        {
            deck.Add(new Card(cards_Data[i]));
        }
    }

    public void AddCardsToHand(List<Card_Data> cards_Data)
    {
        SendError("addCardsToHand is not implemented yet");
    }
    public void AddCardsToProduction(List<Card_Data> cards_Data)
    {
        for (int i = 0; i < cards_Data.Count; i++)
        {
            productionDeck.Add(new Card(cards_Data[i]));
        }
    }

    public void ShuffleDeck()
    {
        List<Card> newDeck = new List<Card >();
        while (deck.Count > 0)
        {
            int RandomIndex = Random.Range(0, deck.Count);
            newDeck.Add(deck[RandomIndex]);
            deck.RemoveAt(RandomIndex);
        }
        deck = newDeck;
    }
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            DrawCard();
        }
    }
    void DrawCard(int index = 0)
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
            ProductionToDeck();
            if (deck.Count > 0)
            {
                index = Mathf.Clamp(index, 0, deck.Count - 1);
                hand.Add(deck[index]);
                OnDraw.Invoke(deck[index]);
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
        if (index < hand.Count && index >= 0)
        {
            OnDiscard.Invoke(hand[index]);
            hand[index] = null;
        }
    }
    public void DiscardCard(Card card)
    {
        if (hand.Contains(card))
        {
            OnDiscard.Invoke(card);
            hand[hand.IndexOf(card)] = null;
            hand.Remove(card);
        }
    }
    public void ProductionToDeck()
    {
        deck.AddRange(productionDeck);
        ShuffleDeck();
        //discardedCards.Clear();
    }


    #region test functions
    [ContextMenu("shuffle Deck")]
    void Test_Shuffle()
    {
        ShuffleDeck();
    }

    [ContextMenu("resshuffle Discard")]
    void Test_Reshuffle()
    {
        ProductionToDeck();
    }

    [ContextMenu("draw Hand")]
    void Test_DrawHand()
    {
        DrawCards(HandSize);
    }

    [ContextMenu("draw Card")]
    void Test_Draw()
    {
        DrawCard();
    }
    [ContextMenu("discard Card")]
    void Discard()
    {
        DiscardCard(0);
    }
    #endregion
}
public enum DeckType
{
    discardPile = 0,
    DeckType = 1,
    deck = 2
}