using UnityEngine;

public class Ui_RessourceManager : MonoBehaviour
{
    public GameObject Ui_RessourcePrefab;
    public Transform UI_RessourceParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RessourceManager.OneNewRessource.AddListener(CreateRessourceUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateRessourceUI(RessourceType type)
    {
        GameObject gO = Instantiate(Ui_RessourcePrefab, UI_RessourceParent);
        gO.GetComponent<Ui_Resssource>().type = type;
        gO.transform.SetSiblingIndex((int)type-1);
    }
}
