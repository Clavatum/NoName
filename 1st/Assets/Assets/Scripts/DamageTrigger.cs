using Combat;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private GameObject character; // Character object assigned in Inspector
    public float damageAmount = 10f; // Damage dealt
    private Collider triggerCollider;
    private bool isAttacking = false;

    private PlayerCombat playerCombat;
    private EnemyAI enemyAI;
    private SoldierAI soldierAI;

    private void Start()
    {
        // Ensure character is assigned
        if (character == null)
        {
            Debug.LogError("Character reference is missing on DamageTrigger.");
            return; // Exit early if character is not assigned
        }

        // Fetch only the relevant component
        if (character.tag == "Player")
        {
            playerCombat = character.GetComponent<PlayerCombat>();
            if (playerCombat == null)
            {
                Debug.LogError("PlayerCombat component missing on player character.");
            }
        }
        else if (character.tag == "Enemy")
        {
            enemyAI = character.GetComponent<EnemyAI>();
            if (enemyAI == null)
            {
                Debug.LogError("EnemyAI component missing on enemy character.");
            }
        }
        else if (character.tag == "Soldier")
        {
            soldierAI = character.GetComponent<SoldierAI>();
            if (soldierAI == null)
            {
                Debug.LogError("SoldierAI component missing on soldier character.");
            }
        }

        // Get the trigger collider and disable it initially
        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogError("Trigger collider is missing on DamageTrigger.");
            return;
        }

        triggerCollider.enabled = false; // Start with the collider disabled
    }

    private void Update()
    {
        if (character == null)
        {
            return; // Exit early if character is not set
        }

        // Update isAttacking flag based on which component is available
        if (playerCombat != null)
        {
            isAttacking = playerCombat.isAttacking;
        }
        else if (enemyAI != null)
        {
            isAttacking = enemyAI.isAttacking;
        }
        else if (soldierAI != null)
        {
            isAttacking = soldierAI.isAttacking;
        }

        // Enable or disable collider based on isAttacking state
        if (isAttacking)
        {
            EnableCollider();
        }
        else
        {
            DisableCollider();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            // Attack eden karakterin tag'ini ekliyoruz
            health.TakeDamage(damageAmount, character.tag);
            Debug.Log($"Damage applied to {other.name} by {character.name}.");
        }
    }

    public void EnableCollider()
    {
        triggerCollider.enabled = true;
        Debug.Log("Collider enabled");
    }

    public void DisableCollider()
    {
        triggerCollider.enabled = false;
        Debug.Log("Collider disabled");
    }
}
