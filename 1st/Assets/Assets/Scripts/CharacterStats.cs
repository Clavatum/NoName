using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public enum CharacterType
    {
        Player,
        Enemy,
        Soldier
    }

    public CharacterType characterType; 
    public float maxHealth = 100f;
    public float shield = 0f; 
    public bool hasShield = false; 
}
