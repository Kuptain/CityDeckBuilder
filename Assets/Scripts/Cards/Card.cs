using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    [HideInInspector] public bool selected;
    public Sprite sprite;
    public List<ResourceType> ressources;
}

