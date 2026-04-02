using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionVisualizer : MonoBehaviour
{
    [ReadOnly] public BuildingObject building;
    [SerializeField] Transform uiParent;
    [SerializeField] GameObject iconPrefab;
    List<GameObject> objectProgression = new List<GameObject>();
    List<GameObject> uiIcons = new List<GameObject>();
    [ReadOnly] public double progress;

    public void Init(BuildingObject newBuilding)
    {
        building = newBuilding;
        building.OnConstructionProgress.AddListener(ChangeConstruction);

        foreach (Transform child in building.buildingVisualTransform.transform)
        {
            objectProgression.Add(child.gameObject);
        }
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

        if((int)sumOfCosts - (int)sumOfOpenCosts > 0 || sumOfCosts == 0)
        {
            building.EnableOriginMaterials();
        }
        //ChangeObjects(progress);
        ChangeUI(CostStillOpen);
    }
    void ChangeObjects(double progess)
    {
        float visibleProgression = objectProgression.Count * (float)progess;

        for (int i = 0; i < objectProgression.Count; i++)
        {
            if (i < visibleProgression)
            {
                objectProgression[i].SetActive(true); // Change Material instead
            }
            else
            {
                objectProgression[i].SetActive(false); // Change Material instead
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
