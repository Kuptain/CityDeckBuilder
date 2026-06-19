using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager instance;
    public bool createLookUpOnAwake;
    public List<Terrainlibrary> terrainLibraries;

    static Dictionary<Tile.TileType, Terrainlibrary> terrainLookUp = new Dictionary<Tile.TileType, Terrainlibrary>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (createLookUpOnAwake) CreateLookup();
        }
        else
        {
            Destroy(this);
        }
    }



    public Terrain GetTerrain(Tile.TileType type)
    {
        return terrainLookUp[type].GetTerrain();
    }

    [ContextMenu("create look up")]
    public void CreateLookup()
    {
        terrainLookUp.Clear();
        for (int i = 0; i < terrainLibraries.Count; i++)
        {
            if (!terrainLookUp.ContainsKey(terrainLibraries[i].type))
            {
                terrainLookUp.Add(terrainLibraries[i].type, terrainLibraries[i]);
            }
            else
            {
                Debug.LogError("the terrainlibrary at index " + i + " has a type that is already in the lookup");
            }
        }
    }

}
