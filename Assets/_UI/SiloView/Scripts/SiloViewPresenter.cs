using UnityEngine;

public class SiloViewPresenter : BaseViewPresenter
{
    private SiloView view;

    public SiloViewPresenter(GamePresenter gamePresenter, Transform transform) : base(gamePresenter,
        transform)
    {
    }

    protected override void AddViews()
    {
        this.view = AddView<SiloView>();
    }
}