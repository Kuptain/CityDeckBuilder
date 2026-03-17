using UnityEngine;

[System.Serializable]
public class Card
{

    public Card_Data data;
    BuildingObject originBuilding;
    public int roundsInHand;
    public int rank;

    public Card(Card_Data _data)
    {
        data = _data;
    }

    public void Upgrade()
    {
        rank += 1;
    }
}
