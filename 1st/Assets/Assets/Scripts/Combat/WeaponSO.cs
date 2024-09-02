using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
