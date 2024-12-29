using System;
using Combat;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Slider healthBar;
    private Animator animator;
    private CharacterStats characterStats;
    [SerializeField] private GameOverPanelMng gameOverPanelMng;
    private GameStatsManager gameStatsManager;

    private bool isDead = false;

    public float goldOnDeath = 30f;

    public event Action<GameObject> OnDeath;

    private void Awake()
    {
        gameStatsManager = GameStatsManager.Instance;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        if (characterStats != null)
        {
            maxHealth = characterStats.maxHealth;
            currentHealth = maxHealth;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage, string attackerTag)
    {
        if (attackerTag == gameObject.tag ||
        (attackerTag == "Player" && gameObject.tag == "Soldier") ||
        (attackerTag == "Soldier" && gameObject.tag == "Player") ||
        (attackerTag == "Arrow" && gameObject.tag == "Player"))
        {
            Debug.Log("Friendly fire prevented!");
            return;
        }
        if (PlayerCombat.isBlocking)
        {
            damage /= 2f;
        }
        if (characterStats != null && characterStats.hasShield && characterStats.shield > 0)
        {
            characterStats.shield -= damage;

            if (characterStats.shield < 0)
            {
                damage = -characterStats.shield;
                characterStats.shield = 0;
            }
            else
            {
                damage = 0;
            }
        }

        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead && gameObject.CompareTag("Player"))
        {
            Die(attackerTag);
            HandlePlayerDeath();
        }
        else if (currentHealth <= 0 && !isDead)
        {
            currentHealth = 0;
            Die(attackerTag);
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
            animator.SetTrigger("Impact");
        }
    }

    void Die(string attackerTag)
    {
        if (isDead)
        {
            return;
        }

        if (attackerTag == "Player" || attackerTag == "Soldier")
        {
            GameStatsManager.Instance.AddKill();
            GameStatsManager.Instance.EarnGold(goldOnDeath);
        }

        isDead = true;

        OnDeath?.Invoke(gameObject);

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
            animator.SetTrigger("Death");
            gameObject.tag = "Dead";
        }

        DisableActions();
        Invoke(nameof(DestroyCharacter), 4f);
    }

    private async void HandlePlayerDeath()
    {
        gameStatsManager.CompleteGame();
        gameOverPanelMng.isGameOver = true;

        await CloudSaveManager.SaveToCloud(CloudSaveManager.CollectDataForSave());
        Debug.Log("Player data saved to the cloud on death.");
    }

    void DestroyCharacter()
    {
        Destroy(gameObject);
    }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }

    void DisableActions()
    {
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
        if (TryGetComponent(out BaseAI movement))
        {
            movement.enabled = false;
        }
    }
}
