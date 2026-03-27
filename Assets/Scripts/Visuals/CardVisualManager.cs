using System.Collections.Generic;
using UnityEngine;

public class CardVisualManager : MonoBehaviour
{
    public Transform container;
    public GameObject cardPrefab;
    public List<CardVisuals> slots = new List<CardVisuals>();
    public bool mouseOverHand;

    public CardVisuals.CardStates cardState;

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

    }
    private void OnEnable()
    {
        if (cardState == CardVisuals.CardStates.Overview)
        {
            SetupOverview();
        }
    }
    private void OnDisable()
    {
        if (cardState == CardVisuals.CardStates.Overview)
        {
            foreach (var slot in slots)
            {
                Destroy(slot.gameObject);
            }
            slots.Clear();
        }
    }
    void SetupOverview()
    {
        foreach (var card in CardManager.instance.productionDeck)
        {
            Draw(card);
        }
    }
    void Draw(Card card, int handPosition = 0)
    {
        CardVisuals slot = AddVisualCard(handPosition);
        slot.Setup(card, cardState);
        slots.Add(slot);
    }

    void Discard(Card card)
    {
        foreach (CardVisuals slot in slots)
        {
            if (slot.card == card)
            {
                Destroy(slot.gameObject);
                slots.Remove(slot);
                return;
            }
        }
    }

    CardVisuals AddVisualCard(int position)
    {
        CardVisuals card = Instantiate(cardPrefab, container).GetComponent<CardVisuals>();
        card.transform.SetSiblingIndex(position);
        return card;
    }

    public void MouseOverHand(bool active)
    {
        mouseOverHand = active;
    }
}
