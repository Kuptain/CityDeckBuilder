using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ui_Resssource : MonoBehaviour
{
    public RessourceType type;
    public TMP_Text textfield;

    private void Update()
    {
        ChangeText();
    }


    public void ChangeText()
    {
        string text = type.ToString() + ": " + RessourceManager.instance.getRessourceCount(type);
        textfield.text = text;
    }

}
