using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Terrain", menuName = "Scriptable Objects/Terrain")]
public class Terrain : ScriptableObject
{
    public Tile.TileType type;

    public TerrainEffect effect;

}

public class TerrainEffect
{
    public bool HasCoolDown;
    public int cooldownDuration;
    public List<KnowledgeType> knowledgeCost = new List<KnowledgeType>();
    public List<ResourceCard_Data> temporaryCards;
    public UnityEvent< CharacterCard, TerrainEffect> OnTrigger = new UnityEvent< CharacterCard, TerrainEffect>();
   

    public void Invoke(CharacterCard card)
    {
        OnTrigger.Invoke(card, this);
    }
}