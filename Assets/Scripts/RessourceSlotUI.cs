using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RessourceSlotUI : MonoBehaviour
{
    public Card type;
    public TMP_Text text;
    public Image iconImage;
    public Image greyImage;

    private void Awake()
    {
        SetIcon(type.sprite);
    }
    public void SetIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
    }
    public void SetIcon(int spriteID)
    {
        /*
        iconImage.sprite = sprite;
        iconImage.enabled = sprite != null;
        */
    }
    public void SetGreyActive(bool state)
    {
        if (greyImage != null)
        {
            greyImage.enabled = state;
        }
    }
}
