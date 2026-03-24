using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IconAnimationManager : MonoBehaviour
{
    #region singleton
    public static IconAnimationManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            OnCentrebuilding.AddListener(SetCentreBuiilding);
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion

    
    public GameObject IconPrefab;
    public Transform DeckTarget;
    public Transform CentreBuilding;
    [SerializeField] List<IconAnimationData> icons;
    public static UnityEvent<BuildingObject> OnCentrebuilding = new UnityEvent<BuildingObject>();
    public void Update()
    {
        MoveIcons();
    }

    private void MoveIcons()
    {
        for(int i = icons.Count - 1; i >= 0; i--)
        {
            if(icons[i] == null)
            {
                icons.RemoveAt(i);
            }
            else
            {
                icons[i].Move();
            }
        }
    }

    void SetCentreBuiilding(BuildingObject centre)
    {
        CentreBuilding = centre.transform;
    }
    public void Anim_ResourceToDeck(ResourceType type, Vector3 startPosition)
    {
        IconAnimationData data = Instantiate(IconPrefab, startPosition, Quaternion.identity, transform).GetComponent<IconAnimationData>();
        data.SetUp(ResourceManager.dataBase.GetIcon(type),startPosition, DeckTarget);
        icons.Add(data);
    }
    public void Anim_ResourceToProductionDeck(ResourceType type, Vector3 startPosition)
    {
        IconAnimationData data = Instantiate(IconPrefab, startPosition, Quaternion.identity, transform).GetComponent<IconAnimationData>();
        data.SetUp(ResourceManager.dataBase.GetIcon(type), startPosition, CentreBuilding);
        icons.Add(data);
    }




}
