using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PatrolRouteConfig
{
    public Transform patrolPath;  // Specific patrol path
    public Transform spawnPoint; // Specific spawn point for this route
    public int spawnCount;        // Number of enemies to spawn on this path
}

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab;                // Enemy type
    public List<PatrolRouteConfig> patrolRoutes;  // Patrol routes with spawn details
    public float spawnDelay = 0.5f;               // Delay between spawns
}

[System.Serializable]
public class PhaseConfig
{
    public string phaseName;                   // Phase identifier
    public List<EnemySpawnConfig> enemies;    // List of enemy configurations
}

public class PhaseManager : MonoBehaviour
{
    [Header("Phase Settings")]
    public List<PhaseConfig> phases;           // All game phases
    private int currentPhaseIndex = 0;

    [Header("Spawn Settings")]
    //public Transform spawnPoint;               // Default spawn point

    [Header("Phase Timing")]
    public float phaseInterval = 2.0f;         // Interval between phases
    private float phaseStartTime;

    [Header("UI")]
    public TextMeshProUGUI phaseMessageText;   // TMP text for displaying phase messages

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool phaseInProgress = false;

    private YouWinPanelMng winPanelManager;    // Reference to the YouWinPanelMng script

    private void Start()
    {
        winPanelManager = FindObjectOfType<YouWinPanelMng>();
        StartPhase();
    }

    public void StartPhase()
    {
        if (currentPhaseIndex < phases.Count)
        {
            phaseInProgress = true;
            phaseStartTime = Time.time;
            StartCoroutine(SpawnPhaseEnemies(phases[currentPhaseIndex]));

            // Display phase start message
            UpdatePhaseMessage($"Phase {currentPhaseIndex + 1} started", 3.0f);
        }
        else
        {
            // All phases completed
            winPanelManager.gameWon = true; // Notify YouWinPanelMng
            UpdatePhaseMessage("All phases completed! You win!", 3.0f);
        }
    }

    private IEnumerator SpawnPhaseEnemies(PhaseConfig phaseConfig)
    {
        foreach (EnemySpawnConfig enemyConfig in phaseConfig.enemies)
        {
            foreach (PatrolRouteConfig routeConfig in enemyConfig.patrolRoutes)
            {
                for (int i = 0; i < routeConfig.spawnCount; i++)
                {
                    // Instantiate enemy
                    GameObject enemy = Instantiate(enemyConfig.enemyPrefab, routeConfig.spawnPoint.position, Quaternion.identity);

                    // Assign patrol path
                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.patrolPath = routeConfig.patrolPath;
                    }

                    // Track active enemies
                    activeEnemies.Add(enemy);

                    // Subscribe to enemy death
                    HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        // Hook into the health system's death logic
                        healthSystem.OnDeath += HandleEnemyDeath;
                    }

                    yield return new WaitForSeconds(enemyConfig.spawnDelay); // Delay between spawns
                }
            }
        }

        // Wait for the phase interval or player to kill all enemies
        yield return new WaitForSeconds(phaseInterval);

        if (phaseInProgress) // Only proceed if the phase hasn't been completed early
        {
            CompletePhase();
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0 && phaseInProgress)
        {
            // All enemies defeated before phase interval
            CompletePhase();
        }
    }

    private void CompletePhase()
    {
        phaseInProgress = false;

        // Display phase completion message
        UpdatePhaseMessage($"Phase {currentPhaseIndex + 1} completed", 3.0f);

        // Move to the next phase
        currentPhaseIndex++;
        StartPhase();
    }

    private void UpdatePhaseMessage(string message, float duration)
    {
        if (phaseMessageText != null)
        {
            phaseMessageText.text = message;
            phaseMessageText.gameObject.SetActive(true);
            StartCoroutine(HidePhaseMessage(duration));
        }
    }

    private IEnumerator HidePhaseMessage(float duration)
    {
        yield return new WaitForSeconds(duration);
        if (phaseMessageText != null)
        {
            phaseMessageText.gameObject.SetActive(false);
        }
    }
}


