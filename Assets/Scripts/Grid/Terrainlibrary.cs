using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrainlibrary", menuName = "Scriptable Objects/Terrainlibrary")]
public class Terrainlibrary : ScriptableObject
{
    public Color color;
    public Tile.TileType type;
    public List<TerrainData> terrains;
    [Range(0,100)] public int weight=1;
    int sumOfWeigth;


    public void CalcSumOfWeigth()
    {
        sumOfWeigth = 0;
        for (int i = 0; i < terrains.Count; i++)
        {
            sumOfWeigth += terrains[i].weight;
        }
    }

    public TerrainData GetTerrain()
    {
        int target = Random.Range(1, sumOfWeigth + 1);
        int currentSum = 0;
        for (int i = 0; i < terrains.Count; i++)
        {
            currentSum += terrains[i].weight;
            if (target <= currentSum)
            {
                return terrains[i];
            }
        }

        Debug.LogError("something with the weigths went wrong that shouldn't happen");
        return terrains[0];
    }
}
