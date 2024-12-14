using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class PatrolRouteConfig
{
    public Transform patrolPath;
    public Transform spawnPoint;
    public int spawnCount;
}

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab;
    public List<PatrolRouteConfig> patrolRoutes;
    public float spawnDelay = 0.5f;
}

[System.Serializable]
public class PhaseConfig
{
    public string phaseName;
    public List<EnemySpawnConfig> enemies;
}

public class PhaseManager : MonoBehaviour
{
    private GameStatsManager gameStatsManager;

    [Header("Phase Settings")]
    public List<PhaseConfig> phases;
    private int currentPhaseIndex = 0;

    [Header("Phase Timing")]
    public float phaseInterval = 2.0f;
    private float phaseStartTime;

    [Header("UI")]
    public TextMeshProUGUI phaseMessageText;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool phaseInProgress = false;
    void Awake()
    {
        gameStatsManager = GameStatsManager.Instance;
    }
    private void Start()
    {
        StartPhase();
    }

    public void StartPhase()
    {
        if (currentPhaseIndex < phases.Count)
        {
            phaseInProgress = true;
            phaseStartTime = Time.time;
            StartCoroutine(SpawnPhaseEnemies(phases[currentPhaseIndex]));

            UpdatePhaseMessage($"Phase {currentPhaseIndex + 1} started", 3.0f);
        }
        else
        {
            gameStatsManager.IncrementGamesWon();
            YouWinPanelMng.gameWon = true;
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
                    GameObject enemy = Instantiate(enemyConfig.enemyPrefab, routeConfig.spawnPoint.position, Quaternion.identity);

                    EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                    if (enemyAI != null)
                    {
                        enemyAI.patrolPath = routeConfig.patrolPath;
                    }

                    activeEnemies.Add(enemy);

                    HealthSystem healthSystem = enemy.GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        healthSystem.OnDeath += HandleEnemyDeath;
                    }

                    yield return new WaitForSeconds(enemyConfig.spawnDelay);
                }
            }
        }

        yield return new WaitForSeconds(phaseInterval);

        if (phaseInProgress)
        {
            CompletePhase();
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0 && phaseInProgress)
        {
            CompletePhase();
        }
    }

    private void CompletePhase()
    {
        phaseInProgress = false;

        UpdatePhaseMessage($"Phase {currentPhaseIndex + 1} completed", 3.0f);

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


