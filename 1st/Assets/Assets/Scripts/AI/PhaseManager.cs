using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnConfig
{
    public GameObject enemyPrefab; 
    public int spawnCount;         
}

[System.Serializable]
public class PhaseConfig
{
    public string phaseName;                  
    public List<EnemySpawnConfig> enemies;    
    public float spawnDelay = 0.5f;           
}

public class PhaseManager : MonoBehaviour
{
    [Header("Phase Settings")]
    public List<PhaseConfig> phases;          
    private int currentPhaseIndex = 0;

    [Header("Spawn Settings")]
    public Transform spawnPoint;              
    public Transform patrolPath;              

    [Header("Phase Timing")]
    public float phaseInterval = 2.0f;        

    private void Start()
    {
        StartPhase();
    }

    public void StartPhase()
    {
        if (currentPhaseIndex < phases.Count)
        {
            StartCoroutine(SpawnPhaseEnemies(phases[currentPhaseIndex]));
        }
    }

    private IEnumerator SpawnPhaseEnemies(PhaseConfig phaseConfig)
    {
        foreach (EnemySpawnConfig enemyConfig in phaseConfig.enemies)
        {
            for (int i = 0; i < enemyConfig.spawnCount; i++)
            {
                GameObject enemy = Instantiate(enemyConfig.enemyPrefab, spawnPoint.position, Quaternion.identity);

                EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.patrolPath = patrolPath; 
                }

                yield return new WaitForSeconds(phaseConfig.spawnDelay); 
            }
        }

        currentPhaseIndex++;
        yield return new WaitForSeconds(phaseInterval); 
        StartPhase(); 
    }
}
