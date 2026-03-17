using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingData", menuName = "Scriptable Objects/Building Data")]
public class BuildingData : ScriptableObject
{
    public byte ID;
    public string buildingName;
    public string buildingDescription;

    public GameObject prefab;
    public Sprite uiIcon;
    public GameObject uiButton;

    [SerializeField] List<RankData> ranks = new List<RankData>();

    public RankData GetRankData(int rank)
    {
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

}
[System.Serializable]
public class RankData
{
    public List<Card_Data> cardsToAdd;
    public List<ResourceCost> resourceCosts;
    public int housingIncrease;
    public List<BuildingEffect> effects = new List<BuildingEffect>();
}
[System.Serializable]
public class BuildingEffect
{
    public triggerType type;
    public bool HasCoolDown;
    public int cooldownDuration;
    public List<ResourceCost> EffectCost = new List<ResourceCost>();
    public UnityEvent<BuildingObject, Card> OnTrigger = new UnityEvent<BuildingObject, Card>();
    public enum triggerType
    {
        onBuild = 0,
        onCard = 1,
        onEndOfTurn = 2
    }
}


