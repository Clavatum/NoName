using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public enum CharacterType
    {
        Player,
        Enemy,
        Soldier
    }

    public CharacterType characterType; // Character type selected from the enum
    public float maxHealth = 100f;
    public float shield = 0f; // Shield is only defined here
    public bool hasShield = false; // Does the character have shield?
}
