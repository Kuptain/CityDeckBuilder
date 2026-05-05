using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CharacterEffect", menuName = "Scriptable Objects/CharacterEffect")]
public class CharacterEffect : ScriptableObject
{
    public string description = " ";
    public UnityEvent trigger;
}
