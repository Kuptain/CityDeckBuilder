using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    [HideInInspector] public bool selected;
    public Sprite sprite;
    public List<RessourceType> ressources;
}

public enum RessourceType
{
    none = 0,
    wood = 1,
    stone = 2,
    gold = 3,
    sheep = 4
}