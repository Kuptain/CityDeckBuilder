using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectCostVisualizer : MonoBehaviour
{
    public bool isConstruction;
    [ReadOnly] public BuildingObject building;
    [SerializeField] Transform uiParent;
    [SerializeField] GameObject iconPrefab;
    List<GameObject> objectProgression = new List<GameObject>();
    List<GameObject> uiIcons = new List<GameObject>();
    [ReadOnly] public double progress;
    public void Init(BuildingObject newBuilding)
    {
        building = newBuilding;
        building.OnEffectProgress.AddListener(ChangeCosts);

        foreach (Transform child in building.buildingVisualTransform.transform)
        {
            objectProgression.Add(child.gameObject);
        }
    }

    void ChangeCosts(BuildingObject.OpenEffect effect)
    {
        if(effect == null)
        {
            ChangeUI(new List<ResourceCost>());
            return;
        }
        if (effect.type == BuildingObject.OpenEffect.Type.construction)
        {
            float sumOfCosts = 0;
            for (int i = 0; i < effect.Costs.Count; i++)
            {
                sumOfCosts += effect.Costs[i].amount;
            }
            float sumOfOpenCosts = 0;
            for (int i = 0; i < effect.CostsStillOpen.Count; i++)
            {
                sumOfOpenCosts += effect.CostsStillOpen[i].amount;
            }

            progress = (sumOfCosts - sumOfOpenCosts) / sumOfCosts;

            if ((int)sumOfCosts - (int)sumOfOpenCosts > 0 || sumOfCosts == 0)
            {
                building.EnableOriginMaterials();
            }
            else
            {
                ChangePreview(progress);
            }
        }
        ChangeUI(effect.CostsStillOpen);
    }
    void ChangePreview(double progess)
    {
        float visibleProgression = objectProgression.Count * (float)progess;

        for (int i = 0; i < objectProgression.Count; i++)
        {
            if (i < visibleProgression)
            {
                building.EnableOriginMaterial(i);
                //objectProgression[i].SetActive(true); // Change Material instead
            }
            else
            {
                //objectProgression[i].SetActive(false); // Change Material instead
            }
        }
    }
    public void ToggleVisible(bool state)
    {
        uiParent.gameObject.SetActive(state);
    }

    void ChangeUI(List<ResourceCost> openCosts)
    {
        Debug.Log("change UI: " + openCosts.Count);
        for (int i = 0; i < uiIcons.Count; i++)
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
        go.GetComponent<Image>().sprite = icon;
        uiIcons.Add(go);
    }
}
