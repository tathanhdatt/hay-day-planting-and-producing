using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseViewPresenter
{
    private readonly List<BaseView> views = new List<BaseView>(4);
    protected GamePresenter GamePresenter { get; private set; }
    public Transform Transform { get; private set; }
    public bool IsShowing { get; private set; }

    protected BaseViewPresenter(GamePresenter gamePresenter, Transform transform)
    {
        GamePresenter = gamePresenter;
        Transform = transform;
    }

    public async UniTask Initialize()
    {
        AddViews();
        foreach (BaseView view in this.views)
        {
            await view.Initialize();
        }
    }

    protected abstract void AddViews();

    protected T AddView<T>(bool canShowWithPresenter = true) where T : BaseView
    {
        T view = Transform.GetComponentInChildren<T>();
        view.CanShowWithPresenter = canShowWithPresenter;
        this.views.Add(view);
        return view;
    }

    public async UniTask Show()
    {
        IsShowing = true;
        foreach (BaseView view in this.views)
        {
            if (view.CanShowWithPresenter)
            {
                await view.Show();
            }
        }

        OnShow();
    }

    protected virtual void OnShow()
    {
    }

    public async UniTask Hide()
    {
        IsShowing = false;
        foreach (BaseView view in this.views)
        {
            await view.Hide();
        }

        OnHide();
    }

    protected virtual void OnHide()
    {
    }
}