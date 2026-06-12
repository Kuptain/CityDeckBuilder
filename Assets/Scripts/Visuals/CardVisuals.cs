using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using static UnityEngine.UI.Image;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class CardVisuals : MonoBehaviour
{
    public  ResourceCard card;
    [Header("Card")]
    [SerializeField] Image BGImage;
    [SerializeField] Image IconImage;
    [SerializeField] RectTransform cardRect;
    [SerializeField] TMP_Text text_name;
    [SerializeField] TMP_Text text_description;
    [Header("Arrow")]
    [SerializeField] GameObject arrowContainer;
    [SerializeField] RectTransform dragArrow;
    [SerializeField] RectTransform dragArrowTrail;
    [Header("highlight")]
    [SerializeField] GameObject highlight;

    [Header("States")]
    [SerializeField] CardHover stateHover;
    [SerializeField] CardHover stateSelect;
    [SerializeField] CardHover stateHoverOverview;

    [Header("CardEffects")]
    [SerializeField] GameObject decayVisuals;
    [SerializeField] TMP_Text decayCount;

    private CardHover stateBase;
    private Canvas canvas;
    private Vector2 arrowOrigin;
    private bool selected;
    private bool hovered;
    private bool isDrawn;
    private bool isPlayed;

    public enum CardStates { Hand, Overview, Deck }
    CardStates state;

    private void Start()
    {
        //rect = image.GetComponent<RectTransform>();
        canvas = HUD.Instance.canvas;
        arrowOrigin = dragArrowTrail.anchoredPosition;

        stateBase = new CardHover();
        stateBase.position = Vector2.zero;
        stateBase.scale = cardRect.localScale;

        if (state == CardStates.Hand)
            StartCoroutine(LerpDraw());
        if (card.temporary)
        {
            Image[] images = BGImage.GetComponentsInChildren<Image>();
            foreach (Image i in images)
            {
                i.color = new Color(i.color.r,i.color.g,i.color.b+.2f, .8f);
            }
        }
    }

    private void Update()
    {
        if (state == CardStates.Hand)
        {
            if(isDrawn)
            {
                //cardRect.anchoredPosition = Vector2.Lerp(cardRect.anchoredPosition, GetComponent<RectTransform>().anchoredPosition, 0.02f);
                if(Vector2.Distance(GetComponent<RectTransform>().anchoredPosition, cardRect.anchoredPosition) < 10f)
                {
                    //isDrawn = false;
                    //cardRect.localPosition = Vector3.zero;
                }
            }
            else if (selected)
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

        if(card.data.decay != 0)
        {
            decayVisuals.SetActive(true);
            decayCount.text = card.GetDecayCount().ToString();
        }
        else
        {
            decayVisuals.SetActive(false);
        }
    }

    float drawDuration = 0.5f;
    float playDuration = 0.35f;
    float arcHeight = 150f;

    IEnumerator LerpDraw() // ChatGPT helped
    {
        isDrawn = true;
        cardRect.SetParent(HUD.Instance.gameObject.transform);

        // Start at deck
        cardRect.position = HUD.Instance.deckButtonTransform.position;

        Vector3 start = cardRect.position;

        float time = 0f;

        while (time < drawDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / drawDuration);

            // Base straight movement
            Vector3 pos = Vector3.Lerp(start, transform.position, t);

            // Add arc (Y axis)
            float arc = arcHeight * 4f * (t * (1f - t));
            pos.y += arc;

            cardRect.position = pos;

            // Scale animation
            cardRect.localScale = Vector3.Lerp(stateBase.scale * 0.6f, stateBase.scale, t);

            yield return null;
        }

        cardRect.position = transform.position;

        isDrawn = false;
        cardRect.SetParent(transform);
    }

    public IEnumerator LerpPlay(Vector2 targetPos) // ChatGPT helped
    {
        isPlayed = true;
        cardRect.SetParent(HUD.Instance.gameObject.transform);

        Vector3 start = cardRect.position;

        float time = 0f;

        while (time < playDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / playDuration);

            // Base straight movement
            Vector3 pos = Vector3.Lerp(start, targetPos, t);

            // Add arc (X axis)
            float arc = arcHeight * 4f * (t * (1f - t));
            pos.x += arc;

            cardRect.position = pos;

            // Scale animation
            cardRect.localScale = Vector3.Lerp(stateBase.scale, stateBase.scale * 0.1f, t);

            yield return null;
        }

        isPlayed = false;
        Destroy(cardRect.gameObject);
    }

    public void Setup(ResourceCard _card, CardStates _newState)
    {
        if (_card != null)
        {
            IconImage.sprite = _card.data.sprite;
            //image.color = Color.gray7;
            card = _card;
            text_name.text = _card.data.cardName;
            text_description.text = _card.data.cardDescription;
            state = _newState;
        }
        else
        {
            card = null;
            IconImage.sprite = null;
            state = _newState;
        }
        CardManager.OnDiscard.AddListener(DiscardCheck);
        CardManager.OnCardDecayed.AddListener(RefreshVisuals);
    }

    void RefreshVisuals(ResourceCard _card)
    {
        if (card == _card)
        {
            IconImage.sprite = card.data.sprite;
            text_name.text = card.data.cardName;
            text_description.text = card.data.cardDescription;
        }
    }

    void MoveHover()
    {
        cardRect.localPosition = Vector3.Lerp(cardRect.localPosition, stateHover.position, CardManager.instance.cardSpeed * Time.deltaTime);
        cardRect.localScale = Vector3.Lerp(cardRect.localScale, stateHover.scale, CardManager.instance.cardSpeed * Time.deltaTime);
    }
    void MoveHoverOverview()
    {
        cardRect.localPosition = Vector3.Lerp(cardRect.localPosition, stateHoverOverview.position, CardManager.instance.cardSpeed * Time.deltaTime);
        cardRect.localScale = Vector3.Lerp(cardRect.localScale, stateHoverOverview.scale, CardManager.instance.cardSpeed * Time.deltaTime);
    }
    void Move()
    {
        //Vector2 targetPosition = Inputmanager.mousePosition;
        //rect.position = Vector3.Lerp(rect.position, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);

        cardRect.localPosition = Vector3.Lerp(cardRect.localPosition, stateSelect.position, CardManager.instance.cardSpeed * Time.deltaTime);
        cardRect.localScale = Vector3.Lerp(cardRect.localScale, stateSelect.scale, CardManager.instance.cardSpeed * Time.deltaTime);
        MoveArrow();
    }
    void Moveback()
    {
        //Vector2 targetPosition = Vector2.zero;
        cardRect.localPosition = Vector3.Lerp(cardRect.localPosition, stateBase.position, CardManager.instance.cardSpeed * Time.deltaTime);
        cardRect.localScale = Vector3.Lerp(cardRect.localScale, stateBase.scale, CardManager.instance.cardSpeed * Time.deltaTime);
        if (Vector2.Distance(cardRect.localPosition, stateBase.position) < 5)
        {
            cardRect.localPosition = stateBase.position;
            cardRect.localScale = stateBase.scale;
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
        if (selected)
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
        highlight.SetActive(false);
        arrowContainer.SetActive(false);
        PlayCard();

        if (dragArrow.localPosition.y > 50 && !isPlayed)
        {
            //PlayCard();
        }
    }

    void PlayCard()
    {
        CardManager.instance.SendLog("play card");
        InteractionManager.OnReleaseCard.Invoke(card);
        
    }

    void DiscardCheck(ICard _card, bool wasPlayed)
    {
        if(_card == card && !wasPlayed)
        {
            Destroy(this);
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
