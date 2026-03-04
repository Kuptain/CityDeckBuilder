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
    public static UnityEvent<Ressource> OneNewRessource = new UnityEvent<Ressource>();
    public static UnityEvent OnRessourceschanged = new UnityEvent();
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
        Ressource newRessource = new Ressource(type);
        ressources.Add(newRessource);
        return newRessource;
    }

    
}

[System.Serializable]
public class Ressource
{
    public Ressource(RessourceType _type)
    {
        type = _type;
        RessourceManager.OneNewRessource.Invoke(this);
    }

    public RessourceType type;
    int _amount;
    public int amount { get { return _amount; } set { _amount = value; RessourceManager.OnRessourceschanged.Invoke(); } }
}
