using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
public class Progression : ScriptableObject
{
    [SerializeField] ProgressionCharacterClass[] characterClasses = null;

    Dictionary<Characters, Dictionary<Stat, float[]>> lookupTable = null;

    public float GetStat(Stat stat, Characters characterClass, int level)
    {
        BuildLookup();

        float[] levels = lookupTable[characterClass][stat];

        if (levels.Length < level) { return 0; }

        return levels[level - 1];
    }
    public int GetLevels(Stat stat, Characters characterClass)
    {
        BuildLookup();

        float[] levels = lookupTable[characterClass][stat];
        return levels.Length;
    }

    private void BuildLookup()
    {
        if (lookupTable != null) return;

        lookupTable = new Dictionary<Characters, Dictionary<Stat, float[]>>();

        foreach (ProgressionCharacterClass progressionClass in characterClasses)
        { //go into each character class that we've got.

            var statLookupTable = new Dictionary<Stat, float[]>();

            foreach (ProgressionStat progressionStat in progressionClass.stats)
            { //go into all the stats that we've got 
                statLookupTable[progressionStat.stat] = progressionStat.levels;//levels are the variables that we want to create.
            }

            lookupTable[progressionClass.characterClass] = statLookupTable;
        }
    }

    [System.Serializable]
    class ProgressionCharacterClass
    {
        public Characters characterClass;
        public ProgressionStat[] stats;

    }
    [System.Serializable]
    class ProgressionStat
    {
        public Stat stat;
        public float[] levels;
    }
}
