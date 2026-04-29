using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    //public byte ID;
    [SerializeField] public string buildingName;
    [SerializeField] public string buildingDescription;
    public ResourceType test;
    public GameObject prefab;
    public Sprite uiIcon;
    public GameObject uiButton;
    public Tile.TileType requiredTerrain;
    public bool isLocked;
    [SerializeField] List<RankData> ranks = new List<RankData>() { new RankData() };

    public RankData GetRankData(int rank)
    {
        if (ranks.Count < rank + 1)
        {
            return null;
        }
        return ranks[rank];
    }
    public List<ResourceCost> GetBaseCost()
    {
        if (ranks.Count == 0)
        {
            return new List<ResourceCost>();
        }
        return ranks[0].resourceCosts;
    }

    public bool HasCardsToAddOnBuild(int currentRank)
    {
        return ranks[currentRank].cardsToAdd.Count > 0;
    }
}
[System.Serializable]
public class RankData
{
    public int housingIncrease;
    public bool usesCrafting;
    public List<Card_Data> cardsToAdd;
    public List<ResourceCost> resourceCosts;
    public List<BuildingEffect> effects = new List<BuildingEffect>();
    public List<CraftRecipe> craftingRecipes = new List<CraftRecipe>();
}
[System.Serializable]
public class BuildingEffect
{
    public triggerType type;
    public bool HasCoolDown;
    public int cooldownDuration;
    public List<ResourceCost> EffectCost = new List<ResourceCost>();
    public List<Card_Data> temporaryCards;
    public UnityEvent<BuildingObject, RessourceCard, BuildingEffect> OnTrigger = new UnityEvent<BuildingObject, RessourceCard,BuildingEffect>();
    public enum triggerType
    {
        onBuild = 0,
        onCard = 1,
        onEndOfTurn = 2
    }

    public void Invoke(BuildingObject bO, RessourceCard card)
    {
        OnTrigger.Invoke(bO, card, this);
    }

}

[System.Serializable]
public class CraftRecipe
{
    public List<ResourceCost> costs;
    public List<Card_Data> cardsToCreate;
}


