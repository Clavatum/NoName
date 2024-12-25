using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UnitProduction : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;
    [SerializeField] private float unitSpacing = 1f;
    [SerializeField] private Button produceUnitButton;
    [SerializeField] private Button destroyTowerButton;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private GameObject towerUIPanel;
    [SerializeField] private LayerMask roadLayerMask;
    [SerializeField] private LayerMask unitLayerMask;
    private float upgradeCost = 30f;

    private void Awake()
    {
        produceUnitButton.onClick.AddListener(ProduceUnits);
        destroyTowerButton.onClick.AddListener(DestroyTower);

        EventTrigger trigger = produceUnitButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((eventData) => SetIgnoreTowerClick(true));
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((eventData) => SetIgnoreTowerClick(false));
        trigger.triggers.Add(entryExit);
    }

    private void DestroyTower()
    {
        AudioManager.Instance.PlayButtonClick();
        Destroy(gameObject);
    }

    void Update()
    {
        if (!BuildingMng.isPanelActive)
        {
            towerUIPanel.SetActive(false);
        }
    }

    private void ProduceUnits()
    {
        AudioManager.Instance.PlayButtonClick();
        if (GameStatsManager.Instance.totalGold >= upgradeCost)
        {
            GameStatsManager.Instance.SpendGold(upgradeCost);

            PlayerPrefs.SetFloat("TotalGold", GameStatsManager.Instance.totalGold);
            PlayerPrefs.Save();

            UpdateBalanceUI();
            Vector3 roadPosition = GetNearestRoadPosition();
            Debug.Log($"Nearest road position: {roadPosition}");

            if (roadPosition != Vector3.zero)
            {
                int soldiersSpawned = 0;
                int maxSoldiers = 5;
                int spawnAttempts = 50;

                for (int attempt = 0; attempt < spawnAttempts && soldiersSpawned < maxSoldiers; attempt++)
                {
                    Vector3 spawnPosition = GetRandomPositionOnSurface(roadPosition);
                    spawnPosition = AdjustToRoadLevel(spawnPosition);

                    if (spawnPosition != Vector3.zero)
                    {
                        Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                        soldiersSpawned++;
                    }
                }

                if (soldiersSpawned == 0)
                {
                    Debug.LogWarning("No valid positions found to spawn soldiers.");
                }
                else
                {
                    Debug.Log($"{soldiersSpawned} soldiers successfully spawned.");
                }
            }
            else
            {
                Debug.LogError("No road detected nearby!");
            }
        }
        else
        {
            StartCoroutine(ShowFeedback("Not enough gold to upgrade towers!"));
            Debug.Log("Upgrade failed: Not enough gold.");
        }
        BuildingMng.ignoreTowerClick = true;
        Invoke(nameof(ResetIgnoreTowerClick), 2f);


    }

    public void UpdateBalanceUI()
    {
        PlayerPrefs.GetFloat("TotalGold", GameStatsManager.Instance.totalGold);
        balanceText.text = "Gold: " + GameStatsManager.Instance.totalGold.ToString("F2");
    }

    public IEnumerator ShowFeedback(string message)
    {
        feedbackText.text = message;
        yield return new WaitForSeconds(3f);
        feedbackText.text = string.Empty;
    }

    private Vector3 GetRandomPositionOnSurface(Vector3 roadPosition)
    {
        float searchRadius = 10f;
        int maxRandomAttempts = 10;

        for (int attempt = 0; attempt < maxRandomAttempts; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-searchRadius, searchRadius),
                0,
                Random.Range(-searchRadius, searchRadius)
            );

            Vector3 potentialPosition = roadPosition + randomOffset;

            if (IsSpawnPositionValid(potentialPosition))
            {
                return potentialPosition;
            }
        }

        return Vector3.zero;
    }

    private Vector3 GetNearestRoadPosition()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f, roadLayerMask);
        if (hitColliders.Length > 0)
        {
            Collider nearestRoad = hitColliders[0];
            float closestDistance = Vector3.Distance(transform.position, nearestRoad.transform.position);

            for (int i = 1; i < hitColliders.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestRoad = hitColliders[i];
                }
            }
            return nearestRoad.transform.position;
        }
        return Vector3.zero;
    }

    private Vector3 FindEmptySpawnPosition(Vector3 roadPosition, int index)
    {
        int maxCircles = 10;
        int pointsPerCircle = 8;

        for (int radius = 1; radius <= maxCircles; radius++)
        {
            float actualRadius = radius * unitSpacing;

            for (int point = 0; point < pointsPerCircle; point++)
            {
                float angle = (2 * Mathf.PI / pointsPerCircle) * point;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * actualRadius,
                    0,
                    Mathf.Sin(angle) * actualRadius
                );

                Vector3 potentialSpawnPosition = roadPosition + offset;

                if (IsSpawnPositionValid(potentialSpawnPosition))
                {
                    return potentialSpawnPosition;
                }
            }

            pointsPerCircle += 4;
        }

        return Vector3.zero;
    }



    private bool IsSpawnPositionValid(Vector3 position)
    {
        if (Physics.CheckSphere(position, 0.5f, unitLayerMask))
        {
            return false;
        }
        if (Physics.Raycast(position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 15f, roadLayerMask))
        {
            return true;
        }
        return false;
    }

    private Vector3 AdjustToRoadLevel(Vector3 position)
    {
        if (Physics.Raycast(position + Vector3.up * 10, Vector3.down, out RaycastHit hit, 50f, roadLayerMask))
        {
            position.y = hit.point.y;
        }
        else
        {
            Debug.LogWarning("No road detected for position adjustment!");
        }
        return position;
    }

    private void ResetIgnoreTowerClick()
    {
        BuildingMng.ignoreTowerClick = false;
    }
    private void SetIgnoreTowerClick(bool state)
    {
        BuildingMng.ignoreTowerClick = state;
    }
}
