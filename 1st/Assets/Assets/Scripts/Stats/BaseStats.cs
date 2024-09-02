using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStats : MonoBehaviour
{
    [SerializeField] Characters characters;
    [SerializeField] Progression progression = null;

    public float GetBaseStat(Stat stat)
    {
        return progression.GetStat(stat, characters, 1);
    }
}
