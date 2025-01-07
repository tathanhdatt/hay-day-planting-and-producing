using System;

public interface ILevelXpStorage
{
    event Action<int> OnLevelUpdated;
    event Action<int> OnXpUpdated;
    int GetCurrentLevel();
    void SetCurrentLevel(int level);
    int GetCurrentXp();
    void AddXp(int levelXp);
    float GetXpPercent();
    int GetRequiredXp();
}