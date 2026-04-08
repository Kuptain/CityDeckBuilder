using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class BuildingDataContainer : ScriptableObject
{
    public BuildingData[] buildings;

    public void AddBuilding()
    {
        BuildingData data = CreateInstance<BuildingData>();
        List<BuildingData> TempList = new List<BuildingData>(){data};
        TempList.Add(data);
        buildings = TempList.ToArray();
        AssetDatabase.CreateAsset(data, "Assets/scriptableObjects/Buildings/Building.asset");
        AssetDatabase.SaveAssets();
    }

}