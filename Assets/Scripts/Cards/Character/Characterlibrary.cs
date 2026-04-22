using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Characterlibrary", menuName = "Scriptable Objects/Characterlibrary")]
public class Characterlibrary : ScriptableObject
{
    [SerializeField] List<string> names;
    [SerializeField] List<string> surnames;
    [SerializeField] List<Colorit> colorits;

    public Character GetCharacter()
    {
        string name = names[Random.Range(0, names.Count)];
        string surname = surnames[Random.Range(0, surnames.Count)];
        Colorit colorit = colorits[Random.Range(0, colorits.Count)];

        return new Character(name, surname, colorit, 3);
    }


    [System.Serializable]
    public class Colorit
    {
        public Color baseColor;
        public Color secondColor;
        public Color highlightColor;
    }

}
