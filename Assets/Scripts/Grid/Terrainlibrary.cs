using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrainlibrary", menuName = "Scriptable Objects/Terrainlibrary")]
public class Terrainlibrary : ScriptableObject
{
    public Color color;
    public Tile.TileType type;
    public List<TerrainData> terrains;
    

    public TerrainData GetTerrain()
    {
        return terrains[Random.Range(0, terrains.Count)];
    }
}
