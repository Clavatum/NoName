using Attributes;
using Combat;
using Stats;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject player;
    private HealthBar healthBar;
    private Health health;
    public LazyValue<float> _health;

    float damage;

    private void Awake()
    {
        _health = new LazyValue<float>(GetInitialHealth);
    }

    private float GetInitialHealth()
    {
        return GetComponent<BaseStats>().GetBaseStat(Stat.Health);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy") && player.GetComponent<PlayerCombat>().isAttacking)
        {
            GameObject enemy = other.gameObject;

            health = enemy.GetComponent<Health>();

            damage = player.GetComponent<BaseStats>().GetBaseStat(Stat.Damage);

            health.TakeDamage(enemy, damage);

            Debug.Log("Enemy Health after damage: " + enemy.GetComponent<BaseStats>().GetBaseStat(Stat.Health));
        }

        //if (health.IsDead())
        //{
        //    //onDie.Invoke();
        //}
    }
}