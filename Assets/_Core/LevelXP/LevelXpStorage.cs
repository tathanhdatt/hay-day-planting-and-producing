using System;
using UnityEngine;

public class LevelXpStorage : ILevelXpStorage
{
    private readonly ILevelRequirement levelRequirement;

    private int currentLevel;
    private int currentXp;
    private int requiredXp;

    public event Action<int> OnLevelUpdated;
    public event Action<int> OnXpUpdated;

    public LevelXpStorage(ILevelRequirement levelRequirement, string data)
    {
        this.levelRequirement = levelRequirement;
        LoadData(data);
    }

    private void LoadData(string data)
    {
        if (data.IsNullOrEmpty()) return;
        LevelXpData levelXpData = JsonUtility.FromJson<LevelXpData>(data);
        this.currentLevel = levelXpData.currentLevel;
        this.currentXp = levelXpData.currentXp;
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

    public string GetJsonData()
    {
        LevelXpData data = new LevelXpData
        {
            currentLevel = this.currentLevel,
            currentXp = this.currentXp,
        };
        return JsonUtility.ToJson(data);
    }
}