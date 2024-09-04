using UnityEngine;
using UnityEngine.UI;

namespace Attributes
{
    public class PlayerHealthBar : MonoBehaviour
    {
        Slider _healthSlider;
        private void Start()
        {
            _healthSlider = GetComponent<Slider>();
        }

        public void SetMaxHealth(float maxHealth
            )
        {
            _healthSlider.maxValue = maxHealth;
            _healthSlider.value = maxHealth;
        }

        public void SetHealth(float health)
        {
            _healthSlider.value = health;
        }


    }
}
