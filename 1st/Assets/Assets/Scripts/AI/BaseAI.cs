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

    protected virtual void Start()
    {
        animator = GetComponent<Animator>(); 
    }

    protected virtual void FixedUpdate()
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

        // Hedefe doðru yaklaþma
        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget(target); // Hedefe doðru hareket et
        }
        else
        {
            // Hedefe bakmasýný saðla
            LookAtTarget(target);

            Debug.Log("Reached attack range! Preparing to attack...");
            HandleAttack(); // Saldýrý iþlemi
        }

        // Eðer hedef menzil dýþýnda ise hedefi kaybet
        if (distanceToTarget > attackRange)
        {
            target = null; // Hedef kaybedildi
        }
    }

    // Hedefe bakma metodunu ekle
    private void LookAtTarget(Transform target)
    {
        // Hedefin yönünü bul
        Vector3 direction = (target.position - transform.position).normalized;

        // Yalnýzca y eksenini kullanarak bakma açýsýný hesapla
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Mevcut rotasyonu al
        Quaternion currentRotation = transform.rotation;

        // Y rotasyonunu güncelle ve mevcut x rotasyonunu koru
        transform.rotation = Quaternion.Euler(currentRotation.eulerAngles.x, lookRotation.eulerAngles.y, currentRotation.eulerAngles.z);
    }

    protected virtual void HandleAttack()
    {
        if (attackCooldown <= 0f)
        {
            Debug.Log($"Attacking target for {damage} damage!");
            // target.GetComponent<BaseUnit>().TakeDamage(damage);

            if (animator != null)
            {
                animator.SetFloat(verticalParam, 0f);
            }

            attackCooldown = attackInterval; 
        }
    }

    public abstract void MoveTowardsTarget(Transform target); 
}
