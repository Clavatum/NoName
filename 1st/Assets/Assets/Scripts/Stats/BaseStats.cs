using UnityEngine;

namespace Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] Characters characters;
        [SerializeField] Progression progression = null;

        public float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characters, 1);
        }
    }
}
