using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrainlibrary", menuName = "Scriptable Objects/Terrainlibrary")]
public class Terrainlibrary : ScriptableObject
{
    public Tile.TileType type;
    public List<Terrain> terrains;
    

    public Terrain GetTerrain()
    {
        return terrains[Random.Range(0, terrains.Count)];
    }
}
