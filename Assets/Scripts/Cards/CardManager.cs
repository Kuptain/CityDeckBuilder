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
    public static UnityEvent<ResourceCard, int> OnDraw = new UnityEvent<ResourceCard, int>();
    public static UnityEvent<ResourceCard, bool> OnDiscard = new UnityEvent<ResourceCard, bool>(); // bool = was this card played (true) or just discarded (false)
    public static UnityEvent OnShuffleDiscard = new UnityEvent();
    public static UnityEvent<ResourceCard> OnCardDecayed = new UnityEvent<ResourceCard>();
    #endregion

    [Header("variables")]
    public int handSize;
    public float cardSpeed = 5;
    public Card_Data population;
    public GameObject CharacterCardPrefab;

    [Header("Cards")]
    public List<Character> deck = new List<Character>(10);
    public List<CharacterCard> hand;
    public List<ResourceCard> temporaryHand;
    public List<Character> discardPile;
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
        HUD.Instance.text_Production.text = discardPile.Count.ToString();
    }
    [ContextMenu("start of Turn")]
    public void StartTurn()
    {
        SendLog("Start Of Turn");

        for (int i = 0; i < handSize; i++)
        {
            if (i >= hand.Count)
            {
               hand.Add(null);
            }

            if (hand[i] == null)
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
        for (int i = 0; i < temporaryHand.Count; i++)
        {
            temporaryHand[i].EndOfTurnInHand();
        }
    }

    public bool HasCardResource(ResourceCard card, ResourceType type)
    {
        foreach (var resource in card.data.ressources)
        {
            if (resource.resource == type)
            {
                return true;
            }
        }

        return false;
    }

    public void AddCardsToDeck(List<Character> cards)
    {
        deck.AddRange(cards);
    }

    public void AddCardsToHand(List<Card_Data> cards_Data)
    {
        SendError("addCardsToHand is not implemented yet");
    }

    public void AddCardsToDiscard(List<Character> cards)
    {
        discardPile.AddRange(cards);
    }


    public void ShuffleDeck()
    {
        List<Character> newDeck = new List<Character>();
        while (deck.Count > 0)
        {
            int RandomIndex = Random.Range(0, deck.Count);
            newDeck.Add(deck[RandomIndex]);
            deck.RemoveAt(RandomIndex);
        }
        deck = newDeck;
    }
    void DrawCard(int handPosition, int deckIndex = 0)
    {

        if (deck.Count <= 0)
        {
            ShuffleDiscardIntoDeck();
        }
        if (deck.Count > 0)
        {
            deckIndex = Mathf.Clamp(deckIndex, 0, deck.Count - 1);
            hand[handPosition] = CreateCharacterCard(deck[deckIndex]);
            deck.RemoveAt(deckIndex);
        }
        else
        {
            //add reaction to no cards beeing drawn
            SendError("No cards in the Deck to Draw");
        }

    }
    CharacterCard CreateCharacterCard(Character character)
    {
        CharacterCard card = Instantiate(CharacterCardPrefab).GetComponent<CharacterCard>();
        card.SetupCard(character);
        return card;
    }
    public void DiscardCard(int index = 0)
    {
        if (index < hand.Count && index >= 0)
        {
            discardPile.Add(hand[index].target);
            hand[index].Discard();
        }
    }
    public void DiscardCard(ICard card, bool wasPlayed = false)
    {
        if (card.GetType() == CardType.Character)
        {
            CharacterCard character = (CharacterCard)card;
            SendLog("discard " + character.target.FullName);
            hand.Remove((CharacterCard) card);
            character.Discard();
            TurnManager.OnEndTurn.Invoke();
        }

    }
    public void ShuffleDiscardIntoDeck()
    {
        for (int i = 0; i < discardPile.Count; i++)
        {
            deck.Add(discardPile[i]);
        }
        ShuffleDeck();
        //discardedCards.Clear();
        OnShuffleDiscard.Invoke();
    }

    public void GetTemporaryCard(Card_Data data)
    {
        ResourceCard newCard = new ResourceCard(data);
        newCard.temporary = true;
        temporaryHand.Add(newCard);
        OnDraw.Invoke(newCard, handSize + temporaryHand.Count - 1);
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
        ShuffleDiscardIntoDeck();
    }

    [ContextMenu("discard Card")]
    void Discard()
    {
        DiscardCard(0);
    }
    [ContextMenu("Get Deck")]
    void GetDeck()
    {
        deck = CharacterManager.characters;
    }
    #endregion
}
public enum DeckType
{
    discardPile = 0,
    DeckType = 1,
    deck = 2
}