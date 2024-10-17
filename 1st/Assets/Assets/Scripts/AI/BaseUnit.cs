using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    [SerializeField] protected float detectionRange = 10f; // Tespit aralýðý
    [SerializeField] protected float attackRange = 2f; // Saldýrý mesafesi
    [SerializeField] protected float attackInterval = 1.5f; // Saldýrý aralýðý
    [SerializeField] protected int damage = 10; // Hasar
    protected float attackCooldown = 0f; // Saldýrý için bekleme süresi
    protected Transform target; // Hedef

    protected virtual void Update()
    {
        if (target == null)
        {
            FindTarget(); // Eðer hedef yoksa tespit et
        }
        else
        {
            HandleMovementAndAttack(); // Hedefe doðru hareket et ve saldýr
        }

        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime; // Saldýrý geri sayýmýný güncelle
        }
    }

    protected virtual void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (Collider hitCollider in hitColliders)
        {
            if (IsValidTarget(hitCollider)) // Geçerli bir hedef mi?
            {
                target = hitCollider.transform; // Hedefi belirle
                Debug.Log($"Target detected: {target.name}");
                break;
            }
        }
    }

    protected abstract bool IsValidTarget(Collider hitCollider); // Geçerli hedef koþulu, asker/düþman özelleþtirilecek

    protected virtual void HandleMovementAndAttack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            if (attackCooldown <= 0f)
            {
                Attack();
                attackCooldown = attackInterval; // Bir sonraki saldýrý için geri sayým
            }
        }
        else
        {
            MoveTowardsTarget(); // Hedefe yaklaþ
        }
    }

    protected virtual void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 2f); // Yürüme hýzý
    }

    protected virtual void Attack()
    {
        Debug.Log($"Attacking target for {damage} damage!");
        // Hedefe zarar ver (örnek)
        // target.GetComponent<BaseUnit>().TakeDamage(damage);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Hasar alma iþlemi örneði
    public virtual void TakeDamage(int amount)
    {
        Debug.Log($"{gameObject.name} took {amount} damage.");
        // Saðlýk güncellemesi yapýlabilir
    }
}
