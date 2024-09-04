using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "Attack/Normal Attack")]
    public class AttackSO : ScriptableObject
    {
        public AnimatorOverrideController animatorOV;
        public float damage;
    }
}