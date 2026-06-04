using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character 
{
    public string name;
    public string surname;
    public string FullName { get { return name + " " + surname; } }
    public Characterlibrary.Colorit colorit;
    public KnowledgeType[] knowledge;
    public HealthState healthState;
    public CharacterEffect effect;
    public Character(string _name, string _surname, Characterlibrary.Colorit _colorit,int _knowledgeSlots)
    {
        name = _name;
        surname = _surname;
        colorit = _colorit;
        knowledge = new KnowledgeType[_knowledgeSlots];
        for(int i = 0; i < knowledge.Length; i++)
        {
            knowledge[i] = KnowledgeType.empty;
        }
    }
    public bool HasFreeKnowledgeSlot()
    {
        for (int i = 0; i < knowledge.Length; i++)
        {
            if (knowledge[i] == KnowledgeType.empty)
            {
                return true;
            }
        }
        return false;
    }

    public void AddKnowledge(KnowledgeType type)
    {
        for(int i = 0; i < knowledge.Length; i++)
        {
            if(knowledge[i] == KnowledgeType.empty)
            {
                knowledge[i] = type;
                return;
            }
        }
    }

    public bool HasKnowledge(List<KnowledgeType> needed)
    {
        bool found;
        KnowledgeType[] copy = new KnowledgeType[knowledge.Length];
        knowledge.CopyTo(copy,0);

        for(int i = 0; i < needed.Count; i++)
        {
            found = false;
            for(int j = 0; j < copy.Length; j++)
            {
                if (needed[i] == copy[j])
                {
                    found = true;
                    copy[j] = KnowledgeType.empty;
                    break;
                }
            }
            if(!found)
            {
                return false;
            }
        }
        return true;
    }


    public void ChangeKnowledge(KnowledgeType type, int index)
    {
        knowledge[index] = type;
    }

    public void AddKnowledgeSlot()
    {
        KnowledgeType[] copy = new KnowledgeType[knowledge.Length + 1];
        knowledge.CopyTo(copy, 0);
        knowledge = copy;
    }



    public void TakeDamage()
    {
        if (healthState == HealthState.alive)
        {
            SetHealthTo(HealthState.hurt);
        }
        if(healthState == HealthState.hurt)
        {
            SetHealthTo(HealthState.dead);
        }
    }

    void SetHealthTo(HealthState state)
    {
        healthState = state;
    }

    public enum HealthState
    {
        alive = 0,
        hurt = 1,
        dead = 2,
        burried = 3
    }
    public class Upgrade
    {
        public Character target;
        public KnowledgeType type;

        public Upgrade(Character _target, KnowledgeType typeToAdd)
        {
            target = _target;
            type = typeToAdd;
        }
    }
}

public enum KnowledgeType
{
    empty = 0,
    athority = 1,
    solidarity = 2,
    independence = 3,
}