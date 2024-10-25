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

    public bool isAttacking; // Tracks whether the AI is currently attacking

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        isAttacking = false; // Initialize isAttacking as false
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
            animator.SetFloat(verticalParam, 1f); // Move animation
        }
        else
        {
            LookAtTarget(target);

            animator.SetFloat(verticalParam, 0f); // Stop movement when in attack range
            HandleAttack();
        }

        if (distanceToTarget > detectionRange)
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
            animator.SetTrigger(attackTrigger); // Trigger attack animation

            //StartAttack(); // Start attack sequence
            attackCooldown = attackInterval;
        }
        else
        {
            //StopAttack(); // Ensure attack stops when not attacking
        }
    }

    /*// New method to start attack
    protected void StartAttack()
    {
        isAttacking = true; // Set isAttacking to true at the start of attack
        animator.SetTrigger(attackTrigger); // Trigger attack animation
    }

    // New method to stop attack
    protected void StopAttack()
    {
        isAttacking = false; // Reset isAttacking when attack ends
    }*/

    // Animation event method to call when attack starts (if using animation events)
    public void OnAttackStart()
    {
        isAttacking = true; // Can be triggered via animation event
    }

    // Animation event method to call when attack ends (if using animation events)
    public void OnAttackEnd()
    {
        isAttacking = false; // Can be triggered via animation event
    }

    public abstract void MoveTowardsTarget(Transform target);
}
