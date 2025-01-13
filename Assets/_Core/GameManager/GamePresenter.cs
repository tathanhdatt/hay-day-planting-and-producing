using System;
using System.Collections.Generic;
using Core.Game;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePresenter : MonoBehaviour
{
    private readonly Dictionary<Type, BaseViewPresenter> presenters =
        new Dictionary<Type, BaseViewPresenter>();

    private GameManager manager;

    public void Enter(GameManager gameManager)
    {
        this.manager = gameManager;
    }

    private void AddPresenters()
    {
        GameViewPresenter gameViewPresenter = new GameViewPresenter(
            this, transform, this.manager.Currency, this.manager.LevelXpStorage);
        AddPresenter(gameViewPresenter);

        ShopViewPresenter shopViewPresenter = new ShopViewPresenter(
            this, transform, this.manager.Currency, this.manager.LevelXpStorage);
        AddPresenter(shopViewPresenter);

        AchievementViewPresenter achievementViewPresenter = new AchievementViewPresenter(
            this, transform);
        AddPresenter(achievementViewPresenter);

        BarnViewPresenter barnViewPresenter = new BarnViewPresenter(
            this, transform);
        AddPresenter(barnViewPresenter);

        SiloViewPresenter siloViewPresenter = new SiloViewPresenter(
            this, transform);
        AddPresenter(siloViewPresenter);
    }

    public async UniTask InitialViewPresenters()
    {
        AddPresenters();
        foreach (BaseViewPresenter presenter in this.presenters.Values)
        {
            await presenter.Initialize();
        }
    }

    private void AddPresenter(BaseViewPresenter presenter)
    {
        this.presenters.Add(presenter.GetType(), presenter);
    }

    public T GetViewPresenter<T>() where T : BaseViewPresenter
    {
        Type type = typeof(T);
        return (T)this.presenters[type];
    }

    public async UniTask HideViewPresenters()
    {
        foreach (BaseViewPresenter viewPresenter in this.presenters.Values)
        {
            await viewPresenter.Hide();
        }
    }
}