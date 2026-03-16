using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using static UnityEngine.UI.Image;
using UnityEngine.InputSystem;
using System;

public class Handslot : MonoBehaviour
{
    public Card card;
    public Image image;
    public GameObject arrowContainer;
    public RectTransform dragArrow;
    public RectTransform dragArrowTrail;
    public RectTransform rect;
    public GameObject highlight;
    public TMP_Text text_name;
    public TMP_Text text_description;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private Vector2 arrowOrigin;

    public CardHover stateHover;
    public CardHover stateSelect;
    public CardHover stateHoverOverview;
    private CardHover stateBase;

    bool selected;
    bool hovered;
    public enum CardStates { Hand, Overview }
    CardStates state;

    private void Start()
    {
        //rect = image.GetComponent<RectTransform>();
        canvas = HUD.Instance.canvas;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        arrowOrigin = dragArrowTrail.anchoredPosition;

        stateBase = new CardHover();
        stateBase.position = Vector2.zero;
        stateBase.scale = rect.localScale;
    }

    private void Update()
    {
        if (state == CardStates.Hand)
        {
            if (selected)
            {
                Move();
            }
            else if (hovered)
            {
                MoveHover();
            }
            else
            {
                Moveback();
            }
        }
        else if (state == CardStates.Overview)
        {
            if (hovered)
            {
                MoveHoverOverview();
            }
            else
            {
                Moveback();
            }

        }
    }


    public void Setup(Card _card, CardStates _newState)
    {
        if (_card != null)
        {
            image.sprite = _card.sprite;
            //image.color = Color.gray7;
            card = _card;
            text_name.text = _card.cardName;
            text_description.text = _card.cardDescription;
            state = _newState;
        }
        else
        {
            card = null;
            image.sprite = null;
            state = _newState;
        }
    }
    void MoveHover()
    {
        rect.localPosition = Vector3.Lerp(rect.localPosition, stateHover.position, CardManager.instance.cardSpeed * Time.deltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, stateHover.scale, CardManager.instance.cardSpeed * Time.deltaTime);
    }
    void MoveHoverOverview()
    {
        rect.localPosition = Vector3.Lerp(rect.localPosition, stateHoverOverview.position, CardManager.instance.cardSpeed * Time.deltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, stateHoverOverview.scale, CardManager.instance.cardSpeed * Time.deltaTime);
    }
    void Move()
    {
        //Vector2 targetPosition = Inputmanager.mousePosition;
        //rect.position = Vector3.Lerp(rect.position, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);

        rect.localPosition = Vector3.Lerp(rect.localPosition, stateSelect.position, CardManager.instance.cardSpeed * Time.deltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, stateSelect.scale, CardManager.instance.cardSpeed * Time.deltaTime);
        MoveArrow();
    }
    void Moveback()
    {
        //Vector2 targetPosition = Vector2.zero;
        rect.localPosition = Vector3.Lerp(rect.localPosition, stateBase.position, CardManager.instance.cardSpeed * Time.deltaTime);
        rect.localScale = Vector3.Lerp(rect.localScale, stateBase.scale, CardManager.instance.cardSpeed * Time.deltaTime);
        if (Vector2.Distance(rect.localPosition, stateBase.position) < 5)
        {
            rect.localPosition = stateBase.position;
            rect.localScale = stateBase.scale;
            //stateBase.position = Vector2.zero;
        }
    }
    void MoveArrow()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        dragArrow.position = mousePos;

        Vector2 dir = mousePos - (Vector2)dragArrowTrail.position;
        float distance = dir.magnitude;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dragArrowTrail.rotation = Quaternion.Euler(0, 0, angle);

        // Convert pixel distance to canvas units
        float canvasDistance = distance / canvas.scaleFactor;

        dragArrowTrail.sizeDelta = new Vector2(canvasDistance, dragArrowTrail.sizeDelta.y);
    }
    #region selection
    public void TryToSelect()
    {
        if (card.selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }

    public void SetHover(bool state)
    {
        hovered = state;
    }

    public void Select()
    {
        
        selected = true;
        //image.color = Color.white;
        highlight.SetActive(true);
        arrowContainer.SetActive(true);
        InteractionManager.OnPickUpCard.Invoke(card);
    }

    public void Deselect()
    {
        selected = false;
        //image.color = Color.gray7;
        highlight.SetActive(false);
        arrowContainer.SetActive(false);
        if (dragArrow.localPosition.y > 50)
        {
            PlayCard();
        }
    }

    void PlayCard()
    {
        Debug.Log("play card");
        InteractionManager.OnReleaseCard.Invoke(card);
        if (!CardManager.instance.hand.Contains(card))
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
[Serializable]
public class CardHover
{
    public Vector2 scale;
    public Vector2 position;
}
