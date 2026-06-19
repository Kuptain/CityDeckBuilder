using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Terrain", menuName = "Scriptable Objects/Terrain")]
public class TerrainData : ScriptableObject
{
    public TerrainEffect effect;

    public TileVisualType visualType = new TileVisualType();
    [Range(0, 100)] public int weight = 1;
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
[System.Serializable]
public class TileVisualType
{
    [Tooltip("How frequent should this type spawn")]
    public float tileOffsetY;
    public List<GameObject> visualVariants;
}