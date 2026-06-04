using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardVisualManager : MonoBehaviour
{
    public Transform container;
    public GameObject cardPrefab;
    public List<CardVisuals> slots = new List<CardVisuals>();
    public bool mouseOverHand;

    public CardVisuals.CardStates cardState;

    [SerializeField] private GameObject cardPlaceholder;

    private void Start()
    {
        if (cardState == CardVisuals.CardStates.Hand)
        {
            CardManager.OnDraw.AddListener(Draw);
            CardManager.OnDiscard.AddListener(Discard);
        }
        else if (cardState == CardVisuals.CardStates.Overview)
        {
            // Whatever
        }

        cardPlaceholder = new GameObject("CardPlaceholder", typeof(RectTransform));
        //cardPlaceholder.transform.SetParent(container);

        cardPlaceholder.SetActive(false);

        RectTransform rect = cardPlaceholder.GetComponent<RectTransform>();
        rect.sizeDelta = cardPrefab.GetComponent<RectTransform>().sizeDelta;
    }
    private void OnEnable()
    {
        SetupOverview(cardState);
    }
    private void OnDisable()
    {
        if (cardState == CardVisuals.CardStates.Overview || cardState == CardVisuals.CardStates.Deck)
        {
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
            slots.Clear();
        }
    }
    void SetupOverview(CardVisuals.CardStates state)
    {
        if (cardState == CardVisuals.CardStates.Overview)
        {
            foreach (var card in CardManager.instance.discardPile)
            {
                //Draw(card);
            }
        }
        if (cardState == CardVisuals.CardStates.Deck)
        {
            foreach (var card in CardManager.instance.deck)
            {
                //Draw(card);
            }
        }
    }
    void Draw(ResourceCard card, int handPosition = 0)
    {
        CardVisuals slot = AddVisualCard(handPosition);
        slot.Setup(card, cardState);
        slots.Add(slot);
    }

    void Discard(ResourceCard card, bool wasPlayed)
    {
        foreach (CardVisuals slot in slots)
        {
            if (slot.card == card)
            {
                DestroyCardVisual(slot, wasPlayed);
                return;
            }
        }

    }

    void DestroyCardVisual(CardVisuals slot, bool wasPlayed)
    {
        if (wasPlayed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            StartCoroutine(slot.LerpPlay(mousePosition));
        }

        int index = slot.transform.GetSiblingIndex();
        Destroy(slot.gameObject);
        slots.Remove(slot);

        if (!slot.card.temporary)
        {
            cardPlaceholder.gameObject.SetActive(true);
            cardPlaceholder.transform.SetParent(container);
            cardPlaceholder.transform.SetSiblingIndex(index);
        }
    }
    CardVisuals AddVisualCard(int position)
    {
        //Debug.Log("CardVisualManager: AddVisualCard position: " + position);
        cardPlaceholder.gameObject.SetActive(false);
        cardPlaceholder.transform.SetParent(null);

        CardVisuals card = Instantiate(cardPrefab, container).GetComponent<CardVisuals>();
        card.transform.SetSiblingIndex(position);
        return card;
    }

    public void MouseOverHand(bool active)
    {
        mouseOverHand = active;
    }
}
