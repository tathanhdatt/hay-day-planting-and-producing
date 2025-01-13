using System;

public class LevelXpStorage : ILevelXpStorage
{
    private readonly ILevelRequirement levelRequirement;

    private int currentLevel;
    private int currentXp;
    private int requiredXp;

    public event Action<int> OnLevelUpdated;
    public event Action<int> OnXpUpdated;

    public LevelXpStorage(ILevelRequirement levelRequirement)
    {
        this.levelRequirement = levelRequirement;
    }

    public int GetCurrentLevel()
    {
        return this.currentLevel;
    }

    public void SetCurrentLevel(int level)
    {
        this.currentLevel = level;
        this.requiredXp = this.levelRequirement.GetRequiredXp(level);
    }

    public int GetCurrentXp()
    {
        return this.currentXp;
    }

    public void AddXp(int levelXp)
    {
        this.currentXp += levelXp;
        bool isEnoughXp = this.currentXp >= this.requiredXp;
        if (isEnoughXp)
        {
            UpdateLevel();
        }

        OnXpUpdated?.Invoke(this.currentXp);
    }

    private void UpdateLevel()
    {
        int nextLevel = Math.Clamp(this.currentLevel + 1, 1, this.levelRequirement.NumberLevel);
        this.currentXp -= this.requiredXp;
        SetCurrentLevel(nextLevel);
        OnLevelUpdated?.Invoke(this.currentLevel);
        Messenger.Broadcast(Message.UpdatedLevel, this.currentLevel);
    }

    public float GetXpPercent()
    {
        return this.currentXp / (float)this.requiredXp;
    }

    public int GetRequiredXp()
    {
        return this.requiredXp;
    }
}