using UnityEngine;

public abstract class BaseAI : MonoBehaviour
{
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackInterval = 1.5f;
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float rotationSpeed = 5f;

    protected float attackCooldown = 0f;
    protected Transform target;
    protected Animator animator;
    protected string verticalParam = "Vertical";
    protected string attackTrigger = "Attack"; 

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            FindTarget();
        }
        else
        {
            HandleTargetAndAttack();
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    protected virtual void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider hitCollider in hitColliders)
        {
            if (IsValidTarget(hitCollider))
            {
                target = hitCollider.transform;
                Debug.Log($"Target detected: {target.name}");
                break;
            }
        }
    }

    protected abstract bool IsValidTarget(Collider hitCollider);

    protected virtual void HandleTargetAndAttack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget(target); 
            animator.SetFloat(verticalParam, 1f);
        }
        else
        {
            LookAtTarget(target);

            Debug.Log("Reached attack range! Preparing to attack...");
            animator.SetFloat(verticalParam, 0f); 
            HandleAttack();
        }

        if (distanceToTarget > attackRange)
        {
            target = null; 
        }
    }

    private void LookAtTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        Quaternion currentRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, lookRotation.eulerAngles.y, currentRotation.eulerAngles.z);
    }

    protected virtual void HandleAttack()
    {
        if (attackCooldown <= 0f)
        {
            animator.SetTrigger(attackTrigger);

            attackCooldown = attackInterval;
        }
    }

    public abstract void MoveTowardsTarget(Transform target);
}
