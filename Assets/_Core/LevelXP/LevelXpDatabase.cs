using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Experience Database",
    menuName = "Level Experience Database")]
public class LevelXpDatabase : ScriptableObject
{
    [SerializeField]
    private List<LevelXp> database;
    
    public int NumberOfLevels => this.database.Count;

    public LevelXp GetLevelXp(int level)
    {
        foreach (LevelXp levelXp in database)
        {
            if (levelXp.level == level)
            {
                return levelXp;
            }
        }
        
        return null;
    }

    public bool Contains(int level)
    {
        foreach (LevelXp xp in this.database)
        {
            if (xp.level == level)
            {
                return true;
            }
        }
        
        return false;
    }
}