using UnityEngine;

public class BarnViewPresenter : BaseViewPresenter
{
    private BarnView view;
    public BarnViewPresenter(GamePresenter gamePresenter, Transform transform) : base(gamePresenter, transform)
    {
    }

    protected override void AddViews()
    {
        this.view = AddView<BarnView>();
    }
}