using System;

public class LevelRequirement : ILevelRequirement
{
    private readonly LevelXpDatabase levelXpDatabase;

    public int NumberLevel => this.levelXpDatabase.NumberOfLevels;

    public LevelRequirement(LevelXpDatabase levelXpDatabase)
    {
        this.levelXpDatabase = levelXpDatabase;
    }

    public int GetRequiredXp(int level)
    {
        if (this.levelXpDatabase.Contains(level))
        {
            return this.levelXpDatabase.GetLevelXp(level).xp;
        }

        throw new ArgumentException($"Level {level} does not exist");
    }
}