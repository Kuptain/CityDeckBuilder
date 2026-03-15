using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using static UnityEngine.UI.Image;
using UnityEngine.InputSystem;

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

    bool selected;
    Vector2 startPosition;

    private void Start()
    {
        //rect = image.GetComponent<RectTransform>();
        canvas = HUD.Instance.canvas;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        arrowOrigin = dragArrowTrail.anchoredPosition;
    }

    private void Update()
    {
        if (selected)
        {

           Move();
        }
        else 
        {
            Moveback();
        }
    }


    public void Setup(Card _card)
    {
        if (_card != null)
        {
            image.sprite = _card.sprite;
            //image.color = Color.gray7;
            card = _card;
            text_name.text = _card.cardName;
            text_description.text = _card.cardDescription;
        }
        else
        {
            card = null;
            image.sprite = null;
        }
    }
    void Move()
    {
        Vector2 targetPosition = Inputmanager.mousePosition;
        //rect.position = Vector3.Lerp(rect.position, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);
        MoveArrow();
    }
    void Moveback()
    {
        Vector2 targetPosition = Vector2.zero;
        rect.localPosition = Vector3.Lerp(rect.localPosition, targetPosition, CardManager.instance.cardSpeed * Time.deltaTime);
        if(Vector2.Distance(rect.localPosition,targetPosition) < 5)
        {
            rect.localPosition = targetPosition;
            startPosition = Vector2.zero;
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

        dragArrowTrail.sizeDelta = new Vector2(distance, dragArrowTrail.sizeDelta.y);

        //dragArrowTrail.position = arrowOrigin;
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


    public void Select()
    {
        
        selected = true;
        //image.color = Color.white;
        highlight.SetActive(true);
        arrowContainer.SetActive(true);
        startPosition = rect.position;
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
