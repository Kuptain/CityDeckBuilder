using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{

    public bool createLookUpOnAwake;
    public List<Terrainlibrary> terrainLibraries;

    static Dictionary<Tile.TileType, Terrainlibrary> terrainLookUp = new Dictionary<Tile.TileType, Terrainlibrary>();
    static int sumOfWeigth;
    private void Awake()
    {

        if (createLookUpOnAwake) CreateLookup();

    }

    public static Tile.TileType GetTileType()
    {
        Tile.TileType returnType = Tile.TileType.Default;
        Tile.TileType[] types = new Tile.TileType[terrainLookUp.Keys.Count];
        terrainLookUp.Keys.CopyTo(types, 0);
        int target = Random.Range(1, sumOfWeigth+1);
        int currentSum = 0;
        for (int i = 0; i < types.Length; i++)
        {
            currentSum += terrainLookUp[types[i]].weight;
            if (target <= currentSum)
            {
                returnType = types[i];
                break;
            }
        }



        return returnType;
    }

    public static TerrainData GetTerrain(Tile.TileType type)
    {
        return terrainLookUp[type].GetTerrain();
    }

    public static Color GetColor(Tile.TileType type)
    {
        return terrainLookUp[type].color;
    }

    [ContextMenu("create look up")]
    public void CreateLookup()
    {
        terrainLookUp.Clear();
        sumOfWeigth = 0;
        for (int i = 0; i < terrainLibraries.Count; i++)
        {
            if (!terrainLookUp.ContainsKey(terrainLibraries[i].type))
            {
                terrainLookUp.Add(terrainLibraries[i].type, terrainLibraries[i]);
                sumOfWeigth += terrainLibraries[i].weight;
                terrainLibraries[i].CalcSumOfWeigth();
            }
            else
            {
                Debug.LogError("the terrainlibrary at index " + i + " has a type that is already in the lookup");
            }
        }
    }

}
