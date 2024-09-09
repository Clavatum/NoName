using Stats;
using UnityEngine;
using UnityEngine.Events;

namespace Attributes
{
    public class Health : MonoBehaviour
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;
        [SerializeField] private PlayerHealthBar healthBar;

        public LazyValue<float> _health;

        private void Awake()
        {
            _health = new LazyValue<float>(GetInitialHealth);
        }

        public void Start()
        {
            _health.ForceInit();
        }

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetBaseStat(Stat.Health);
        }

        public float GetFraction()
        {
            return _health.value / GetComponent<BaseStats>().GetBaseStat(Stat.Health);
        }

        public bool IsDead()
        {
            return _health.value <= 0;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            _health.value = Mathf.Max(_health.value - damage, 0);
            if (gameObject.tag == "Player")
            {
                healthBar.SetHealth(_health.value);
            }
            //if (IsDead())
            //{
            //    onDie.Invoke();
            //}
            if (gameObject.tag == "Enemy")
            {
                Debug.Log("hit enemy");
                takeDamage.Invoke(damage);
            }
        }
    }
}
