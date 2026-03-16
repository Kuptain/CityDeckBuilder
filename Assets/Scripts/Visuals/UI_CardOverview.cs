using System.Collections.Generic;
using UnityEngine;

public class UI_CardOverview : MonoBehaviour
{
    public Transform parent;
    public GameObject cardPrefab;
    public List<Handslot> slots = new List<Handslot>();
    public bool mouseOverHand;

    private void OnEnable()
    {
        foreach(var card in CardManager.instance.productionDeck)
        {
            SpawnCard(card);
        }
    }
    private void OnDisable()
    {
        foreach (var slot in slots)
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
    }

    void SpawnCard(Card card)
    {
        Handslot slot = Instantiate(cardPrefab, parent).GetComponent<Handslot>();
        slot.Setup(card, Handslot.CardStates.Overview);
        slots.Add(slot);
    }

    void Remove(Card card)
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

    public void MouseOverHand(bool active)
    {
        mouseOverHand = active;
    }
}
