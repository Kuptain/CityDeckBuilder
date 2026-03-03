using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class Card : ScriptableObject
{
    public Sprite sprite;
    public RessourceType ressource;
    public int amount;
}

public enum RessourceType
{
    none = 0,
    wood = 1,
    stone = 2
}