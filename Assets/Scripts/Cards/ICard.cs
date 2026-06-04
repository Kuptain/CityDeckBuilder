using UnityEngine;

public interface ICard 
{
    public CardType GetType();
   
}

public enum CardType
{
    Character = 0,
    Resource = 1
}