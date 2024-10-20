using UnityEngine;
using UnityEngine.UI;

public class UnitProduction : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;
    [SerializeField] private float unitSpacing = 1f;
    [SerializeField] private Button produceUnitButton;
    [SerializeField] private Button upgradeTowerButton;

    [SerializeField] private LayerMask roadLayerMask; 
    [SerializeField] private LayerMask unitLayerMask; 

    private void Awake()
    {
        produceUnitButton.onClick.AddListener(ProduceUnits);
        upgradeTowerButton.onClick.AddListener(UpgradeTower);
    }

    private void ProduceUnits()
    {
        Vector3 roadPosition = GetNearestRoadPosition();
        Debug.Log($"Nearest road position: {roadPosition}"); 

        if (roadPosition != Vector3.zero)
        {
            for (int i = 0; i < 5; i++) 
            {
                Vector3 spawnPosition = FindEmptySpawnPosition(roadPosition, i);
                if (spawnPosition != Vector3.zero)
                {
                    Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                    Debug.Log($"Unit spawned at: {spawnPosition}"); 
                }
                else
                {
                    Debug.Log("No valid spawn position found for unit."); 
                    break; 
                }
            }
            Debug.Log("Units produced");
        }
        else
        {
            Debug.Log("No road found nearby.");
        }
    }

    private Vector3 GetNearestRoadPosition()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10f, roadLayerMask);
        Debug.Log($"Detected road colliders: {hitColliders.Length}");

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

            // Askerin doðrudan yolun üzerine yerleþmesini saðla
            return new Vector3(nearestRoad.transform.position.x, nearestRoad.transform.position.y, nearestRoad.transform.position.z);
        }
        return Vector3.zero;
    }


    private Vector3 FindEmptySpawnPosition(Vector3 roadPosition, int index)
    {
        Vector3 offset = Vector3.right * (index * unitSpacing);
        Vector3 potentialSpawnPosition = roadPosition + offset;

        if (IsSpawnPositionValid(potentialSpawnPosition))
        {
            return potentialSpawnPosition; 
        }

        for (int i = 1; i <= 3; i++)
        {
            potentialSpawnPosition = roadPosition + offset + (Vector3.forward * i * unitSpacing);

            if (IsSpawnPositionValid(potentialSpawnPosition))
            {
                return potentialSpawnPosition; 
            }

            potentialSpawnPosition = roadPosition + offset - (Vector3.forward * i * unitSpacing);

            if (IsSpawnPositionValid(potentialSpawnPosition))
            {
                return potentialSpawnPosition; 
            }
        }

        Debug.Log("No valid spawn position found for unit."); 
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

        Debug.Log("Spawn position is not valid (not on road)."); 
        return false; 
    }

    private void UpgradeTower()
    {
        Debug.Log("Tower upgraded");
    }
}
