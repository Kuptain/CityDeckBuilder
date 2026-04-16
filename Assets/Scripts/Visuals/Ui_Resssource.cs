using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Ui_Resssource : MonoBehaviour
{
    public ResourceType type;
    public TMP_Text textfield;

    private void Update()
    {
        ChangeText();
    }


    public void ChangeText()
    {
        //string text = type.ToString() + ": " + ResourceManager.instance.GetRessourceCount(type);
        //textfield.text = text;
    }

}
