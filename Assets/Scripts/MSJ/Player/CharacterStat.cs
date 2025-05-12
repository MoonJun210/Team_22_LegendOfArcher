using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat", menuName = "Character/Stat Data")]
public class CharacterStat : ScriptableObject
{
    public int maxHealth = 5;
    public float moveSpeed = 3f;
}
