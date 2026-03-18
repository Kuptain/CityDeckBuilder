using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionVisualizer : MonoBehaviour
{
    [SerializeField] BuildingObject building;
    [SerializeField] List<GameObject> objectProgression = new List<GameObject>();
    [SerializeField] Transform uiParent;
    [SerializeField] GameObject iconPrefab;
    List<GameObject> uiIcons = new List<GameObject>();
    public double progress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        building.OnConstructionProgress.AddListener(ChangeConstruction);
    }

    void ChangeConstruction(List<ResourceCost> constructionCost, List<ResourceCost> CostStillOpen)
    {
        float sumOfCosts = 0;
        for (int i = 0; i < constructionCost.Count; i++)
        {
            sumOfCosts += constructionCost[i].amount;
        }
        float sumOfOpenCosts = 0;
        for (int i = 0; i < CostStillOpen.Count; i++)
        {
            sumOfOpenCosts += CostStillOpen[i].amount;
        }

       

        progress = (sumOfCosts - sumOfOpenCosts)/sumOfCosts;
        ChangeObjects(progress);
        ChangeUI(CostStillOpen);
    }

    void ChangeObjects(double progess)
    {
        float visibleProgression = objectProgression.Count * (float)progess;

        for (int i = 0; i < objectProgression.Count; i++)
        {
            if (i < visibleProgression)
            {
                objectProgression[i].SetActive(true);
            }
            else
            {
                objectProgression[i].SetActive(false);
            }
        }
    }

    void ChangeUI(List<ResourceCost> openCosts)
    {
        for(int i = 0; i < uiIcons.Count;i++)
        {
            Destroy(uiIcons[i]);
        }
        uiIcons.Clear();
        for (int i = 0; i < openCosts.Count; i++)
        {
            for (int j = 0; j < openCosts[i].amount; j++)
            {
                CreateUIIcon(ResourceManager.dataBase.GetIcon(openCosts[i].resource));
            }
        }
    }
    void CreateUIIcon(Sprite icon)
    {
        GameObject go = Instantiate(iconPrefab, uiParent);
        go.GetComponent<Image>().sprite =icon;
        uiIcons.Add(go);
    }
}
