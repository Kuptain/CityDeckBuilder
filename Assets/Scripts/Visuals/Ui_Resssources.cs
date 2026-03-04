using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ui_Resssources : MonoBehaviour
{
    public Ressource ressource;
    public TMP_Text textfield;

    private void Update()
    {
        ChangeText();
    }


    public void ChangeText()
    {
        string text = ressource.type.ToString() + ": " + ressource.amount;
        textfield.text = text;
    }

}
