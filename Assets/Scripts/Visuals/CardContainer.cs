using System.Collections.Generic;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    public Transform container;
    public GameObject cardPrefab;
    public List<Handslot> slots = new List<Handslot>();
    public bool mouseOverHand;

    public Handslot.CardStates cardState;

    private void Start()
    {
        if (cardState == Handslot.CardStates.Hand)
        {
            CardManager.OnDraw.AddListener(Draw);
            CardManager.OnDiscard.AddListener(Discard);
        }
        else if (cardState == Handslot.CardStates.Overview)
        {
            // Whatever
        }

    }
    private void OnEnable()
    {
        if (cardState == Handslot.CardStates.Overview)
        {
            SetupOverview();
        }
    }
    private void OnDisable()
    {
        if (cardState == Handslot.CardStates.Overview)
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
    void Draw(Card card)
    {
        Handslot slot = AddHandSlot();
        slot.Setup(card, cardState);
        slots.Add(slot);
    }

    void Discard(Card card)
    {
        foreach (Handslot slot in slots)
        {
            if (slot.card == card)
            {
                Destroy(slot.gameObject);
                slots.Remove(slot);
                return;
            }
        }
    }

    Handslot AddHandSlot()
    {
        return Instantiate(cardPrefab, container).GetComponent<Handslot>();
    }

    public void MouseOverHand(bool active)
    {
        mouseOverHand = active;
    }
}
