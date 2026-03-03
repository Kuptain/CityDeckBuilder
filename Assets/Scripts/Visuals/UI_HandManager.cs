using System.Collections.Generic;
using UnityEngine;

public class UI_HandManager : MonoBehaviour
{
    public Transform HandParent;
    public GameObject HandSlotPrefab;
    public List<Handslot> slots = new List<Handslot>();

    private void Start()
    {
        CardManager.OnDraw.AddListener(Draw);
        CardManager.OnDiscard.AddListener(Discard);
    }

    void Draw(Card card)
    {
        Handslot slot = AddHandSlot();
        slot.Setup(card);
        slots.Add(slot);
    }

    void Discard(Card card)
    {
        foreach (Handslot slot in slots)
        {
            if(slot.card == card)
            {
                Destroy(slot.gameObject);
                slots.Remove(slot);
                return;
            }
        }
    }

    Handslot AddHandSlot()
    {
        return Instantiate(HandSlotPrefab, HandParent).GetComponent<Handslot>();
    }
}
