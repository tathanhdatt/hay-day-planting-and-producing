public interface ILevelRequirement
{
    int NumberLevel { get; }
    int GetRequiredXp(int level);
}