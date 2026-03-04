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

    void CreateRessourceUI(Ressource ressource)
    {
        GameObject gO = Instantiate(Ui_RessourcePrefab, UI_RessourceParent);
        gO.GetComponent<Ui_Resssources>().ressource = ressource;
        gO.transform.SetSiblingIndex((int)ressource.type-1);
    }
}
