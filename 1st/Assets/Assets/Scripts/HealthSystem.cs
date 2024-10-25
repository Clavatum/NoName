using Combat;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthBar;
    private Animator animator;  // Animator fetched from component
    private CharacterStats characterStats; // Reference to CharacterStats script
    private bool isDead = false; // Track if the character is dead

    void Start()
    {
        // Fetch the Animator component
        animator = GetComponent<Animator>();

        // Fetch the CharacterStats script to access its properties
        characterStats = GetComponent<CharacterStats>();

        if (characterStats != null)
        {
            maxHealth = characterStats.maxHealth; // Use maxHealth from CharacterStats
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage, string attackerTag)
    {
        // Eðer karakter ölmüþse hiçbir iþlem yapýlmaz
        // Karakterlerin ayný tag'e sahip olup olmadýðýný kontrol et
        if (attackerTag == gameObject.tag)
        {
            Debug.Log("Friendly fire prevented!");
            return; // Ayný tag'e sahiplerse hasar iþlemini sonlandýr
        }

        if (characterStats != null && characterStats.hasShield && characterStats.shield > 0)
        {
            characterStats.shield -= damage;
            Debug.Log($"{gameObject.name} kalkan hasarý: {characterStats.shield}");

            if (characterStats.shield < 0)
            {
                damage = -characterStats.shield; // Eðer kalkan sýfýrýn altýna düþerse kalan hasar
                characterStats.shield = 0;
            }
            else
            {
                damage = 0; // Kalkan hasarý tamamen emdi
            }
        }

        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            TriggerImpact();
        }
        UpdateHealthBar();
    }


    void TriggerImpact()
    {
        if (animator != null)
        {
            animator.SetTrigger("Impact"); // Play Impact animation
        }
    }

    void Die()
    {
        if (isDead)
        {
            return; // Ensure Die() is only called once
        }

        isDead = true; // Set isDead to true to prevent further actions

        if (animator != null)
        {
            animator.SetTrigger("Death"); // Play Death animation
        }

        // Disable any movement, combat, or other scripts here
        DisableActions();

        // Optionally destroy the character after some time
        Invoke(nameof(DestroyCharacter), 4f);
    }

    void DestroyCharacter()
    {
        Destroy(gameObject); // Destroy the character after 5 seconds
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth; // Update health bar
        }
    }

    void DisableActions()
    {
        // Here you can disable other scripts like movement and combat
        if (TryGetComponent(out PlayerCombat playerCombat))
        {
            playerCombat.enabled = false;
        }
        if (TryGetComponent(out EnemyAI enemyAI))
        {
            enemyAI.enabled = false;
        }
        if (TryGetComponent(out SoldierAI soldierAI))
        {
            soldierAI.enabled = false;
        }

        // If there's a movement script, disable it as well
        if (TryGetComponent(out BaseAI movement))
        {
            movement.enabled = false;
        }

        /*// Disable collider interaction but leave the collider itself active if needed
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.isTrigger = true; // Optionally make colliders triggers instead of disabling them
        }*/
    }
}
