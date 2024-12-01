using Combat;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private GameObject character; 
    public float damageAmount = 10f; 
    private Collider triggerCollider;
    private bool isAttacking = false;

    private PlayerCombat playerCombat;
    private EnemyAI enemyAI;
    private SoldierAI soldierAI;
    [SerializeField] private bool isRanged = false;


    private void Start()
    {
        if (character == null)
        {
            Debug.LogError("Character reference is missing on DamageTrigger.");
            return; 
        }

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

        triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null)
        {
            Debug.LogError("Trigger collider is missing on DamageTrigger.");
            return;
        }
        if (!isRanged) 
        {
            triggerCollider.enabled = false;
        }
    }

    private void Update()
    {
        if (character == null)
        {
            return; 
        }

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

        if (isAttacking && !isRanged)
        {
            EnableCollider();
        }
        else if(!isAttacking && !isRanged)
        {
            DisableCollider();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health != null)
        {
            health.TakeDamage(damageAmount, character.tag);
            Debug.Log($"Damage applied to {other.name} by {character.name}.");
        }
        if (isRanged)
        {
            Destroy(gameObject);
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
    }
}
