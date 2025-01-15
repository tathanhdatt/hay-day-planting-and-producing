using UnityEngine;

public class AchievementViewPresenter : BaseViewPresenter
{
    private AchievementView view;

    public AchievementViewPresenter(GamePresenter gamePresenter, Transform transform) : base(
        gamePresenter, transform)
    {
    }

    protected override void AddViews()
    {
        this.view = AddView<AchievementView>();
    }
}