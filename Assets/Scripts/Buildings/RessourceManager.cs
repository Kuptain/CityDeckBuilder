using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RessourceManager : MonoBehaviour
{
    #region singleton
    public static RessourceManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    public List<Ressource> ressources = new List<Ressource>();
    public static UnityEvent<RessourceType> OnNotEnoughRessources = new UnityEvent<RessourceType>();
    public void GetRessources(RessourceType type, int amount)
    {
        foreach(Ressource r in ressources)
        {
            if ( r.type == type)
            {
                r.amount += amount;
                return;
            }
        }
        AddNewRessource(type).amount+=amount;

    }

    public bool TryToSpendRessource(RessourceType type, int amount)
    {
        foreach (Ressource r in ressources)
        {
            if (r.type == type)
            {
                if (r.amount >= amount)
                {
                    r.amount -= amount;
                    return true;
                }
                else
                {
                    OnNotEnoughRessources.Invoke(type);
                    return false;
                }
            }
        }
        return false;
    }


    Ressource AddNewRessource(RessourceType type)
    {
        Ressource returnValue = new Ressource(type);
        ressources.Add(returnValue);
        return returnValue;
    }
}

[System.Serializable]
public class Ressource
{
    public Ressource(RessourceType _type)
    {
        type = _type;
    }

    public RessourceType type;
    public int amount;
}
