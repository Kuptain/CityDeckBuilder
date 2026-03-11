using Mono.Cecil;
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
    gold = 3
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
}