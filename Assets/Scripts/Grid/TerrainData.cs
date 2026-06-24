using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Terrain", menuName = "Scriptable Objects/Terrain")]
public class TerrainData : ScriptableObject
{

    [Range(0, 100)] public int weight = 1;
    public TileVisualType visualType = new TileVisualType();
    public List<TerrainEffect> effects = new List<TerrainEffect>();

    public TerrainEffect GetEffect(ICard card)
    {
        if (card.GetType() == CardType.Character)
        {
            CharacterCard character = (CharacterCard)card;
            for (int i = 0; i < effects.Count; i++)
            {
                if (character.ContainsKnowledge(effects[i].knowledgeCost))
                {
                    Debug.Log(effects[i]);
                    return effects[i];
                }
            }
        }
        return null;
    }
}

[System.Serializable]
public class TerrainEffect
{
    public bool HasCoolDown;
    public int cooldownDuration;
    public List<KnowledgeType> knowledgeCost = new List<KnowledgeType>();
    public List<ResourceCard_Data> temporaryCards;
    public UnityEvent<CharacterCard, TerrainEffect> OnTrigger = new UnityEvent<CharacterCard, TerrainEffect>();


    public bool TryToInvoke(ICard card)
    {
        if (card.GetType() == CardType.Character)
        {
            CharacterCard character = (CharacterCard)card;
            if (character.ContainsKnowledge(knowledgeCost))
            {
                Invoke(character);
                return true;
            }
        }
        return false;
    }

    void Invoke(CharacterCard card)
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