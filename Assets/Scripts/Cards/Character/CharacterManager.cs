using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CharacterManager : Manager
{
    #region singleton
    public static CharacterManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    #region events
    public static UnityEvent<List<Character.Upgrade>> OnUpgradeWindow = new UnityEvent<List<Character.Upgrade>>();
    #endregion

    public Characterlibrary library;
    public static List<Character> characters = new List<Character>();

    
    public void CreateStartingCharacters(int count)
    {
        for(int i = 0; i < count; i++)
        {
            characters.Add(CreateCharacter());
        }
    }

    public Character CreateCharacter()
    {
        return library.GetCharacter();
    }

    List<Character.Upgrade> getUpgradeOptions(int count)
    {
        List<Character> possibleCharacters = new List<Character>();
        List<Character.Upgrade> upgrades = new List<Character.Upgrade>();
        foreach (Character c in characters)
        {
            if (c.HasFreeKnowledgeSlot())
            {
                possibleCharacters.Add(c);
            }
        }
        for(int i = 0; i < count; i++)
        {
            int randIindex = Random.Range(0, possibleCharacters.Count);
            KnowledgeType type = (KnowledgeType) Random.Range(1, 4);
            upgrades.Add(new Character.Upgrade(possibleCharacters[randIindex], type));
            possibleCharacters.RemoveAt(randIindex);
        }
        return upgrades;
    }



    #region debug
    [ContextMenu("create 5 test Chracters")]
    void createChracters()
    {
        CreateStartingCharacters(5);
    }

    [ContextMenu("open upgradevideo")]
    void OpenUpgradeWindow()
    {
        OnUpgradeWindow.RemoveListener(UpgradeWindowMessage);
        OnUpgradeWindow.AddListener(UpgradeWindowMessage);
        OnUpgradeWindow.Invoke(getUpgradeOptions(3));
    }

    void UpgradeWindowMessage(List<Character.Upgrade> upgrades)
    {
        for(int i = 0;i<upgrades.Count; i++)
        {
            SendLog(upgrades[i].target.name + " " + upgrades[i].target.surname + " upgrade with: " + upgrades[i].type);
        }
    }

    #endregion
}

