using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public string buildingDescription;
    public GameObject prefab;
    public GameObject preview;
    public byte ID;
    //public List<ResourceTypes> resourceCosts; add a cost to the building
}
