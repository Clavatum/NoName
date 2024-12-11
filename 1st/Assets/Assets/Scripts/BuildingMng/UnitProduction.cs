using UnityEngine;
using UnityEngine.UI;

public class UnitProduction : MonoBehaviour
{
    [SerializeField] private Transform unitPrefab;
    [SerializeField] private float unitSpacing = 1f;
    [SerializeField] private Button produceUnitButton;

    [SerializeField] private LayerMask roadLayerMask;
    [SerializeField] private LayerMask unitLayerMask;

    [SerializeField] private bool visualizeAreas = true;

    private void Awake()
    {
        produceUnitButton.onClick.AddListener(ProduceUnits);
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
                    spawnPosition = AdjustToRoadLevel(spawnPosition);
                    Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning("No valid spawn position found!");
                    break;
                }
            }
        }
        else
        {
            Debug.LogError("No road detected nearby!");
        }
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
        int searchRadius = 10;
        Vector3 baseOffset = Vector3.right * (index * unitSpacing);

        for (int x = -searchRadius; x <= searchRadius; x++)
        {
            for (int z = -searchRadius; z <= searchRadius; z++)
            {
                Vector3 potentialSpawnPosition = roadPosition + baseOffset + new Vector3(x * unitSpacing, 0, z * unitSpacing);
                if (IsSpawnPositionValid(potentialSpawnPosition))
                {
                    return potentialSpawnPosition;
                }
            }
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

}
