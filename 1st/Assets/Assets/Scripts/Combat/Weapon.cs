using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;
    private HealthBar healthBar;
    private Health health;
    public LazyValue<float> _health;

    float damage;

    private void Awake()
    {
        health = enemy.GetComponent<Health>();
        _health = new LazyValue<float>(GetInitialHealth);
    }

    private float GetInitialHealth()
    {
        return GetComponent<BaseStats>().GetBaseStat(Stat.Health);
    }

    private void OnTriggerEnter(Collider other)
    {
        damage = player.GetComponent<BaseStats>().GetBaseStat(Stat.Damage);
        if (other.CompareTag("Enemy") && player.GetComponent<PlayerCombat>().isAttacking)
        {
            health.TakeDamage(enemy, damage);
            Debug.Log(enemy.GetComponent<BaseStats>().GetBaseStat(Stat.Health));
        }

        //if (health.IsDead())
        //{
        //    //onDie.Invoke();
        //}
    }
}