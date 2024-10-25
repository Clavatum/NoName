using UnityEngine;

public class SoldierAI : BaseAI
{
    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Enemy"); 
    }

    public override void MoveTowardsTarget(Transform target)
    {
        float step = moveSpeed * Time.deltaTime; 
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z); 

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        Vector3 direction = (target.position - transform.position).normalized; 
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)); 

        Quaternion currentRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, lookRotation.eulerAngles.y, currentRotation.eulerAngles.z);
    }

}
