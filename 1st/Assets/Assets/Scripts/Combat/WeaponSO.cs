using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Weapon/Melee")]
    public class WeaponSO : ScriptableObject
    {
        [SerializeField] GameObject weapon;
        [SerializeField] float weaponDamage;

        private float GetDamage()
        {
            return weaponDamage;
        }
    }
}
