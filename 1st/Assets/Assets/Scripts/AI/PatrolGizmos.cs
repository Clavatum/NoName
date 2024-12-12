using UnityEngine;

public class PatrolGizmos : MonoBehaviour
{
    public Transform[] patrolPoints;

    private void OnDrawGizmos()
    {
        if (patrolPoints == null || patrolPoints.Length < 2) return;

        Gizmos.color = Color.blue;
        for (int i = 0; i < patrolPoints.Length - 1; i++)
        {
            if (patrolPoints[i] != null && patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
            }
        }
    }
}
