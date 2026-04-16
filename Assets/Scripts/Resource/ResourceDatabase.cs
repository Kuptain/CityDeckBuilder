using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDatabase", menuName = "Scriptable Objects/Resource Database")]
public class ResourceDatabase : ScriptableObject
{
    public List<ResourceEntry> resources = new List<ResourceEntry>();

    private Dictionary<ResourceType, Sprite> _iconLookup;

    private void OnEnable()
    {
        BuildDictionary();
    }

    void BuildDictionary()
    {
        _iconLookup = new Dictionary<ResourceType, Sprite>();

        foreach (var entry in resources)
        {
            if (!_iconLookup.ContainsKey(entry.type))
                _iconLookup.Add(entry.type, entry.icon);
        }
    }

    public Sprite GetIcon(ResourceType type)
    {
        if (_iconLookup == null)
            BuildDictionary();

        return _iconLookup.TryGetValue(type, out var icon) ? icon : null;
    }
}
public enum ResourceType
{
    none = 0,
    wood = 1,
    stone = 2,
    gold = 3,
    sheep = 4,
    person = 5,
    clay = 6,
    grain = 8,
    straw = 10,
    flour = 11,
    bread = 12, 
    meat = 13,
    brick = 14,
    coal = 15,
    sandwich = 16,
    Fish = 17

}
[System.Serializable]
public class ResourceEntry
{
    public ResourceType type;
    public Sprite icon;
}

[System.Serializable]
public class ResourceCost
{
    public ResourceType resource;
    public int amount;

    public ResourceCost (ResourceType type, int _value)
    {
        resource = type;
        amount = _value;
    }

    public void Subtract(ResourceCost cost)
    {
        if(cost.resource == resource)
        {
            amount -= cost.amount;
        }
    }

    public void Add(ResourceCost cost)
    {
        if (cost.resource == resource)
        {
            amount += cost.amount;
        }
    }
}