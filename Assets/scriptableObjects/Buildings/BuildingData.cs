using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    public byte ID;
    public string buildingName;
    public string buildingDescription;
    public List<Card> cardsToAdd;
    public List<ResourceCost> resourceCosts;

    public GameObject prefab;
    public Sprite uiIcon;
}

