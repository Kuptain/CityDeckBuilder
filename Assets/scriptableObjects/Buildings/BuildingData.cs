using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Mono.Cecil;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public string buildingDescription;
    public GameObject prefab;
    public GameObject uiButton;
    public byte ID;
    public List<ResourceCost> resourceCosts;
    public List<Card> cardsToAdd;

    public ResourceCost EffectCost;
    public UnityEvent<BuildingData> OnBuild = new UnityEvent<BuildingData>();
    public UnityEvent OnDrag = new UnityEvent();
    public UnityEvent OnClick = new UnityEvent();
    public UnityEvent OnEndOfRound = new UnityEvent();

    public void createWater(BuildingData data)
    {

    }
}

